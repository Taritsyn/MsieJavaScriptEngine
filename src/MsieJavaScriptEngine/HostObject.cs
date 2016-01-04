namespace MsieJavaScriptEngine
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;

	using Helpers;

	/// <summary>
	/// Wrapper for object, that implements <see cref="IReflect"/> interface
	/// </summary>
	internal class HostObject : IReflect
	{
		/// <summary>
		/// Target object
		/// </summary>
		private readonly object _target;

		/// <summary>
		/// Target type
		/// </summary>
		private readonly Type _type;

		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		private readonly JsEngineMode _engineMode;

		/// <summary>
		/// List of fields
		/// </summary>
		private readonly FieldInfo[] _fields;

		/// <summary>
		/// List of properties
		/// </summary>
		private readonly PropertyInfo[] _properties;

		/// <summary>
		/// List of methods
		/// </summary>
		private readonly MethodInfo[] _methods;

		/// <summary>
		/// Gets a target object
		/// </summary>
		public object Target
		{
			get { return _target; }
		}


		/// <summary>
		/// Constructs an instance of the wrapper for object, that implements <see cref="IReflect"/> interface
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		public HostObject(object target, JsEngineMode engineMode)
		{
			_target = target;
			_type = target.GetType();
			_engineMode = engineMode;

			var defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;
			_fields = _type.GetFields(defaultBindingFlags);
			_properties = _type.GetProperties(defaultBindingFlags);
			_methods = _type.GetMethods(defaultBindingFlags);
		}

		#region IReflect implementation

		Type IReflect.UnderlyingSystemType
		{
			get { throw new NotImplementedException(); }
		}


		FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
		{
			FieldInfo field = _fields.SingleOrDefault(f => f.Name == name);

			return field;
		}

		FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
		{
			return _fields;
		}

		MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
		{
			MethodInfo method = _methods.SingleOrDefault(m => m.Name == name);

			return method;
		}

		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
		{
			return _methods;
		}

		PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
		{
			return _properties;
		}

		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
		{
			PropertyInfo property = _properties.SingleOrDefault(p => p.Name == name);

			return property;
		}

		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder,
			Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			object result;
			object processedTarget = TypeMappingHelpers.MapToHostType(target);
			object[] processedArgs = TypeMappingHelpers.MapToHostType(args);

			var del = processedTarget as Delegate;
			if (del != null)
			{
				if (_engineMode == JsEngineMode.Classic && processedArgs.Length > 0)
				{
					processedArgs = processedArgs.Skip(1).ToArray();
				}

				result = del.DynamicInvoke(processedArgs);
			}
			else
			{
				BindingFlags processedInvokeAttr = invokeAttr;
				if ((processedInvokeAttr.HasFlag(BindingFlags.GetProperty)
					|| processedInvokeAttr.HasFlag(BindingFlags.PutDispProperty))
					&& !_properties.Any(p => p.Name == name)
					&& _fields.Any(p => p.Name == name))
				{
					if (processedInvokeAttr.HasFlag(BindingFlags.GetProperty))
					{
						processedInvokeAttr &= ~BindingFlags.GetProperty;
						processedInvokeAttr |= BindingFlags.GetField;
					}
					else if (processedInvokeAttr.HasFlag(BindingFlags.PutDispProperty))
					{
						processedInvokeAttr &= ~BindingFlags.PutDispProperty;
						processedInvokeAttr |= BindingFlags.SetField;
					}
				}

				result = _type.InvokeMember(name, processedInvokeAttr, binder, processedTarget,
					processedArgs, modifiers, culture, namedParameters);
			}

			return TypeMappingHelpers.MapToScriptType(result, _engineMode);
		}

		#endregion
	}
}
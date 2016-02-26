namespace MsieJavaScriptEngine
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Base class of item, that implements <see cref="IReflect"/> interface
	/// </summary>
	internal abstract class HostItemBase : IReflect
	{
		/// <summary>
		/// Target type
		/// </summary>
		protected readonly Type _type;

		/// <summary>
		/// Target object
		/// </summary>
		protected readonly object _target;

		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

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
		/// Constructs an instance of the wrapper for item, that implements <see cref="IReflect"/> interface
		/// </summary>
		/// <param name="type">Target type</param>
		/// <param name="target">Target object</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="instance">Flag for whether to allow access to members of the instance</param>
		protected HostItemBase(Type type, object target, JsEngineMode engineMode, bool instance)
		{
			_type = type;
			_target = target;
			_engineMode = engineMode;

			BindingFlags bindingFlags = BindingFlags.Public;
			if (instance)
			{
				bindingFlags |= BindingFlags.Instance;
			}
			else
			{
				bindingFlags |= BindingFlags.Static;
			}

			_fields = _type.GetFields(bindingFlags);
			_properties = _type.GetProperties(bindingFlags);
			_methods = _type.GetMethods(bindingFlags);
		}


		protected abstract object InnerInvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		protected object InvokeStandardMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
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

			object result = _type.InvokeMember(name, processedInvokeAttr, binder, target,
				args, modifiers, culture, namedParameters);

			return result;
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
			return InnerInvokeMember(name, invokeAttr, binder,target, args, modifiers, culture, namedParameters);
		}

		#endregion
	}
}
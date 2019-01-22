#if !NETSTANDARD
using System;
using System.Globalization;
using System.Reflection;

using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine
{
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
		/// JS engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

		/// <summary>
		/// List of fields
		/// </summary>
		private readonly FieldInfo[] _fields;

		/// <summary>
		/// List of field names
		/// </summary>
		private string[] _fieldNames;

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
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="instance">Flag for whether to allow access to members of the instance</param>
		protected HostItemBase(Type type, object target, JsEngineMode engineMode, bool instance)
		{
			_type = type;
			_target = target;
			_engineMode = engineMode;

			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			FieldInfo[] fields = _type.GetFields(defaultBindingFlags);
			string[] fieldNames = fields.Length > 0 ? Array.ConvertAll(fields, f => f.Name) : new string[0];
			PropertyInfo[] properties = _type.GetProperties(defaultBindingFlags);
			MethodInfo[] methods = _type.GetMethods(defaultBindingFlags);
			MethodInfo[] fullyFledgedMethods = methods.Length > 0 ?
				Array.FindAll(methods, ReflectionHelpers.IsFullyFledgedMethod) : methods;

			_fields = fields;
			_fieldNames = fieldNames;
			_properties = properties;
			_methods = fullyFledgedMethods;
		}


		protected abstract object InnerInvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		protected object InvokeStandardMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			BindingFlags processedInvokeAttr = invokeAttr;
			if ((processedInvokeAttr.HasFlag(BindingFlags.GetProperty)
				|| processedInvokeAttr.HasFlag(BindingFlags.SetProperty)
				|| processedInvokeAttr.HasFlag(BindingFlags.PutDispProperty))
				&& Array.IndexOf(_fieldNames, name) != -1)
			{
				if (processedInvokeAttr.HasFlag(BindingFlags.GetProperty))
				{
					processedInvokeAttr &= ~BindingFlags.GetProperty;
					processedInvokeAttr |= BindingFlags.GetField;
				}
				else if (processedInvokeAttr.HasFlag(BindingFlags.SetProperty))
				{
					processedInvokeAttr &= ~BindingFlags.SetProperty;
					processedInvokeAttr |= BindingFlags.SetField;
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
#endif
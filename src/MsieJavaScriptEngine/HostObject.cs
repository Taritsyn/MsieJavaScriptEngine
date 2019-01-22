#if !NETSTANDARD
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Wrapper for object, that implements <see cref="IReflect"/> interface
	/// </summary>
	internal sealed class HostObject : HostItemBase
	{
		/// <summary>
		/// Number of delegate parameters
		/// </summary>
		private int _delegateParameterCount = int.MinValue;


		/// <summary>
		/// Constructs an instance of the wrapper for object, that implements <see cref="IReflect"/> interface
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="engineMode">JS engine mode</param>
		public HostObject(object target, JsEngineMode engineMode)
			: base(target.GetType(), target, engineMode, true)
		{
			if (_engineMode == JsEngineMode.Classic)
			{
				var del = _target as Delegate;
				if (del != null)
				{
					_delegateParameterCount = del.Method.GetParameters().Length;
				}
			}
		}


		#region HostItemBase overrides

		protected override object InnerInvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			object result;
			object processedTarget = TypeMappingHelpers.MapToHostType(target);

			if (name == SpecialMemberName.Default && processedTarget is Delegate)
			{
				var del = (Delegate)processedTarget;
				object[] processedArgs = args;

				if (_engineMode == JsEngineMode.Classic
					&& processedArgs.Length > 0
					&& (processedArgs.Length - _delegateParameterCount) == 1)
				{
					processedArgs = processedArgs.Skip(1).ToArray();
				}
				processedArgs = TypeMappingHelpers.MapToHostType(processedArgs);

				result = del.DynamicInvoke(processedArgs);
			}
			else
			{
				object[] processedArgs = TypeMappingHelpers.MapToHostType(args);

				result = InvokeStandardMember(name, invokeAttr, binder, processedTarget,
					processedArgs, modifiers, culture, namedParameters);
			}

			return TypeMappingHelpers.MapToScriptType(result, _engineMode);
		}

		#endregion
	}
}
#endif
#if !NETSTANDARD1_3
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
		/// Constructs an instance of the wrapper for object, that implements <see cref="IReflect"/> interface
		/// </summary>
		/// <param name="target">Target object</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		public HostObject(object target, JsEngineMode engineMode)
			: base(target.GetType(), target, engineMode, true)
		{ }


		private object InvokeDelegate(Delegate del, object[] args)
		{
			if (del == null)
			{
				throw new ArgumentNullException("del");
			}

			object[] processedArgs = args;

			if (_engineMode == JsEngineMode.Classic && processedArgs.Length > 0)
			{
				processedArgs = processedArgs.Skip(1).ToArray();
			}

			object result = del.DynamicInvoke(processedArgs);

			return result;
		}

		#region HostItemBase implementation

		protected override object InnerInvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			object result;
			object processedTarget = TypeMappingHelpers.MapToHostType(target);
			object[] processedArgs = TypeMappingHelpers.MapToHostType(args);

			if (name == SpecialMemberName.Default && processedTarget is Delegate)
			{
				var del = (Delegate)processedTarget;
				result = InvokeDelegate(del, processedArgs);
			}
			else
			{
				result = InvokeStandardMember(name, invokeAttr, binder, processedTarget,
					processedArgs, modifiers, culture, namedParameters);
			}

			return TypeMappingHelpers.MapToScriptType(result, _engineMode);
		}

		#endregion
	}
}
#endif
#if NETFRAMEWORK
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Wrapper for type, that implements <see cref="IReflect"/> interface
	/// </summary>
	internal sealed class HostType : HostItemBase
	{
		/// <summary>
		/// Constructs an instance of the wrapper for type, that implements <see cref="IReflect"/> interface
		/// </summary>
		/// <param name="type">Target type</param>
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="allowReflection">Flag for whether to allow the usage of reflection API in the script code</param>
		public HostType(Type type, JsEngineMode engineMode, bool allowReflection)
			: base(type, null, engineMode, allowReflection, false)
		{ }


		#region HostItemBase overrides

		protected override object InnerInvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
			object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			object result;

			if (name == SpecialMemberName.Default && invokeAttr.HasFlag(BindingFlags.CreateInstance))
			{
				object[] processedArgs = args;

				if (_engineMode != JsEngineMode.Classic && processedArgs.Length > 0)
				{
					processedArgs = processedArgs.Skip(1).ToArray();
				}
				processedArgs = TypeMappingHelpers.MapToHostType(processedArgs);

				result = Activator.CreateInstance(_type, processedArgs);
			}
			else
			{
				object[] processedArgs = TypeMappingHelpers.MapToHostType(args);

				result = InvokeStandardMember(name, invokeAttr, binder, target,
					processedArgs, modifiers, culture, namedParameters);
			}

			return TypeMappingHelpers.MapToScriptType(result, _engineMode, _allowReflection);
		}

		#endregion
	}
}
#endif
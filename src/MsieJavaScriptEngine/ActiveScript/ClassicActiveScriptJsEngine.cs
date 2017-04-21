#if !NETSTANDARD1_3
using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Classic JS engine
	/// </summary>
	internal sealed class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		/// <summary>
		/// Flag indicating whether this JS engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs instance of the Classic Active Script engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public ClassicActiveScriptJsEngine(bool enableDebugging, bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(JsEngineMode.Classic, enableDebugging, useEcmaScript5Polyfill, useJson2Library)
		{ }


		/// <summary>
		/// Checks a support of the Classic Active Script engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(ClassId.Classic, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
		}
	}
}
#endif
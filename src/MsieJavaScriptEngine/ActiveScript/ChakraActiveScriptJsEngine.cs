#if !NETSTANDARD1_3
using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Chakra JS engine
	/// </summary>
	internal sealed class ChakraActiveScriptJsEngine : ActiveScriptJsEngineBase
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
		/// Constructs instance of the Chakra Active Script engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ChakraActiveScriptJsEngine(bool enableDebugging)
			: base(JsEngineMode.ChakraActiveScript, enableDebugging, false, false)
		{ }


		/// <summary>
		/// Checks a support of the Chakra Active Script engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(ClassId.Chakra, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
		}
	}
}
#endif
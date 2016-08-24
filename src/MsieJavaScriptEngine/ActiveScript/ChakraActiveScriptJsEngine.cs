namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// ActiveScript version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		private const string CHAKRA_CLSID = "{16d51579-a30b-4c8b-a276-0ff4dc41e755}";

		/// <summary>
		/// Flag indicating whether this JavaScript engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs instance of the Chakra ActiveScript JavaScript engine
		/// </summary>
		public ChakraActiveScriptJsEngine()
			: base(CHAKRA_CLSID, JsEngineMode.ChakraActiveScript, "9",
				ScriptLanguageVersion.EcmaScript5, false, false)
		{ }


		/// <summary>
		/// Checks a support of the Chakra ActiveScript JavaScript engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(CHAKRA_CLSID, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
		}
	}
}
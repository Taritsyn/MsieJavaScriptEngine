namespace MsieJavaScriptEngine.ActiveScript
{
	using Constants;

	/// <summary>
	/// ActiveScript version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		private const string CHAKRA_CLSID = "{16d51579-a30b-4c8b-a276-0ff4dc41e755}";


		/// <summary>
		/// Constructs instance of the Chakra ActiveScript JavaScript engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public ChakraActiveScriptJsEngine(bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(CHAKRA_CLSID, JsEngineModeName.ChakraActiveScript, "9",
				useEcmaScript5Polyfill, useJson2Library)
		{ }


		/// <summary>
		/// Checks a support of the Chakra ActiveScript JavaScript engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(CHAKRA_CLSID);

			return isSupported;
		}
	}
}
namespace MsieJavaScriptEngine.ActiveScript
{
	using Constants;
	
	/// <summary>
	/// Classic MSIE JavaScript engine
	/// </summary>
	internal sealed class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		private const string CLASSIC_CLSID = "{f414c260-6ac0-11cf-b6d1-00aa00bbbb58}";


		/// <summary>
		/// Constructs instance of the Classic MSIE JavaScript engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public ClassicActiveScriptJsEngine(bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(CLASSIC_CLSID, JsEngineModeName.Classic, "6", 
				useEcmaScript5Polyfill, useJson2Library)
		{ }


		/// <summary>
		/// Checks a support of the Classic MSIE JavaScript engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(CLASSIC_CLSID);

			return isSupported;
		}
	}
}
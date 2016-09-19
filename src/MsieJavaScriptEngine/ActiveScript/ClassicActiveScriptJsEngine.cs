#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Classic MSIE JavaScript engine
	/// </summary>
	internal sealed class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		private const string CLASSIC_CLSID = "{f414c260-6ac0-11cf-b6d1-00aa00bbbb58}";

		/// <summary>
		/// Flag indicating whether this JavaScript engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs instance of the Classic MSIE JavaScript engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public ClassicActiveScriptJsEngine(bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(CLASSIC_CLSID, JsEngineMode.Classic, "6",
				ScriptLanguageVersion.None, useEcmaScript5Polyfill, useJson2Library)
		{ }


		/// <summary>
		/// Checks a support of the Classic MSIE JavaScript engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(CLASSIC_CLSID, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
		}
	}
}
#endif
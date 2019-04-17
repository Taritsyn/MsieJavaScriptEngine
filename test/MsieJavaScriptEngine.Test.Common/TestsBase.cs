namespace MsieJavaScriptEngine.Test.Common
{
	public abstract class TestsBase
	{
		/// <summary>
		/// Gets a JS engine mode
		/// </summary>
		protected abstract JsEngineMode EngineMode { get; }

		/// <summary>
		/// Gets a flag for whether to use the ECMAScript 5 Polyfill
		/// </summary>
		protected virtual bool UseEcmaScript5Polyfill => false;

		/// <summary>
		/// Gets a flag for whether to use the JSON2 library
		/// </summary>
		protected virtual bool UseJson2Library => false;


		protected MsieJsEngine CreateJsEngine()
		{
			return CreateJsEngine(false);
		}

		protected MsieJsEngine CreateJsEngine(bool enableDebugging)
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EnableDebugging = enableDebugging,
				EngineMode = EngineMode,
				UseEcmaScript5Polyfill = UseEcmaScript5Polyfill,
				UseJson2Library = UseJson2Library
			});

			return jsEngine;
		}
	}
}
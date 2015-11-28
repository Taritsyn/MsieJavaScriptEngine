namespace MsieJavaScriptEngine.Test.A_Auto
{
	using MsieJavaScriptEngine;

	public class CommonTests : CommonTestsBase
	{
		protected override MsieJsEngine CreateJsEngine()
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.Auto,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}
	}
}
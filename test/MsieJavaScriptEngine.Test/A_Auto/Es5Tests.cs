namespace MsieJavaScriptEngine.Test.A_Auto
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		protected override MsieJsEngine CreateJsEngine()
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.Auto,
				UseEcmaScript5Polyfill = true,
				UseJson2Library = true
			});

			return jsEngine;
		}
	}
}
namespace MsieJavaScriptEngine.Test.Auto
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;
	using Common;

	[TestFixture]
	public class InteropTests : InteropTestsBase
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
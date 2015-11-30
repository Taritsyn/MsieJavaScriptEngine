namespace MsieJavaScriptEngine.Test.ChakraEdgeJsRt
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;
	using Common;

	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		protected override MsieJsEngine CreateJsEngine()
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraEdgeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}
	}
}
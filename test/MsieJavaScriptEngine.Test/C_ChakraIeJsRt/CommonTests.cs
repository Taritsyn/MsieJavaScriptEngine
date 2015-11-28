namespace MsieJavaScriptEngine.Test.C_ChakraIeJsRt
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		protected override MsieJsEngine CreateJsEngine()
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraIeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}
	}
}
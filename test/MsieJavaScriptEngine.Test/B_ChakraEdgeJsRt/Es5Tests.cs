namespace MsieJavaScriptEngine.Test.B_ChakraEdgeJsRt
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
				EngineMode = JsEngineMode.ChakraEdgeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}
	}
}
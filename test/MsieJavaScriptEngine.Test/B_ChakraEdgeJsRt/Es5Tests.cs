namespace MsieJavaScriptEngine.Test.B_ChakraEdgeJsRt
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraEdgeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});
		}
	}
}
namespace MsieJavaScriptEngine.Tests.B_ChakraJsRt
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});
		}
	}
}
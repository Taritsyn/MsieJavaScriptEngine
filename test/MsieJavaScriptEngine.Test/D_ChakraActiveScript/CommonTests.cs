namespace MsieJavaScriptEngine.Test.D_ChakraActiveScript
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
				EngineMode = JsEngineMode.ChakraActiveScript,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});
		}
	}
}
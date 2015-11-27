namespace MsieJavaScriptEngine.Test.E_Classic
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
				EngineMode = JsEngineMode.Classic,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});
		}
	}
}
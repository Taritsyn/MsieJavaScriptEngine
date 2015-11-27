namespace MsieJavaScriptEngine.Test.A_Auto
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	public class CommonTests : CommonTestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.Auto,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});
		}
	}
}
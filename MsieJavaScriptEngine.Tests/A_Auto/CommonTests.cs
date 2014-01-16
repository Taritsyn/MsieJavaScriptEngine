namespace MsieJavaScriptEngine.Tests.A_Auto
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	public class CommonTests : CommonTestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(JsEngineMode.Auto, false, false);
		}
	}
}
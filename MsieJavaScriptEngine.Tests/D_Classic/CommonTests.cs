namespace MsieJavaScriptEngine.Tests.D_Classic
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(JsEngineMode.Classic, false, false);
		}
	}
}
namespace MsieJavaScriptEngine.Tests.C_ChakraActiveScript
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(JsEngineMode.ChakraActiveScript, false, false);
		}
	}
}
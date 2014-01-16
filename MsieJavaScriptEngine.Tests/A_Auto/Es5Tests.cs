namespace MsieJavaScriptEngine.Tests.A_Auto
{
	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		[TestFixtureSetUp]
		public override void SetUp()
		{
			_jsEngine = new MsieJsEngine(JsEngineMode.Auto, true, true);
		}
	}
}
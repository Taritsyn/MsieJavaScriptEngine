using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class MultithreadingTests : MultithreadingTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;
	}
}
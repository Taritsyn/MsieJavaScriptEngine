using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;
	}
}
using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class PrecompilationTests : PrecompilationTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;
	}
}
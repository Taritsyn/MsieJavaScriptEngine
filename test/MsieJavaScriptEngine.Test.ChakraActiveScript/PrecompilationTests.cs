using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	[TestFixture]
	public class PrecompilationTests : PrecompilationTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraActiveScript;
	}
}
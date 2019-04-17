using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraActiveScript;
	}
}
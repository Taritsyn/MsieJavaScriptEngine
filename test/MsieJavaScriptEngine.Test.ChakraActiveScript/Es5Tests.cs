using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraActiveScript;
	}
}
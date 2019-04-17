using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraEdgeJsRt
{
	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraEdgeJsRt;
	}
}
using System;
using System.Text.RegularExpressions;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraEdgeJsRt
{
	[TestFixture]
	public class MultithreadingTests : MultithreadingTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraEdgeJsRt;
	}
}
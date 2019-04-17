using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraIeJsRt
{
	[TestFixture]
	public class MultithreadingTests : MultithreadingTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraIeJsRt;
	}
}
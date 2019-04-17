using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	[TestFixture]
	public class MultithreadingTests : MultithreadingTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraActiveScript;
	}
}
using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Classic
{
	[TestFixture]
	public class MultithreadingTests : MultithreadingTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Classic;
	}
}
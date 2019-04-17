using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Classic
{
	[TestFixture]
	public class PrecompilationTests : PrecompilationTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Classic;
	}
}
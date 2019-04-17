using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Classic
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Classic;
	}
}
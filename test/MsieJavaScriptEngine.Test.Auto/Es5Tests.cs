using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;
		protected override bool UseEcmaScript5Polyfill => true;
		protected override bool UseJson2Library => true;
	}
}
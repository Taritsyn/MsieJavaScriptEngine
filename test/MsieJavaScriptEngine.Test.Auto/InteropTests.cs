﻿using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;
	}
}
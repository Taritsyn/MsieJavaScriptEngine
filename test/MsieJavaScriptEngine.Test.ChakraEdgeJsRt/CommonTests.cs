using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraEdgeJsRt
{
	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		protected override MsieJsEngine CreateJsEngine(bool enableDebugging)
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EnableDebugging = enableDebugging,
				EngineMode = JsEngineMode.ChakraEdgeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}

		#region Mapping errors

		[Test]
		public void GenerationOfParseErrorMessageIsCorrect()
		{
			// Arrange
			const string input = @"var arr = [];
var obj = {};
var foo = 'Browser's bar';";
			string targetOutput = "Compile error: Expected ';'" + Environment.NewLine +
				"   at 3:20"
				;

			JsException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					jsEngine.Execute(input, "variables.js");
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
			Assert.IsNotEmpty(exception.Message);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		[Test]
		public void GenerationOfParseErrorMessageInDebugModeIsCorrect()
		{
			// Arrange
			const string input = @"var arr = [];
var obj = {};
var foo = 'Browser's bar';";
			string targetOutput = "Compile error: Expected ';'" + Environment.NewLine +
				"   at 3:20"
				;

			JsException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine(true))
			{
				try
				{
					jsEngine.Execute(input, "variables.js");
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
			Assert.IsNotEmpty(exception.Message);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		[Test]
		public void GenerationOfRuntimeErrorMessageIsCorrect()
		{
			// Arrange
			const string input = @"function foo(x, y) {
	var z = x + y;
	if (z > 20) {
		bar();
	}
}

var a = 8;
var b = 15;

foo(a, b);";
			string targetOutput = "ReferenceError: 'bar' is undefined\n" +
				"   at foo (functions.js:4:3)\n" +
				"   at Global code (functions.js:11:1)"
				;

			JsException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					jsEngine.Execute(input, "functions.js");
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
			Assert.IsNotEmpty(exception.Message);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		#endregion
	}
}
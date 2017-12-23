using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	[TestFixture]
	public class CommonTests : CommonTestsBase
	{
		protected override MsieJsEngine CreateJsEngine(bool enableDebugging)
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EnableDebugging = enableDebugging,
				EngineMode = JsEngineMode.ChakraActiveScript,
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
			string targetOutput = "Compilation error: Expected ';'" + Environment.NewLine +
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
			string targetOutput = "Compilation error: Expected ';'" + Environment.NewLine +
				"   at variables.js:3:20"
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
			string targetOutput = "Runtime error: 'bar' is undefined" + Environment.NewLine +
				"   at 4:3";

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

		[Test]
		public override void MappingRuntimeErrorDuringEvaluationOfExpressionIsCorrect()
		{
			// Arrange
			const string input = @"var $variable1 = 611;
var _variable2 = 711;
var @variable3 = 678;

$variable1 + _variable2 - variable3;";

			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					int result = jsEngine.Evaluate<int>(input);
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.IsNotEmpty(exception.Message);
			Assert.AreEqual(3, exception.LineNumber);
			Assert.AreEqual(15, exception.ColumnNumber);
		}

		#endregion
	}
}
using System;
using System.Text.RegularExpressions;

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

		#region Error handling

		#region Mapping of errors

		[Test]
		public void MappingCompilationErrorDuringEvaluationOfExpressionIsCorrect()
		{
			// Arrange
			const string input = @"var $variable1 = 611;
var _variable2 = 711;
var @variable3 = 678;

$variable1 + _variable2 - @variable3;";

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					int result = jsEngine.Evaluate<int>(input, "variables.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Compilation error", exception.Category);
			Assert.AreEqual("Invalid character", exception.Description);
			Assert.AreEqual("SyntaxError", exception.Type);
			Assert.AreEqual("variables.js", exception.DocumentName);
			Assert.AreEqual(3, exception.LineNumber);
			Assert.AreEqual(5, exception.ColumnNumber);
			Assert.AreEqual("var @variable3 = 678;", exception.SourceFragment);
		}

		[Test]
		public void MappingCompilationErrorDuringEvaluationOfExpressionInDebugModeIsCorrect()
		{
			// Arrange
			const string input = @"var $variable1 = 611;
var _variable2 = 711;
var @variable3 = 678;

$variable1 + _variable2 - @variable3;";

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine(true))
			{
				try
				{
					int result = jsEngine.Evaluate<int>(input, "variables.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Compilation error", exception.Category);
			Assert.AreEqual("Invalid character", exception.Description);
			Assert.AreEqual("SyntaxError", exception.Type);
			Assert.AreEqual("variables.js", exception.DocumentName);
			Assert.AreEqual(3, exception.LineNumber);
			Assert.AreEqual(5, exception.ColumnNumber);
			Assert.AreEqual("var @variable3 = 678;", exception.SourceFragment);
		}

		[Test]
		public void MappingRuntimeErrorDuringEvaluationOfExpressionIsCorrect()
		{
			// Arrange
			const string input = @"var $variable1 = 611;
var _variable2 = 711;
var variable3 = 678;

$variable1 + -variable2 - variable3;";

			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					int result = jsEngine.Evaluate<int>(input, "variables.js");
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.True(Regex.IsMatch(exception.Description, @"^'variable2' is (un|not )defined$"));
			Assert.AreEqual("ReferenceError", exception.Type);
			Assert.AreEqual("variables.js", exception.DocumentName);
			Assert.AreEqual(5, exception.LineNumber);
			Assert.AreEqual(1, exception.ColumnNumber);
			Assert.IsEmpty(exception.SourceFragment);
			Assert.AreEqual("   at Global code (variables.js:5:1)", exception.CallStack);
		}

		[Test]
		public void MappingCompilationErrorDuringExecutionOfCodeIsCorrect()
		{
			// Arrange
			const string input = @"function factorial(value) {
	if (value <= 0) {
		throw new Error(""The value must be greater than or equal to zero."");
	}

	return value !== 1 ? value * factorial(value - 1) : 1;
}

factorial(5);
factorial(2%);
factorial(0);";

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					jsEngine.Execute(input, "factorial.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Compilation error", exception.Category);
			Assert.AreEqual("Syntax error", exception.Description);
			Assert.AreEqual("SyntaxError", exception.Type);
			Assert.AreEqual("factorial.js", exception.DocumentName);
			Assert.AreEqual(10, exception.LineNumber);
			Assert.AreEqual(13, exception.ColumnNumber);
			Assert.AreEqual("factorial(2%);", exception.SourceFragment);
		}

		[Test]
		public virtual void MappingCompilationErrorDuringExecutionOfCodeInDebugModeIsCorrect()
		{
			// Arrange
			const string input = @"function factorial(value) {
	if (value <= 0) {
		throw new Error(""The value must be greater than or equal to zero."");
	}

	return value !== 1 ? value * factorial(value - 1) : 1;
}

factorial(5);
factorial(2%);
factorial(0);";

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine(true))
			{
				try
				{
					jsEngine.Execute(input, "factorial.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Compilation error", exception.Category);
			Assert.AreEqual("Syntax error", exception.Description);
			Assert.AreEqual("SyntaxError", exception.Type);
			Assert.AreEqual("factorial.js", exception.DocumentName);
			Assert.AreEqual(10, exception.LineNumber);
			Assert.AreEqual(13, exception.ColumnNumber);
			Assert.AreEqual("factorial(2%);", exception.SourceFragment);
		}

		[Test]
		public void MappingRuntimeErrorDuringExecutionOfCodeIsCorrect()
		{
			// Arrange
			const string input = @"function factorial(value) {
	if (value <= 0) {
		throw new Error(""The value must be greater than or equal to zero."");
	}

	return value !== 1 ? value * factorial(value - 1) : 1;
}

factorial(5);
factorial(-1);
factorial(0);";

			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					jsEngine.Execute(input, "factorial.js");
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("The value must be greater than or equal to zero.", exception.Description);
			Assert.AreEqual("Error", exception.Type);
			Assert.AreEqual("factorial.js", exception.DocumentName);
			Assert.AreEqual(3, exception.LineNumber);
			Assert.AreEqual(3, exception.ColumnNumber);
			Assert.IsEmpty(exception.SourceFragment);
			Assert.AreEqual(
				"   at factorial (factorial.js:3:3)" + Environment.NewLine +
				"   at Global code (factorial.js:10:1)",
				exception.CallStack
			);
		}

		#endregion

		#region Generation of error messages

		[Test]
		public void GenerationOfCompilationErrorMessageIsCorrect()
		{
			// Arrange
			const string input = @"var arr = [];
var obj = {};
var foo = 'Browser's bar';";
			string targetOutput = "SyntaxError: Expected ';'" + Environment.NewLine +
				"   at variables.js:3:20 -> var foo = 'Browser's bar';"
				;

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					jsEngine.Execute(input, "variables.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		[Test]
		public void GenerationOfCompilationErrorMessageInDebugModeIsCorrect()
		{
			// Arrange
			const string input = @"var arr = [];
var obj = {};
var foo = 'Browser's bar';";
			string targetOutput = "SyntaxError: Expected ';'" + Environment.NewLine +
				"   at variables.js:3:20 -> var foo = 'Browser's bar';"
				;

			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine(true))
			{
				try
				{
					jsEngine.Execute(input, "variables.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
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
			string targetOutputPattern = @"^ReferenceError: 'bar' is (un|not )defined" + Environment.NewLine +
				@"   at foo \(functions.js:4:3\)" + Environment.NewLine +
				@"   at Global code \(functions.js:11:1\)$"
				;

			JsRuntimeException exception = null;

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
			Assert.True(Regex.IsMatch(exception.Message, targetOutputPattern));
		}

		#endregion

		#endregion
	}
}
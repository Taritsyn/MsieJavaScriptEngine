using System;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraIeJsRt
{
	[TestFixture]
	public class PrecompilationTests : PrecompilationTestsBase
	{
		protected override MsieJsEngine CreateJsEngine(bool enableDebugging)
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EnableDebugging = enableDebugging,
				EngineMode = JsEngineMode.ChakraIeJsRt,
				UseEcmaScript5Polyfill = false,
				UseJson2Library = false
			});

			return jsEngine;
		}

		#region Error handling

		#region Mapping of errors

		[Test]
		public void MappingCompilationErrorDuringPrecompilationOfCodeIsCorrect()
		{
			// Arrange
			const string input = @"function guid() {
	function s4() {
		return Math.floor((1 + Math.random() * 0x10000)
			.toString(16)
			.substring(1)
			;
	}

	var result = s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();

	return result;
}";

			PrecompiledScript precompiledScript = null;
			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					precompiledScript = jsEngine.Precompile(input, "guid.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.Null(precompiledScript);
			Assert.NotNull(exception);
			Assert.AreEqual("Compilation error", exception.Category);
			Assert.AreEqual("Expected ')'", exception.Description);
			Assert.AreEqual("SyntaxError", exception.Type);
			Assert.AreEqual("guid.js", exception.DocumentName);
			Assert.AreEqual(6, exception.LineNumber);
			Assert.AreEqual(4, exception.ColumnNumber);
			Assert.AreEqual("			;", exception.SourceFragment);
		}

		[Test]
		public void MappingRuntimeErrorDuringExecutionOfPrecompiledCodeIsCorrect()
		{
			// Arrange
			const string input = @"function getItem(items, itemIndex) {
	var item = items[itemIndex];

	return item;
}

(function (getItem) {
	var items = null,
		item = getItem(items, 5)
		;

	return item;
})(getItem);";

			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					PrecompiledScript precompiledScript = jsEngine.Precompile(input, "getItem.js");
					jsEngine.Execute(precompiledScript);
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("Unable to get property '5' of undefined or null reference", exception.Description);
			Assert.AreEqual("TypeError", exception.Type);
			Assert.AreEqual("getItem.js", exception.DocumentName);
			Assert.AreEqual(2, exception.LineNumber);
			Assert.AreEqual(2, exception.ColumnNumber);
			Assert.IsEmpty(exception.SourceFragment);
			Assert.AreEqual(
				"   at getItem (getItem.js:2:2)" + Environment.NewLine +
				"   at Anonymous function (getItem.js:9:3)" + Environment.NewLine +
				"   at Global code (getItem.js:7:2)",
				exception.CallStack
			);
		}

		#endregion

		#region Generation of error messages

		[Test]
		public void GenerationOfCompilationErrorMessageIsCorrect()
		{
			// Arrange
			const string input = @"function makeId(length) {
	var result = '',
		possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789',
		charIndex
		;

	for (charIndex = 0; charIndex < length; charIndex++) 
		result += possible.charAt(Math.floor(Math.random() * possible.length));
	}

	return result;
}";
			string targetOutput = "SyntaxError: 'return' statement outside of function" + Environment.NewLine +
				"   at makeId.js:11:2 -> 	return result;"
				;

			PrecompiledScript precompiledScript = null;
			JsCompilationException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					precompiledScript = jsEngine.Precompile(input, "makeId.js");
				}
				catch (JsCompilationException e)
				{
					exception = e;
				}
			}

			Assert.Null(precompiledScript);
			Assert.NotNull(exception);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		[Test]
		public void GenerationOfRuntimeErrorMessageIsCorrect()
		{
			// Arrange
			const string input = @"function getFullName(firstName, lastName) {
	var fullName = firstName + ' ' + middleName + ' ' + lastName;

	return fullName;
}

(function (getFullName) {
	var firstName = 'Vasya',
		lastName = 'Pupkin'
		;

	return getFullName(firstName, lastName);
})(getFullName);";
			string targetOutput = "ReferenceError: 'middleName' is undefined" + Environment.NewLine +
				"   at getFullName (getFullName.js:2:2)" + Environment.NewLine +
				"   at Anonymous function (getFullName.js:12:2)" + Environment.NewLine +
				"   at Global code (getFullName.js:7:2)"
				;

			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					PrecompiledScript precompiledScript = jsEngine.Precompile(input, "getFullName.js");
					jsEngine.Execute(precompiledScript);
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			Assert.NotNull(exception);
			Assert.AreEqual(targetOutput, exception.Message);
		}

		#endregion

		#endregion
	}
}
using System;
using System.IO;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraIeJsRt
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraIeJsRt;


		#region Embedding of objects

		#region Recursive calls

		#region Mapping of errors

		[Test]
		public void MappingRuntimeErrorDuringRecursiveEvaluationOfFiles()
		{
			// Arrange
			string directoryPath = GetAbsolutePath("SharedFiles/recursiveEvaluation/runtimeError");
			const string input = "require('index').calculateResult();";

			// Act
			JsRuntimeException exception = null;

			using (var jsEngine = CreateJsEngine())
			{
				try
				{
					Func<string, object> loadModule = path => {
						string absolutePath = Path.Combine(directoryPath, $"{path}.js");
						string code = File.ReadAllText(absolutePath);
						object result = jsEngine.Evaluate(code, absolutePath);

						return result;
					};

					jsEngine.EmbedHostObject("require", loadModule);
					double output = jsEngine.Evaluate<double>(input);
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("'argumens' is undefined", exception.Description);
			Assert.AreEqual("ReferenceError", exception.Type);
			Assert.AreEqual("math.js", exception.DocumentName);
			Assert.AreEqual(10, exception.LineNumber);
			Assert.AreEqual(4, exception.ColumnNumber);
			Assert.IsEmpty(exception.SourceFragment);
			Assert.AreEqual(
				"   at sum (math.js:10:4)" + Environment.NewLine +
				"   at calculateResult (index.js:7:4)" + Environment.NewLine +
				"   at Global code (Script Document:1:1)",
				exception.CallStack
			);
		}

		#endregion

		#endregion

		#endregion
	}
}
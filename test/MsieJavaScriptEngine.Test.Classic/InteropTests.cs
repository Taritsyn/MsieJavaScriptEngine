﻿using System;
using System.IO;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;
using MsieJavaScriptEngine.Test.Common.Interop;
using MsieJavaScriptEngine.Test.Common.Interop.Animals;

namespace MsieJavaScriptEngine.Test.Classic
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Classic;


		#region Embedding of objects

		#region Objects with methods

		[Test]
		public override void EmbeddingOfInstanceOfCustomValueTypeAndCallingOfItsGetTypeMethod()
		{
			// Arrange
			string TestAllowReflectionSetting(bool allowReflection)
			{
				var date = new Date();

				using (var jsEngine = CreateJsEngine(allowReflection: allowReflection))
				{
					jsEngine.EmbedHostObject("date", date);
					return jsEngine.Evaluate<string>("date.GetType();");
				}
			}

			// Act and Assert
			Assert.AreEqual(typeof(Date).FullName, TestAllowReflectionSetting(true));

			var exception = Assert.Throws<JsRuntimeException>(() => TestAllowReflectionSetting(false));
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("Object doesn't support this property or method", exception.Description);
		}

		[Test]
		public override void EmbeddingOfInstanceOfCustomReferenceTypeAndCallingOfItsGetTypeMethod()
		{
			// Arrange
			string TestAllowReflectionSetting(bool allowReflection)
			{
				var cat = new Cat();

				using (var jsEngine = CreateJsEngine(allowReflection: allowReflection))
				{
					jsEngine.EmbedHostObject("cat", cat);
					return jsEngine.Evaluate<string>("cat.GetType();");
				}
			}

			// Act and Assert
			Assert.AreEqual(typeof(Cat).FullName, TestAllowReflectionSetting(true));

			var exception = Assert.Throws<JsRuntimeException>(() => TestAllowReflectionSetting(false));
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("Object doesn't support this property or method", exception.Description);
		}

		#endregion

		#region Delegates

		[Test]
		public override void EmbeddingOfInstanceOfDelegateAndCheckingItsPrototype()
		{ }

		[Test]
		public override void EmbeddingOfInstanceOfDelegateAndGettingItsMethodProperty()
		{
			// Arrange
			string TestAllowReflectionSetting(bool allowReflection)
			{
				var cat = new Cat();
				var cryFunc = new Func<string>(cat.Cry);

				using (var jsEngine = CreateJsEngine(allowReflection: allowReflection))
				{
					jsEngine.EmbedHostObject("cry", cryFunc);
					return jsEngine.Evaluate<string>("cry.Method;");
				}
			}

			// Act and Assert
			Assert.AreEqual("System.String Cry()", TestAllowReflectionSetting(true));
			Assert.AreEqual("undefined", TestAllowReflectionSetting(false));
		}

		#endregion

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
			Assert.AreEqual("TypeError", exception.Type);
			Assert.AreEqual("math.js", exception.DocumentName);
			Assert.AreEqual(10, exception.LineNumber);
			Assert.AreEqual(4, exception.ColumnNumber);
			Assert.IsEmpty(exception.SourceFragment);
			Assert.IsEmpty(exception.CallStack);
		}

		#endregion

		#endregion

		#endregion


		#region Embedding of types

		#region Creating of instances

		[Test]
		public override void CreatingAnInstanceOfEmbeddedCustomExceptionAndCallingOfItsGetTypeMethod()
		{
			// Arrange
			string TestAllowReflectionSetting(bool allowReflection)
			{
				Type loginFailedExceptionType = typeof(LoginFailedException);

				using (var jsEngine = CreateJsEngine(allowReflection: allowReflection))
				{
					jsEngine.EmbedHostType("LoginFailedError", loginFailedExceptionType);
					return jsEngine.Evaluate<string>("new LoginFailedError(\"Wrong password entered!\").GetType();");
				}
			}

			// Act and Assert
			Assert.AreEqual(typeof(LoginFailedException).FullName, TestAllowReflectionSetting(true));

			var exception = Assert.Throws<JsRuntimeException>(() => TestAllowReflectionSetting(false));
			Assert.AreEqual("Runtime error", exception.Category);
			Assert.AreEqual("Object doesn't support this property or method", exception.Description);
		}

		#endregion

		#endregion
	}
}
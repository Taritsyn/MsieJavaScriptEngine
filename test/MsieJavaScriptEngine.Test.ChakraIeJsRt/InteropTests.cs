#if NETCOREAPP
using System;

#endif
using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.ChakraIeJsRt
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.ChakraIeJsRt;
#if NETCOREAPP


		[Test]
		public void EmbeddedInstanceOfDelegateHasFunctionPrototype()
		{
			// Arrange
			var someFunc = new Func<int>(() => 42);

			const string input = "Object.getPrototypeOf(embeddedFunc) === Function.prototype";

			// Act
			bool output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("embeddedFunc", someFunc);
				output = jsEngine.Evaluate<bool>(input);
			}

			// Assert
			Assert.True(output);
		}
#endif
	}
}
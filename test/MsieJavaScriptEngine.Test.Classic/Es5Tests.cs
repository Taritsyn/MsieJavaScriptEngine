using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Classic
{
	[TestFixture]
	public class Es5Tests : Es5TestsBase
	{
		protected override MsieJsEngine CreateJsEngine()
		{
			var jsEngine = new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.Classic,
				UseEcmaScript5Polyfill = true,
				UseJson2Library = true
			});

			return jsEngine;
		}

		#region Object methods

		[Test]
		public override void ObjectKeysMethodIsSupported()
		{
			// Arrange
			const string input1 = "Object.keys(['a', 'b', 'c']).toString();";
			const string targetOutput1 = "0,1,2";

			const string input2 = "Object.keys({ 0: 'a', 1: 'b', 2: 'c' }).toString();";
			const string targetOutput2 = "0,1,2";

			const string input3 = "Object.keys({ 100: 'a', 2: 'b', 7: 'c' }).toString();";
			const string targetOutput3 = "100,2,7";

			const string initCode4 = @"var myObj = function() { };
myObj.prototype = { getFoo: { value: function () { return this.foo } } };;
myObj.foo = 1;
";
			const string input4 = "Object.keys(myObj).toString();";
			const string targetOutput4 = "foo";

			// Act
			string output1;
			string output2;
			string output3;
			string output4;

			using (var jsEngine = CreateJsEngine())
			{
				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<string>(input3);

				jsEngine.Execute(initCode4);
				output4 = jsEngine.Evaluate<string>(input4);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
			Assert.AreEqual(targetOutput4, output4);
		}

		#endregion
	}
}
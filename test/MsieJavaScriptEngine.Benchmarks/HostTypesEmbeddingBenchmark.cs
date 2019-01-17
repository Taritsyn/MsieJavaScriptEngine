using System;
using System.Drawing;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;

using MsieJavaScriptEngine.Benchmarks.Interop.TypesEmbedding;

namespace MsieJavaScriptEngine.Benchmarks
{
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
	public class HostTypesEmbeddingBenchmark
	{
		private static void EmbedAndUseHostTypes(Func<MsieJsEngine> createJsEngine)
		{
			// Arrange
			var someType = typeof(SomeClass);
			var pointType = typeof(Point);
			var someOtherType = typeof(SomeOtherClass);

			const string input = @"(function(SomeClass, Point, SomeOtherClass, undefined) {
	var arg1, arg2, arg3, arg4, interimResult, result;

	SomeClass.Field1 = false;
	SomeClass.Field2 = 678;
	SomeClass.Field3 = 2.20;
	SomeClass.Field4 = 'QWERTY';
	SomeClass.Field5 = new Point(2, 4);

	SomeClass.Property1 = true;
	SomeClass.Property2 = 711;
	SomeClass.Property3 = 5.5;
	SomeClass.Property4 = 'ЙЦУКЕН';
	SomeClass.Property5 = new SomeOtherClass(true, 611, 69.82, 'ASDF',
		false, 555, 79.99, 'ФЫВА');

	arg1 = SomeClass.Field1 || SomeClass.Property1;
	arg2 = SomeClass.Field2 + SomeClass.Property2 + SomeClass.Field5.X;
	arg3 = SomeClass.Field3 + SomeClass.Property3 + SomeClass.Field5.Y;
	arg4 = SomeClass.Field4 + SomeClass.Property4;

	interimResult = SomeClass.DoSomething(arg1, arg2, arg3, arg4);

	arg1 = SomeClass.Property5.Field1 && SomeClass.Property5.Property1;
	arg2 = interimResult - SomeClass.Property5.Field2 - SomeClass.Property5.Property2;
	arg3 = SomeClass.Property5.Field3 / SomeClass.Property5.Property3;
	arg4 = SomeClass.Property5.Field4 + SomeClass.Property5.Property4;

	result = SomeOtherClass.DoSomething(arg1, arg2, arg3, arg4);

	return result;
}(SomeClass, Point, SomeOtherClass));";
			const string targetOutput = "RmFsc2V8MjkyMHwwLjg3Mjg1OTEwNzM4ODQyNHxBU0RG0KTQq9CS0JA=";

			// Act
			string output;

			using (var jsEngine = createJsEngine())
			{
				jsEngine.EmbedHostType("SomeClass", someType);
				jsEngine.EmbedHostType("Point", pointType);
				jsEngine.EmbedHostType("SomeOtherClass", someOtherType);

				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.Equal(targetOutput, output);
		}
#if NET46

		[Benchmark]
		public void Classic()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings{
				EngineMode = JsEngineMode.Classic
			});
			EmbedAndUseHostTypes(createJsEngine);
		}

		[Benchmark]
		public void ChakraActiveScript()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraActiveScript
			});
			EmbedAndUseHostTypes(createJsEngine);
		}
#endif

		[Benchmark]
		public void ChakraIeJsRt()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraIeJsRt
			});
			EmbedAndUseHostTypes(createJsEngine);
		}

		[Benchmark]
		public void ChakraEdgeJsRt()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraEdgeJsRt
			});
			EmbedAndUseHostTypes(createJsEngine);
		}
	}
}
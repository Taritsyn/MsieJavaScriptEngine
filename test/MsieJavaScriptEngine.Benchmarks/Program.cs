using BenchmarkDotNet.Running;

namespace MsieJavaScriptEngine.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<JsExecutionBenchmark>();
		}
	}
}
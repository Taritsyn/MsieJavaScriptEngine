﻿using System.Reflection;

using BenchmarkDotNet.Running;

namespace MsieJavaScriptEngine.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
		}
	}
}
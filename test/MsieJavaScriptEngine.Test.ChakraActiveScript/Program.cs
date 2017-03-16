#if NETCOREAPP1_0 || NET451
using System.Reflection;

using NUnitLite;

namespace MsieJavaScriptEngine.Test.ChakraActiveScript
{
	class Program
	{
		public static int Main(string[] args)
		{
			return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args);
		}
	}
}
#endif
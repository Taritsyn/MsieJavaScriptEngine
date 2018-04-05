using System.Threading;

namespace MsieJavaScriptEngine.Utilities
{
	internal struct InterlockedStatedFlag
	{
		private int _counter;


		public bool IsSet()
		{
			return _counter != 0;
		}

		public bool Set()
		{
			return Interlocked.Exchange(ref _counter, 1) == 0;
		}
	}
}
#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Used to specify the type of thread
	/// </summary>
	internal static class ScriptThreadId
	{
		/// <summary>
		/// The currently executing thread
		/// </summary>
		public const uint Current = 0xFFFFFFFD;

		/// <summary>
		/// The base thread; that is, the thread in which the scripting engine was instantiated
		/// </summary>
		public const uint Base = 0xFFFFFFFE;

		/// <summary>
		/// All threads
		/// </summary>
		public const uint All = 0xFFFFFFFF;
	}
}
#endif
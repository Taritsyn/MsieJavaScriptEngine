#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Indicates what caused the break
	/// </summary>
	internal enum BreakReason
	{
		/// <summary>
		/// The language engine is in the stepping mode
		/// </summary>
		Step,

		/// <summary>
		/// The language engine encountered an explicit breakpoint
		/// </summary>
		Breakpoint,

		/// <summary>
		/// The language engine encountered a debugger block on another thread
		/// </summary>
		DebuggerBlock,

		/// <summary>
		/// The host requested a break
		/// </summary>
		HostInitiated,

		/// <summary>
		/// The language engine requested a break
		/// </summary>
		LanguageInitiated,

		/// <summary>
		/// The debugger IDE requested a break
		/// </summary>
		DebuggerHalt,

		/// <summary>
		/// An execution error caused the break
		/// </summary>
		Error,

		/// <summary>
		/// Caused by JIT Debugging startup
		/// </summary>
		Jit
	}
}
#endif
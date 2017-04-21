#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Describes ways to continue from a breakpoint
	/// </summary>
	internal enum BreakResumeAction
	{
		/// <summary>
		/// Aborts the application
		/// </summary>
		Abort,

		/// <summary>
		/// Continues running
		/// </summary>
		Continue,

		/// <summary>
		/// Steps into a procedure
		/// </summary>
		StepInto,

		/// <summary>
		///  Steps over a procedure
		/// </summary>
		StepOver,

		/// <summary>
		/// Steps out of the current procedure
		/// </summary>
		StepOut,

		/// <summary>
		/// Continues running with state
		/// </summary>
		Ignore,

		/// <summary>
		/// Steps to the next document
		/// </summary>
		StepDocument
	}
}
#endif
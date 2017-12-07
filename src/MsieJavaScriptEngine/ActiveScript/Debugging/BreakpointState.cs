#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Indicates the state of a breakpoint
	/// </summary>
	internal enum BreakpointState
	{
		/// <summary>
		/// The breakpoint no longer exists, but there are still references to it
		/// </summary>
		Deleted,

		/// <summary>
		/// The breakpoint exists but is disabled
		/// </summary>
		Disabled,

		/// <summary>
		/// The breakpoint exists and is enabled
		/// </summary>
		Enabled
	}
}
#endif
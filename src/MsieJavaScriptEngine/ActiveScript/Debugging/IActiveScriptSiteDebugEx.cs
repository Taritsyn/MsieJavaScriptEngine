#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Implement this interface along with the <see cref="IActiveScriptSiteDebug32"/>
	/// or <see cref="IActiveScriptSiteDebug64"/> interface if you are writing a host that needs
	/// to get a notification of a run-time error in an application and optionally attach to
	/// the application for debugging. The Process Debug Manager provides notification through
	/// <see cref="IActiveScriptDebug32"/> or <see cref="IActiveScriptDebug64"/> if a Just-In-Time
	/// script debugger is found on the computer. If no Just-In-Time script debugger is found,
	/// the PDM provides notification through <see cref="IActiveScriptSiteDebugEx"/> instead.
	/// </summary>
	[ComImport]
	[Guid("bb722ccb-6ad2-41c6-b780-af9c03ee69f5")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptSiteDebugEx
	{
		/// <summary>
		/// Informs the host about a script run-time error when the Process Debug Manager does not
		/// find a Just In Time script debugger
		/// </summary>
		/// <param name="errorDebug">The run-time error that occurred</param>
		/// <param name="callOnScriptErrorWhenContinuing">Whether to call
		/// <see cref="IActiveScriptSiteDebug32.OnScriptErrorDebug"/> or
		/// <see cref="IActiveScriptSiteDebug64.OnScriptErrorDebug"/> if the user decides to
		/// continue without debugging</param>
		void OnCanNotJitScriptErrorDebug(
			[In] [MarshalAs(UnmanagedType.Interface)] IActiveScriptErrorDebug errorDebug,
			[Out] [MarshalAs(UnmanagedType.Bool)] out bool callOnScriptErrorWhenContinuing
		);
	}
}
#endif
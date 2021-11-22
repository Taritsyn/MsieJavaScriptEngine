#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// An abstraction that represents a position in executable code
	/// </summary>
	[ComImport]
	[Guid("51973c13-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugCodeContext
	{
		/// <summary>
		/// Returns the document context associated with this code context
		/// </summary>
		/// <param name="context">The document context associated with this code context</param>
		void GetDocumentContext(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugDocumentContext context
		);

		/// <summary>
		/// Sets or clears a breakpoint at this code context
		/// </summary>
		/// <param name="state">Specifies the breakpoint state for this code context</param>
		void SetBreakPoint(
			[In] BreakpointState state
		);
	}
}
#endif
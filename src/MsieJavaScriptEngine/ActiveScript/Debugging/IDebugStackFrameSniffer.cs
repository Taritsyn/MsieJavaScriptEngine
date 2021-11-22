#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides a way to enumerate the logical stack frames known by a component. Script engines typically
	/// implement this interface. The process debug manager uses this interface to find all stack frames
	/// associated with a given thread.
	/// </summary>
	[ComImport]
	[Guid("51973c18-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugStackFrameSniffer
	{
		/// <summary>
		/// Returns an enumerator of stack frames for the current thread
		/// </summary>
		/// <param name="enumFrames">Enumerator of stack frames for the current thread</param>
		void EnumStackFrames(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugStackFrames enumFrames
		);
	}
}
#endif
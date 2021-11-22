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
	[Guid("8cd12af4-49c1-4d52-8d8a-c146f47581aa")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugStackFrameSnifferEx64 // : IDebugStackFrameSniffer
	{
		#region IDebugStackFrameSniffer methods

		/// <summary>
		/// Returns an enumerator of stack frames for the current thread
		/// </summary>
		/// <param name="enumFrames">Enumerator of stack frames for the current thread</param>
		void EnumStackFrames(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugStackFrames enumFrames
		);

		#endregion

		void EnumStackFramesEx64(
			[In] ulong minimum,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugStackFrames enumFrames
		);
	}
}
#endif
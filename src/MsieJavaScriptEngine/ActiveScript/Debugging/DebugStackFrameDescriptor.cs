#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Enumerates stack frames and merges output from several enumerators on the same thread
	/// </summary>
	/// <remarks>The process debug manager uses this structure to sort the stack frames from
	/// multiple script engines. By convention, stacks grow down. Consequently, on architectures
	/// where stacks grow up, the addresses should be twos-complemented.</remarks>
	[StructLayout(LayoutKind.Sequential)]
	internal struct DebugStackFrameDescriptor
	{
		/// <summary>
		/// The stack frame object
		/// </summary>
		[MarshalAs(UnmanagedType.Interface)]
		public IDebugStackFrame Frame;

		/// <summary>
		/// A machine-dependent representation of the lower range of physical addresses
		/// associated with this stack frame
		/// </summary>
		public uint Minimum;

		/// <summary>
		/// A machine-dependent representation of the upper range of physical addresses
		/// associated with this stack frame
		/// </summary>
		public uint Limit;

		/// <summary>
		/// Flag that indicates that the frame is being processed
		/// </summary>
		[MarshalAs(UnmanagedType.Bool)]
		public bool IsFinal;

		/// <summary>
		/// If this parameter is not null, the current enumerator merging should stop and
		/// a new one should be started. The object indicates how to start the new enumeration.
		/// </summary>
		public IntPtr pFinalObject;
	}
}
#endif
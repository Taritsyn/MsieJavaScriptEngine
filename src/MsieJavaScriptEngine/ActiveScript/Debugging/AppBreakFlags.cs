#if NETFRAMEWORK
using System;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Indicate the current debug state for applications and threads
	/// </summary>
	[Flags]
	internal enum AppBreakFlags : uint
	{
		None = 0,

		/// <summary>
		/// Language engine should break immediately on all threads with
		/// <see cref="BreakReason.DebuggerBlock"/>
		/// </summary>
		DebuggerBlock = 0x00000001,

		/// <summary>
		/// Language engine should break immediately with <see cref="BreakReason.DebuggerHalt"/>
		/// </summary>
		DebuggerHalt = 0x00000002,

		/// <summary>
		/// Language engine should break immediately in the stepping thread with
		/// <see cref="BreakReason.Step"/>
		/// </summary>
		Step = 0x00010000,

		/// <summary>
		/// The application is in nested execution on a breakpoint
		/// </summary>
		Nested = 0x00020000,

		/// <summary>
		/// The debugger is stepping at the source level
		/// </summary>
		StepTypeSource = 0x00000000,

		/// <summary>
		/// The debugger is stepping at the byte code level
		/// </summary>
		StepTypeByteCode = 0x00100000,

		/// <summary>
		/// The debugger is stepping at the machine level
		/// </summary>
		StepTypeMachine = 0x00200000,

		/// <summary>
		/// Mask for factoring out the step types
		/// </summary>
		StepTypeMask = 0x00F00000,

		/// <summary>
		/// A breakpoint is in progress
		/// </summary>
		InBreakpoint = 0x80000000
	}
}
#endif
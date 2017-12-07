#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Represents a logical stack frame on the thread stack
	/// </summary>
	[ComImport]
	[Guid("51973c17-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugStackFrame
	{
		/// <summary>
		/// Returns the current code context associated with the stack frame
		/// </summary>
		/// <param name="context">The code context associated with the stack frame</param>
		void GetCodeContext(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugCodeContext context
		);

		/// <summary>
		/// Returns a short or long textual description of the stack frame
		/// </summary>
		/// <param name="longString">Flag, where <code>true</code> returns a long description and
		/// <code>false</code> returns a short description</param>
		/// <param name="description">The description of the stack frame</param>
		void GetDescriptionString(
			[In] [MarshalAs(UnmanagedType.Bool)] bool longString,
			[Out] [MarshalAs(UnmanagedType.BStr)] out string description
		);

		/// <summary>
		/// Returns a short or long textual description of the language
		/// </summary>
		/// <param name="longString">Flag, where <code>true</code> returns a long description and
		/// <code>false</code> returns a short description</param>
		/// <param name="language">The description of the language</param>
		void GetLanguageString(
			[In] [MarshalAs(UnmanagedType.Bool)] bool longString,
			[Out] [MarshalAs(UnmanagedType.BStr)] out string language
		);

		/// <summary>
		/// Returns the thread associated with this stack frame
		/// </summary>
		/// <param name="thread">The thread associated with this stack frame</param>
		void GetThread(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationThread thread
		);

		/// <summary>
		/// Returns a property browser for the current frame
		/// </summary>
		/// <param name="property">A property browser for the current frame</param>
		void GetDebugProperty(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugProperty property
		);
	}
}
#endif
#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Smart hosts implement the <see cref="IActiveScriptSiteDebug64"/> interface to perform document management
	/// and to participate in debugging
	/// </summary>
	[ComImport]
	[Guid("d6b96b0a-7463-402c-92ac-89984226942f")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptSiteDebug64
	{
		/// <summary>
		/// Used by the language engine to delegate IDebugCodeContext.GetSourceContext
		/// </summary>
		/// <param name="sourceContext">The source context as provided to
		/// <see cref="IActiveScriptParse64.ParseScriptText"/> or
		/// <see cref="IActiveScriptParse64.AddScriptlet"/></param>
		/// <param name="offset">Character offset relative to start of script block or scriptlet</param>
		/// <param name="length">Number of characters in this context</param>
		/// <param name="documentContext">The document context corresponding to this character-position
		/// range</param>
		void GetDocumentContextFromPosition(
			[In] ulong sourceContext,
			[In] uint offset,
			[In] uint length,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugDocumentContext documentContext
		);

		/// <summary>
		/// Returns the debug application object associated with this script site
		/// </summary>
		/// <param name="application">The debug application object associated with the script site</param>
		void GetApplication(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplication64 application
		);

		/// <summary>
		/// Gets the application node under which script documents should be added
		/// </summary>
		/// <param name="node">The debug application node that holds script documents</param>
		void GetRootApplicationNode(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationNode node
		);

		/// <summary>
		/// Allows a smart host to determine how to handle run-time errors
		/// </summary>
		/// <param name="errorDebug">The run-time error that occurred</param>
		/// <param name="enterDebugger">Flag indicating whether to pass the error to the debugger to
		/// do JIT debugging</param>
		/// <param name="callOnScriptErrorWhenContinuing">Flag indicating whether to call
		/// <see cref="IActiveScriptSite.OnScriptError"/> when the user decides to continue without
		/// debugging</param>
		void OnScriptErrorDebug(
			[In] [MarshalAs(UnmanagedType.Interface)] IActiveScriptErrorDebug errorDebug,
			[Out] [MarshalAs(UnmanagedType.Bool)] out bool enterDebugger,
			[Out] [MarshalAs(UnmanagedType.Bool)] out bool callOnScriptErrorWhenContinuing
		);
	}
}
#endif
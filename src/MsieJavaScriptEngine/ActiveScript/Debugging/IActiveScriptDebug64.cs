#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Implemented by script engines that support debugging
	/// </summary>
	[ComImport]
	[Guid("bc437e23-f5b8-47f4-bb79-7d1ce5483b86")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptDebug64
	{
		/// <summary>
		/// Returns the text attributes for an arbitrary block of script text
		/// </summary>
		/// <param name="code">The script block text</param>
		/// <param name="length">The number of characters in the script block text</param>
		/// <param name="delimiter">End of script block delimiter</param>
		/// <param name="flags">Flags associated with the script block</param>
		/// <param name="pAttrs">Buffer to contain the returned attributes</param>
		void GetScriptTextAttributes(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string code,
			[In] uint length,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string delimiter,
			[In] ScriptTextFlags flags,
			[In] [Out] ref IntPtr pAttrs
		);

		/// <summary>
		/// Returns the text attributes for an arbitrary scriptlet
		/// </summary>
		/// <param name="code">The scriptlet text</param>
		/// <param name="length">The number of characters in the scriptlet text</param>
		/// <param name="delimiter">End of scriptlet delimiter</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <param name="pAttrs">Buffer to contain the returned attributes</param>
		void GetScriptletTextAttributes(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string code,
			[In] uint length,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string delimiter,
			[In] ScriptTextFlags flags,
			[In] [Out] ref IntPtr pAttrs
		);

		/// <summary>
		/// Used by a smart host to delegate the <see cref="IDebugDocumentContext.EnumCodeContexts"/> method
		/// </summary>
		/// <param name="sourceContext">The source context as provided to
		/// <see cref="IActiveScriptParse64.ParseScriptText"/> or
		/// <see cref="IActiveScriptParse64.AddScriptlet"/></param>
		/// <param name="offset">Character offset relative to start of script text</param>
		/// <param name="length">Number of characters in this context</param>
		/// <param name="enumContexts">An enumerator of the code contexts in the specified range</param>
		void EnumCodeContextsOfPosition(
			[In] ulong sourceContext,
			[In] uint offset,
			[In] uint length,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
		);
	}
}
#endif
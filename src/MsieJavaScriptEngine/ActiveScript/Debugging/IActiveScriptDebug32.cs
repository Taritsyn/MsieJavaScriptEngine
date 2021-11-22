#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Implemented by script engines that support debugging
	/// </summary>
	[ComImport]
	[Guid("51973c10-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptDebug32
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
		/// <see cref="IActiveScriptParse32.ParseScriptText"/> or
		/// <see cref="IActiveScriptParse32.AddScriptlet"/></param>
		/// <param name="offset">Character offset relative to start of script text</param>
		/// <param name="length">Number of characters in this context</param>
		/// <param name="enumContexts">An enumerator of the code contexts in the specified range</param>
		/// <returns>The method returns an HRESULT</returns>
		[PreserveSig]
		uint EnumCodeContextsOfPosition(
			[In] uint sourceContext,
			[In] uint offset,
			[In] uint length,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
		);
	}
}
#endif
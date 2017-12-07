#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides information on a document, which may or may not be instantiated
	/// </summary>
	[ComImport]
	[Guid("51973c1f-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentInfo
	{
		/// <summary>
		/// Returns the specified document name
		/// </summary>
		/// <param name="type">The type of document name to return</param>
		/// <param name="name">String containing the name</param>
		void GetName(
			[In] DocumentNameType type,
			[Out] [MarshalAs(UnmanagedType.BStr)] out string name
		);

		/// <summary>
		/// Returns a CLSID identifying the document type
		/// </summary>
		/// <param name="clsid">A CLSID identifying the document type</param>
		void GetDocumentClassId(
			[Out] out Guid clsid
		);
	}
}
#endif
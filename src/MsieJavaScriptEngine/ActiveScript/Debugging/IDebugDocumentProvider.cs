#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides the means for instantiating a document on demand
	/// </summary>
	[ComImport]
	[Guid("51973c20-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentProvider // : IDebugDocumentInfo
	{
		#region IDebugDocumentInfo methods

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

		#endregion

		/// <summary>
		/// Causes the document to be instantiated if it does not already exist
		/// </summary>
		/// <param name="document">The debug document corresponding to the document</param>
		void GetDocument(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugDocument document
		);
	}
}
#endif
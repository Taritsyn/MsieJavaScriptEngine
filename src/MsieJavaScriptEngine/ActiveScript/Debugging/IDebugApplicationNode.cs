#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// The <see cref="IDebugApplicationNode"/> interface extends the functionality of
	/// the <see cref="IDebugDocumentProvider"/> interface by providing a context within a project tree
	/// </summary>
	[ComImport]
	[Guid("51973c34-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugApplicationNode // : IDebugDocumentProvider
	{
		#region IDebugDocumentProvider methods

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

		#endregion

		/// <summary>
		/// Enumerates the child nodes of this application node
		/// </summary>
		/// <param name="enumNodes">The enumeration of this node's child nodes</param>
		void EnumChildren(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugApplicationNodes enumNodes
		);

		/// <summary>
		/// Returns the parent node of this application node
		/// </summary>
		/// <param name="node">Parent application node of this application node</param>
		void GetParent(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationNode node
		);

		/// <summary>
		/// Sets the document provider for this application node
		/// </summary>
		/// <param name="provider">The document provider for this application node</param>
		void SetDocumentProvider(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugDocumentProvider provider
		);

		/// <summary>
		/// Causes this application to release all references and enter an inactive state
		/// </summary>
		void Close();

		/// <summary>
		/// Adds this application node to the specified project tree
		/// </summary>
		/// <param name="node">The project tree where this application node is to be added</param>
		void Attach(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugApplicationNode node
		);

		/// <summary>
		/// Removes this application node from the project tree
		/// </summary>
		void Detach();
	}
}
#endif
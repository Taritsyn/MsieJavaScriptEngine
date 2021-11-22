#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides an abstract representation of a portion of the document being debugged.
	/// For text documents, this representation consists of a character-position range
	/// </summary>
	[ComImport]
	[Guid("51973c28-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentContext
	{
		/// <summary>
		/// Returns the document that contains this context
		/// </summary>
		/// <param name="document">The document that contains this context</param>
		void GetDocument(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugDocument document
		);

		/// <summary>
		/// Enumerates the code contexts associated with this document context
		/// </summary>
		/// <param name="enumContexts">The code contexts associated with this document context</param>
		void EnumCodeContexts(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
		);
	}
}
#endif
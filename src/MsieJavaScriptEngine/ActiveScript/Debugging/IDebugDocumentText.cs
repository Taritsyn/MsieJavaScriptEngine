#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides access to a text-only version of the debug document
	/// </summary>
	[ComImport]
	[Guid("51973c22-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentText // : IDebugDocument
	{
		#region IDebugDocument methods

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

		#endregion

		/// <summary>
		/// Returns the attributes of the document
		/// </summary>
		/// <param name="attrs">The text attributes of the document</param>
		void GetDocumentAttributes(
			[Out] out TextDocAttrs attrs
		);

		/// <summary>
		/// Returns the number of lines and number of characters in the document
		/// </summary>
		/// <param name="numLines">Number of lines in the document. If this parameter is null,
		/// the method does not return a value</param>
		/// <param name="length">Number of characters in the document. If this parameter is null,
		/// the method does not return a value</param>
		void GetSize(
			[Out] out uint numLines,
			[Out] out uint length
		);

		/// <summary>
		/// Returns the character-position corresponding to the first character of a line
		/// </summary>
		/// <param name="lineNumber">The line number</param>
		/// <param name="position">The character position within the document of the start of
		/// line <paramref name="lineNumber"/></param>
		void GetPositionOfLine(
			[In] uint lineNumber,
			[Out] out uint position
		);

		/// <summary>
		/// Returns the line number and, optionally, the character offset within the line that corresponds
		/// to the given character-position
		/// </summary>
		/// <param name="position">Start location of the character position range</param>
		/// <param name="lineNumber">The line number of the range</param>
		/// <param name="offsetInLine">The character offset of the range within line <paramref name="lineNumber"/>.
		/// If this parameter is null, the method does not return a value</param>
		void GetLineOfPosition(
			[In] uint position,
			[Out] out uint lineNumber,
			[Out] out uint offsetInLine
		);

		/// <summary>
		/// Retrieves the characters and/or the character attributes associated with a character-position range
		/// </summary>
		/// <param name="position">Start location of the character position range</param>
		/// <param name="pChars">A character text buffer. The buffer must be large enough to hold
		/// <paramref name="maxChars"/> characters. If this parameter is null, the method does not return characters.</param>
		/// <param name="pAttrs">A character attribute buffer. The buffer must be large enough to hold
		/// <paramref name="maxChars"/> characters. If this parameter is null, the method does not return attributes.</param>
		/// <param name="length">The number of characters/attributes returned. This parameter must be set to
		/// zero before calling this method.</param>
		/// <param name="maxChars">Number of characters in the character position range. Also specifies
		/// the maximum number of characters to return.</param>
		void GetText(
			[In] uint position,
			[In] IntPtr pChars,
			[In] IntPtr pAttrs,
			[In] [Out] ref uint length,
			[In] uint maxChars
		);

		/// <summary>
		/// Returns the character-position range corresponding to a document context
		/// </summary>
		/// <param name="context">The document context object</param>
		/// <param name="position">Start location of the character position range</param>
		/// <param name="length">Number of characters in the range</param>
		void GetPositionOfContext(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugDocumentContext context,
			[Out] out uint position,
			[Out] out uint length
		);

		/// <summary>
		/// Creates a document context object corresponding to the provided character position range
		/// </summary>
		/// <param name="position">Start location of the character position range</param>
		/// <param name="length">Number of characters in the range</param>
		/// <param name="context">The document context object corresponding to the specified character
		/// position range</param>
		void GetContextOfPosition(
			[In] uint position,
			[In] uint length,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugDocumentContext context
		);
	}
}
#endif
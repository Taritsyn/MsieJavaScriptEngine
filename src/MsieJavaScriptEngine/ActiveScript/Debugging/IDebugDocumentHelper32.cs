using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provide implementations for many interfaces necessary for smart hosting, such as
	/// the IDebugDocument, IDebugDocumentContext, IDebugDocumentProvider, IDebugDocumentText
	/// and IDebugDocumentTextEvents interfaces
	/// </summary>
	[ComImport]
	[Guid("51973C26-CB0C-11d0-B5C9-00A0244A0E7A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentHelper32
	{ }
}
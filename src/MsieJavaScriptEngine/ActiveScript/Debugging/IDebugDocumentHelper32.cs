using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provide implementations for many interfaces necessary for smart hosting, such as
	/// the <c>IDebugDocument</c>, <c>IDebugDocumentContext</c>, <c>IDebugDocumentProvider</c>,
	/// <c>IDebugDocumentText</c> and <c>IDebugDocumentTextEvents</c> interfaces
	/// </summary>
	[ComImport]
	[Guid("51973C26-CB0C-11d0-B5C9-00A0244A0E7A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentHelper32
	{ }
}
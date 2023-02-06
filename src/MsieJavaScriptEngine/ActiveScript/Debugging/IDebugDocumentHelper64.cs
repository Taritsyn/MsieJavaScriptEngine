using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provide implementations for many interfaces necessary for smart hosting, such as
	/// the <c>IDebugDocument</c>, <c>IDebugDocumentContext</c>, <c>IDebugDocumentProvider</c>,
	/// <c>IDebugDocumentText</c> and <c>IDebugDocumentTextEvents</c> interfaces
	/// </summary>
	[ComImport]
	[Guid("c4c7363c-20fd-47f9-bd82-4855e0150871")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugDocumentHelper64
	{ }
}
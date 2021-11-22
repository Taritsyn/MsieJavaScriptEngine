#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// The Process Debug Manager implements the <see cref="IDebugAsyncOperation"/> interface.
	/// A language engine calls the IDebugApplication.CreateAsyncDebugOperation method
	/// to obtain a reference to this interface. The language engine can use
	/// the <see cref="IDebugAsyncOperation"/> interface to provide asynchronous access to a synchronous
	/// debug operation.
	/// </summary>
	[ComImport]
	[Guid("51973c1b-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugAsyncOperation
	{ }
}
#endif
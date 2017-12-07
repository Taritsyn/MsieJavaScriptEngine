#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Allows a script engine to abstract an operation (such as expression evaluation) that needs to be
	/// performed while nested in a particular blocked thread. The interface also provides a mechanism for
	/// canceling unresponsive operations.
	/// </summary>
	[ComImport]
	[Guid("51973c1a-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugSyncOperation
	{ }
}
#endif
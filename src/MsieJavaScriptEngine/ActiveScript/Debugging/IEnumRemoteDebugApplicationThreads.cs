#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Enumerates the running threads in an application
	/// </summary>
	[ComImport]
	[Guid("51973c3c-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumRemoteDebugApplicationThreads
	{ }
}
#endif
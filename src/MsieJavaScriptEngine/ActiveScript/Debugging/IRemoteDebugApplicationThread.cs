#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Represents a thread of execution within a particular application
	/// </summary>
	[ComImport]
	[Guid("51973c37-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IRemoteDebugApplicationThread
	{ }
}
#endif
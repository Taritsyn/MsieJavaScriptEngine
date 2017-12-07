#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Allows language engines and hosts to provide thread synchronization and to maintain
	/// thread-specific debug state information. This interface extends
	/// the <see cref="IRemoteDebugApplicationThread"/> interface to provide non-remote access to the thread.
	/// </summary>
	[ComImport]
	[Guid("51973c38-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugApplicationThread // : IRemoteDebugApplicationThread
	{ }
}
#endif
#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// The primary interface exposed by a debugger
	/// </summary>
	[ComImport]
	[Guid("51973c2a-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IApplicationDebugger
	{ }
}
#endif
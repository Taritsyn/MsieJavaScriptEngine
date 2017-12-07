#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Enumerates child nodes of a node associated with an application
	/// </summary>
	[ComImport]
	[Guid("51973c3a-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumDebugApplicationNodes
	{ }
}
#endif
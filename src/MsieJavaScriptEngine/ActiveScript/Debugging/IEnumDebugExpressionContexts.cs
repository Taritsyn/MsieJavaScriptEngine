#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Enumerates a collection of IDebugExpressionContexts objects
	/// </summary>
	[ComImport]
	[Guid("51973c40-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumDebugExpressionContexts
	{ }
}
#endif
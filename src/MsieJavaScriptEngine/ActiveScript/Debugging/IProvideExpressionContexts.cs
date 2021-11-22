#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides a way to enumerate expression contexts known by a certain component
	/// </summary>
	[ComImport]
	[Guid("51973c41-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IProvideExpressionContexts
	{ }
}
#endif
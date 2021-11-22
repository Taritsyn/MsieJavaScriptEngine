#if NETFRAMEWORK
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// The <see cref="IDebugThreadCall64"/> interface is typically implemented by a component that makes
	/// cross-thread calls with the IDebugThread marshalling implementation provided by
	/// the process debug manager (PDM).
	/// </summary>
	[ComImport]
	[Guid("cb3fa335-e979-42fd-9fcf-a7546a0f3905")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugThreadCall64
	{ }
}
#endif
#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// The <see cref="IDebugThreadCall32"/> interface is typically implemented by a component that makes
	/// cross-thread calls with the IDebugThread marshalling implementation provided by
	/// the process debug manager (PDM).
	/// </summary>
	[ComImport]
	[Guid("51973c36-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugThreadCall32
	{ }
}
#endif
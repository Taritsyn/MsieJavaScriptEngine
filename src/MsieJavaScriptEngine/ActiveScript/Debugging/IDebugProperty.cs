#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Used to describe any hierarchical property of the entity being debugged that has a name, type, and value.
	/// Most commonly, <see cref="IDebugProperty"/> is used to describe the result of expression evaluation,
	/// statement evaluation, or register evaluation.
	/// </summary>
	[ComImport]
	[Guid("51973c50-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugProperty
	{ }
}
#endif
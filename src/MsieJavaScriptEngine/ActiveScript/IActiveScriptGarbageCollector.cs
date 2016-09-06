#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Provides a method to start garbage collection.
	/// This interface should be implemented by Active Script engines that want to clean up their resources.
	/// </summary>
	[ComImport]
	[Guid("6aa2c4a0-2b53-11d4-a2a0-00104bd35090")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptGarbageCollector
	{
		/// <summary>
		/// Starts a garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		void CollectGarbage([In] ScriptGCType type);
	}
}
#endif
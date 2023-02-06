#if NETFRAMEWORK
using System;

namespace MsieJavaScriptEngine.ActiveScript
{
	[Flags]
	internal enum ScriptInfoFlags : uint
	{
		/// <summary>
		/// Not a valid option
		/// </summary>
		None = 0,

		/// <summary>
		/// Returns the <c>IUnknown</c> interface for this item
		/// </summary>
		IUnknown = 1,

		/// <summary>
		/// Returns the <c>ITypeInfo</c> interface for this item
		/// </summary>
		ITypeInfo = 2
	}
}
#endif
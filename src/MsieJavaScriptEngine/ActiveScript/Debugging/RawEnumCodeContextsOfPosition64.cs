#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	internal delegate uint RawEnumCodeContextsOfPosition64(
		[In] IntPtr pThis,
		[In] ulong sourceContext,
		[In] uint offset,
		[In] uint length,
		[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
	);
}
#endif
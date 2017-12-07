#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	internal delegate uint RawEnumCodeContextsOfPosition32(
		[In] IntPtr pThis,
		[In] uint sourceContext,
		[In] uint offset,
		[In] uint length,
		[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
	);
}
#endif
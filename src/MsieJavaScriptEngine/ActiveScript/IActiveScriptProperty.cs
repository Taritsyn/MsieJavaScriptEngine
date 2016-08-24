using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript
{
	[ComImport]
	[Guid("4954e0d0-fbc7-11d1-8410-006008c3fbfc")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptProperty
	{
		[PreserveSig]
		uint GetProperty(
			[In] uint dwProperty,
			[In] IntPtr pvarIndex,
			[Out] out object pvarValue);

		[PreserveSig]
		uint SetProperty(
			[In] uint dwProperty,
			[In] IntPtr pvarIndex,
			[In] [Out] ref object pvarValue);
	}
}
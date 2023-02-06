#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// The <see cref="IActiveScriptProperty"/> interface is used to get and set
	/// configuration properties
	/// </summary>
	[ComImport]
	[Guid("4954e0d0-fbc7-11d1-8410-006008c3fbfc")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptProperty
	{
		/// <summary>
		/// Gets the property that is specified by the parameter
		/// </summary>
		/// <param name="dwProperty">The property value to get</param>
		/// <param name="pvarIndex">Not used</param>
		/// <param name="pvarValue">The value of the property</param>
		/// <returns>The method returns an <c>HRESULT</c></returns>
		[PreserveSig]
		uint GetProperty(
			[In] uint dwProperty,
			[In] IntPtr pvarIndex,
			[Out] out object pvarValue
		);

		/// <summary>
		/// Sets the property that is specified by the parameter
		/// </summary>
		/// <param name="dwProperty">The property value to set</param>
		/// <param name="pvarIndex">Not used</param>
		/// <param name="pvarValue">The value of the property</param>
		/// <returns>The method returns an <c>HRESULT</c></returns>
		[PreserveSig]
		uint SetProperty(
			[In] uint dwProperty,
			[In] IntPtr pvarIndex,
			[In] [Out] ref object pvarValue
		);
	}
}
#endif
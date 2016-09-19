using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” function callback
	/// </summary>
	/// <param name="callee">The <c>Function</c> object that represents the function being invoked</param>
	/// <param name="isConstructCall">Indicates whether this is a regular call or a 'new' call</param>
	/// <param name="arguments">The arguments to the call</param>
	/// <param name="argumentCount">The number of arguments</param>
	/// <param name="callbackData">Callback data, if any</param>
	/// <returns>The result of the call, if any</returns>
	internal delegate EdgeJsValue EdgeJsNativeFunction(EdgeJsValue callee, bool isConstructCall, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] EdgeJsValue[] arguments, ushort argumentCount, IntPtr callbackData);
}
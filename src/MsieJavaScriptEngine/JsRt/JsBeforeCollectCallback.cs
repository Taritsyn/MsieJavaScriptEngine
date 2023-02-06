using System;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// The callback called before collection
	/// </summary>
	/// <param name="callbackState">The state passed to <c>SetBeforeCollectCallback</c></param>
	internal delegate void JsBeforeCollectCallback(IntPtr callbackState);
}
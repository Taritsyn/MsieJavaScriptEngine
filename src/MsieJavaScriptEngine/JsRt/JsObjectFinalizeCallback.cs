using System;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// The finalization callback
	/// </summary>
	/// <param name="data">The external data that was passed in when creating the object being finalized</param>
	internal delegate void JsObjectFinalizeCallback(IntPtr data);
}
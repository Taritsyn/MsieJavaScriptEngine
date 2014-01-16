namespace MsieJavaScriptEngine.JsRt
{
	using System;

	/// <summary>
	/// A finalization callback.
	/// </summary>
	/// <param name="data">
	/// The external data that was passed in when creating the object being finalized.
	/// </param>
	internal delegate void JavaScriptObjectFinalizeCallback(IntPtr data);
}
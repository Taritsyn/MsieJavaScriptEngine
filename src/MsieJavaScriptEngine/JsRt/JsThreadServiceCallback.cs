﻿using System;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// The thread service callback
	/// </summary>
	/// <remarks>
	/// The host can specify a background thread service when creating a runtime. If
	/// specified, then background work items will be passed to the host using this callback. The
	/// host is expected to either begin executing the background work item immediately and return
	/// <c>true</c> or return <c>false</c> and the runtime will handle the work item in-thread.
	/// </remarks>
	/// <param name="callbackFunction">The callback for the background work item</param>
	/// <param name="callbackData">The data argument to be passed to the callback</param>
	/// <returns>Whether the thread service will execute the callback</returns>
	internal delegate bool JsThreadServiceCallback(JsBackgroundWorkItemCallback callbackFunction, IntPtr callbackData);
}
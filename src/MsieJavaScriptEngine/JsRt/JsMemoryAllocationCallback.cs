﻿using System;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// User implemented callback routine for memory allocation events
	/// </summary>
	/// <param name="callbackState">The state passed to <c>SetRuntimeMemoryAllocationCallback</c></param>
	/// <param name="allocationEvent">The type of type allocation event</param>
	/// <param name="allocationSize">The size of the allocation</param>
	/// <returns>
	/// For the Allocate event, returning <c>true</c> allows the runtime to continue with
	/// allocation. Returning <c>false</c> indicates the allocation request is rejected. The return value
	/// is ignored for other allocation events.
	/// </returns>
	internal delegate bool JsMemoryAllocationCallback(IntPtr callbackState, JsMemoryEventType allocationEvent, UIntPtr allocationSize);
}
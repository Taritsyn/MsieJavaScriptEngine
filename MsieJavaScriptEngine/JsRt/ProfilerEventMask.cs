namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// Event mask for profiling
	/// </summary>
	internal enum ProfilerEventMask
	{
		/// <summary>
		/// Trace calls to script functions
		/// </summary>
		TraceScriptFunctionCall = 0x1,

		/// <summary>
		/// Trace calls to built-in functions
		/// </summary>
		TraceNativeFunctionCall = 0x2,

		/// <summary>
		/// Trace calls to DOM methods
		/// </summary>
		TraceDomFunctionCall = 0x4,

		/// <summary>
		/// Trace all calls except DOM methods
		/// </summary>
		TraceAll = (TraceScriptFunctionCall | TraceNativeFunctionCall),

		/// <summary>
		/// Trace all calls
		/// </summary>
		TraceAllWithDom = (TraceAll | TraceDomFunctionCall)
	}
}
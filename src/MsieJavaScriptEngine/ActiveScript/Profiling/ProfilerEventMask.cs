namespace MsieJavaScriptEngine.ActiveScript.Profiling
{
	/// <summary>
	/// Indicates the types of events that should be profiled
	/// </summary>
	internal enum ProfilerEventMask : uint
	{
		/// <summary>
		/// Profiles functions that are defined in user-written script and dynamic code
		/// </summary>
		TraceScriptFunctionCall = 0x00000001,

		/// <summary>
		/// Profiles native functions that are defined by the scripting engine
		/// </summary>
		TraceNativeFunctionCall = 0x00000002,

		/// <summary>
		/// Profiles functions that call into the DOM
		/// </summary>
		TraceDomFunctionCall = 0x00000004,

		/// <summary>
		/// Profiles all user-defined and scripting engine functions, excluding calls into
		/// the Document Object Model (DOM)
		/// </summary>
		TraceAll = TraceScriptFunctionCall | TraceNativeFunctionCall,

		/// <summary>
		/// Profiles all functions, including calls into the DOM
		/// </summary>
		TraceAllWithDom = TraceAll | TraceDomFunctionCall
	}
}
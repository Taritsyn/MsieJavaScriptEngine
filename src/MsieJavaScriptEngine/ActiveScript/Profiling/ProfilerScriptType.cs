namespace MsieJavaScriptEngine.ActiveScript.Profiling
{
	/// <summary>
	/// Specifies the type of script
	/// </summary>
	internal enum ProfilerScriptType : uint
	{
		/// <summary>
		/// Specifies user-written script code
		/// </summary>
		User,

		/// <summary>
		/// Specifies script code that is generated dynamically during execution
		/// </summary>
		Dynamic,

		/// <summary>
		/// Specifies the script type for native functions and objects that are defined by
		/// the scripting engine
		/// </summary>
		Native,

		/// <summary>
		/// Specifies a call into the Document Object Model (DOM) of Internet Explorer
		/// (for example, a call to the <code>document.getElementById</code> method)
		/// </summary>
		Dom
	}
}
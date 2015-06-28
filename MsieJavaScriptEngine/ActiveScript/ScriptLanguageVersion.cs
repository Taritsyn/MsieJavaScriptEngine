namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Version of script language (see https://msdn.microsoft.com/en-gb/library/hh769820(v=vs.94).aspx)
	/// </summary>
	internal enum ScriptLanguageVersion
	{
		/// <summary>
		/// The default version
		/// </summary>
		Default = 0,

		/// <summary>
		/// Windows Scripting version 5.7
		/// </summary>
		WindowsScripting57 = 1,

		/// <summary>
		/// Windows Scripting version 5.8
		/// </summary>
		WindowsScripting58 = 2,

		/// <summary>
		/// ECMAScript 5
		/// </summary>
		EcmaScript5 = 3,
	}
}
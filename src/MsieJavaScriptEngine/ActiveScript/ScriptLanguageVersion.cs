#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Specifies the possible scripting versions
	/// </summary>
	internal enum ScriptLanguageVersion
	{
		/// <summary>
		/// Version not selected
		/// </summary>
		None = -1,

		/// <summary>
		/// Default version
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
		EcmaScript5 = 3
	}
}
#endif
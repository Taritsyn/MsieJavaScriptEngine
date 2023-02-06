#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Script property (see https://msdn.microsoft.com/en-us/subscriptions/downloads/cc512774(v=vs.94).aspx)
	/// </summary>
	internal enum ScriptProperty : uint
	{
		/// <summary>
		/// Forces the scripting engine to divide in integer mode instead of floating point mode.
		/// The default value is <c>false</c>.
		/// </summary>
		IntegerMode = 0x00003000,

		/// <summary>
		/// Allows the string compare function of the scripting engine to be replaced
		/// </summary>
		StringCompareInstance = 0x00003001,

		/// <summary>
		/// Informs the scripting engine that no other scripting engines exist to contribute
		/// to the global object
		/// </summary>
		AbbreviateGlobalNameResolution = 0x70000002,

		/// <summary>
		/// Forces the JavaScript scripting engine to select a set of language features to be supported.
		/// The default set of language features supported by the JavaScript scripting engine is equivalent
		/// to the language feature set that appeared in version 5.7 of the JavaScript scripting engine.
		/// </summary>
		InvokeVersioning = 0x00004000
	}
}
#endif
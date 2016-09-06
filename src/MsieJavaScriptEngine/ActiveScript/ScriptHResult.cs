#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Common HRESULT Values
	/// </summary>
	internal enum ScriptHResult : uint
	{
		/// <summary>
		/// Success
		/// </summary>
		Ok = 0x00000000,

		/// <summary>
		/// An argument is not valid
		/// </summary>
		InvalidArg = 0x80070057,

		/// <summary>
		/// The call was not expected (for example, the scripting engine
		/// has not yet been loaded or initialized)
		/// </summary>
		Unexpected = 0x8000FFFF
	}
}
#endif
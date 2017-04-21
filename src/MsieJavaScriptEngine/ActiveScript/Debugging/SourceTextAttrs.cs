#if !NETSTANDARD1_3
using System;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Describe the attributes of a single character of source text
	/// </summary>
	[Flags]
	internal enum SourceTextAttrs : ushort
	{
		None = 0,

		/// <summary>
		/// The character is part of a language keyword
		/// (for example, the JavaScript keyword <code>while</code>)
		/// </summary>
		Keyword = 0x0001,

		/// <summary>
		/// The character is part of a comment block
		/// </summary>
		Comment = 0x0002,

		/// <summary>
		/// The character is not part of compiled language source text
		/// (for example, the HTML surrounding a script block)
		/// </summary>
		NonSource = 0x0004,

		/// <summary>
		/// The character is part of a language operator
		/// (for example, the arithmetic operator <code>+</code>)
		/// </summary>
		Operator = 0x0008,

		/// <summary>
		/// The character is part of a language numeric constant
		/// (for example, the constant <code>3.14159</code>)
		/// </summary>
		Number = 0x0010,

		/// <summary>
		/// The character is part of a language string constant
		/// (for example, the string <code>"Hello World"</code>)
		/// </summary>
		String = 0x0020,

		/// <summary>
		/// The character indicates the start of a function block
		/// </summary>
		FunctionStart = 0x0040
	}
}
#endif
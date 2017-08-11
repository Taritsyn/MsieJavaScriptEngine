#if !NETSTANDARD1_3
using System.Runtime.CompilerServices;

namespace MsieJavaScriptEngine.Utilities
{
	/// <summary>
	/// Extensions for Char
	/// </summary>
	internal static class CharExtensions
	{
		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		public static bool IsWhitespace(this char source)
		{
			return source == ' ' || (source >= '\t' && source <= '\r');
		}
	}
}
#endif
#if !NETSTANDARD1_3
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.Helpers
{
	/// <summary>
	/// Text helpers
	/// </summary>
	internal static class TextHelpers
	{
		/// <summary>
		/// Array of characters used to find the next line break
		/// </summary>
		private static readonly char[] _nextLineBreakChars = new char[] { '\r', '\n' };


		/// <summary>
		/// Finds a next line break
		/// </summary>
		/// <param name="sourceText">Source text</param>
		/// <param name="startPosition">Position in the input string that defines the leftmost
		/// position to be searched</param>
		/// <param name="length">Number of characters in the substring to include in the search</param>
		/// <param name="lineBreakPosition">Position of line break</param>
		/// <param name="lineBreakLength">Length of line break</param>
		public static void FindNextLineBreak(string sourceText, int startPosition, int length,
			out int lineBreakPosition, out int lineBreakLength)
		{
			lineBreakPosition = sourceText.IndexOfAny(_nextLineBreakChars, startPosition, length);
			if (lineBreakPosition != -1)
			{
				lineBreakLength = 1;
				char currentCharacter = sourceText[lineBreakPosition];

				if (currentCharacter == '\r')
				{
					int nextCharacterPosition = lineBreakPosition + 1;
					char nextCharacter;

					if (sourceText.TryGetChar(nextCharacterPosition, out nextCharacter)
						&& nextCharacter == '\n')
					{
						lineBreakLength = 2;
					}
				}
			}
			else
			{
				lineBreakLength = 0;
			}
		}
	}
}
#endif
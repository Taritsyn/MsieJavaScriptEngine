using System;
using System.Text;

using AdvancedStringBuilder;

using MsieJavaScriptEngine.Extensions;

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
		/// Gets a fragment from the text line
		/// </summary>
		/// <param name="textLine">Content of the text line</param>
		/// <param name="columnNumber">Column number</param>
		/// <param name="maxFragmentLength">Maximum length of the text fragment</param>
		internal static string GetTextFragmentFromLine(string textLine, int columnNumber,
			int maxFragmentLength = 100)
		{
			if (string.IsNullOrEmpty(textLine))
			{
				return string.Empty;
			}

			string fragment;
			int lineLength = textLine.Length;

			if (lineLength > maxFragmentLength)
			{
				const string ellipsisSymbol = "…";
				string startPart = string.Empty;
				string endPart = string.Empty;

				var leftOffset = (int)Math.Floor((double)maxFragmentLength / 2);
				int fragmentStartPosition = columnNumber - leftOffset - 1;
				if (fragmentStartPosition > 0)
				{
					if (lineLength - fragmentStartPosition < maxFragmentLength)
					{
						fragmentStartPosition = lineLength - maxFragmentLength;
					}
				}
				else
				{
					fragmentStartPosition = 0;
				}
				int fragmentLength = maxFragmentLength;

				if (fragmentStartPosition > 0)
				{
					startPart = ellipsisSymbol;
				}
				if (fragmentStartPosition + fragmentLength < lineLength)
				{
					endPart = ellipsisSymbol;
				}

				var stringBuilderPool = StringBuilderPool.Shared;
				StringBuilder fragmentBuilder = stringBuilderPool.Rent();
				if (startPart.Length > 0)
				{
					fragmentBuilder.Append(startPart);
				}
				fragmentBuilder.Append(textLine.Substring(fragmentStartPosition, fragmentLength));
				if (endPart.Length > 0)
				{
					fragmentBuilder.Append(endPart);
				}

				fragment = fragmentBuilder.ToString();
				stringBuilderPool.Return(fragmentBuilder);
			}
			else
			{
				fragment = textLine;
			}

			return fragment;
		}

		/// <summary>
		/// Finds a next line break
		/// </summary>
		/// <param name="sourceText">Source text</param>
		/// <param name="startPosition">Position in the input string that defines the leftmost
		/// position to be searched</param>
		/// <param name="length">Number of characters in the substring to include in the search</param>
		/// <param name="lineBreakPosition">Position of line break</param>
		/// <param name="lineBreakLength">Length of line break</param>
		internal static void FindNextLineBreak(string sourceText, int startPosition, int length,
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
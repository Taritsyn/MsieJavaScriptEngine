using System;
using System.Linq;

namespace MsieJavaScriptEngine.Utilities
{
	/// <summary>
	/// Extensions for String
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Returns a value indicating whether the specified quoted string occurs within this string
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <param name="value">The string without quotes to seek</param>
		/// <returns>true if the quoted value occurs within this string; otherwise, false</returns>
		public static bool ContainsQuotedValue(this string source, string value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			bool result = source.Contains("'" + value + "'") || source.Contains("\"" + value + "\"");

			return result;
		}

		/// <summary>
		/// Removes leading occurrence of the specified string from the current <see cref="String"/> object
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <param name="trimString">An string to remove</param>
		/// <returns>The string that remains after removing of the specified string from the start of
		/// the current string</returns>
		public static string TrimStart(this string source, string trimString)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (trimString == null)
			{
				throw new ArgumentNullException("trimString");
			}

			if (source.Length == 0 || trimString.Length == 0)
			{
				return source;
			}

			string result = source;
			if (source.StartsWith(trimString, StringComparison.Ordinal))
			{
				result = source.Substring(trimString.Length);
			}

			return result;
		}

		/// <summary>
		/// Converts a first letter of string to capital
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <returns>The string starting with a capital letter</returns>
		public static string CapitalizeFirstLetter(this string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			int length = source.Length;
			if (length == 0)
			{
				return source;
			}

			string result;
			char firstCharacter = source.First();

			if (char.IsLower(firstCharacter))
			{
				result = char.ToUpperInvariant(firstCharacter).ToString();
				if (length > 1)
				{
					result += source.Substring(1);
				}
			}
			else
			{
				result = source;
			}

			return result;
		}

		/// <summary>
		/// Gets a character at the specified index from the string.
		/// A return value indicates whether the receiving succeeded.
		/// </summary>
		/// <param name="source">The source string</param>
		/// <param name="index">The zero-based index of the character</param>
		/// <param name="result">When this method returns, contains the character from the string,
		/// if the receiving succeeded, or null character if the receiving failed.
		/// The receiving fails if the index out of bounds.</param>
		/// <returns>true if the character was received successfully; otherwise, false</returns>
		public static bool TryGetChar(this string source, int index, out char result)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			bool isSuccess;
			int length = source.Length;

			if (length > 0 && index >= 0 && index < length)
			{
				result = source[index];
				isSuccess = true;
			}
			else
			{
				result = '\0';
				isSuccess = false;
			}

			return isSuccess;
		}
	}
}
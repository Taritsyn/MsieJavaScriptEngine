#if !NETSTANDARD
using System;
using System.Collections.Generic;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Extensions;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script error helpers
	/// </summary>
	internal static class ActiveScriptJsErrorHelpers
	{
		/// <summary>
		/// Checks whether the specified error number is compilation error
		/// </summary>
		/// <param name="errorNumber">Error number</param>
		/// <returns>Result of check (true - is compilation error; false - is not compilation error)</returns>
		public static bool IsCompilationError(int errorNumber)
		{
			bool result = errorNumber >= JScriptSyntaxErrorNumber.SyntaxError
				&& errorNumber <= JScriptSyntaxErrorNumber.ThrowMustBeFollowedByExpressionOnSameSourceLine;

			return result;
		}

		/// <summary>
		/// Checks whether the specified error number is runtime error
		/// </summary>
		/// <param name="errorNumber">Error number</param>
		/// <returns>Result of check (true - is runtime error; false - is not runtime error)</returns>
		public static bool IsRuntimeError(int errorNumber)
		{	bool result = errorNumber == JScriptRuntimeErrorNumber.OutOfStackSpace
				|| (errorNumber >= JScriptRuntimeErrorNumber.CannotAssignToThisKeyword
					&& errorNumber <= JScriptRuntimeErrorNumber.InvalidReplacerArgument);

			return result;
		}

		/// <summary>
		/// Gets a error type by number
		/// </summary>
		/// <param name="errorNumber">Error number</param>
		/// <param name="runtimeErrorTypeMap">Mapping of error numbers and types</param>
		/// <returns>Error type</returns>
		public static string GetErrorTypeByNumber(int errorNumber, Dictionary<int, string> runtimeErrorTypeMap)
		{
			string errorType = string.Empty;

			if (IsCompilationError(errorNumber))
			{
				errorType = JsErrorType.Syntax;
			}
			else if (IsRuntimeError(errorNumber))
			{
				if (!runtimeErrorTypeMap.TryGetValue(errorNumber, out errorType))
				{
					errorType = string.Empty;
				}
			}

			return errorType;
		}

		/// <summary>
		/// Shortens a name of error item
		/// </summary>
		/// <param name="itemName">Name of error item</param>
		/// <param name="prefix">Prefix</param>
		/// <returns>Short name of error item</returns>
		public static string ShortenErrorItemName(string itemName, string prefix)
		{
			if (itemName == null)
			{
				throw new ArgumentNullException(nameof(itemName));
			}

			if (prefix == null)
			{
				throw new ArgumentNullException(nameof(prefix));
			}

			int itemNameLength = itemName.Length;
			if (itemNameLength == 0 || prefix.Length == 0)
			{
				return itemName;
			}

			string shortItemName = itemName.TrimStart(prefix);
			int shortItemNameLength = shortItemName.Length;

			if (shortItemNameLength > 0 && shortItemNameLength < itemNameLength)
			{
				shortItemName = shortItemName.CapitalizeFirstLetter();
			}

			return shortItemName;
		}
	}
}
#endif
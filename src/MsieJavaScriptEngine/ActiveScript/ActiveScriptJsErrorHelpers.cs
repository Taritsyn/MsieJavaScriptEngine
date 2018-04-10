#if !NETSTANDARD1_3
using System;

using MsieJavaScriptEngine.Utilities;

using HResultHelpers = MsieJavaScriptEngine.Helpers.ComHelpers.HResult;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script error helpers
	/// </summary>
	internal static class ActiveScriptJsErrorHelpers
	{
		/// <summary>
		/// Checks whether the specified HRESULT value is syntax error
		/// </summary>
		/// <param name="hResult">The HRESULT value</param>
		/// <returns>Result of check (true - is syntax error; false - is not syntax error)</returns>
		public static bool IsSyntaxError(int hResult)
		{
			bool isSyntaxError = false;

			if (HResultHelpers.GetFacility(hResult) == HResultHelpers.FACILITY_CONTROL)
			{
				int errorCode = HResultHelpers.GetCode(hResult);
				isSyntaxError = errorCode >= 1002 && errorCode <= 1035;
			}

			return isSyntaxError;
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
				throw new ArgumentNullException("itemName");
			}

			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
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
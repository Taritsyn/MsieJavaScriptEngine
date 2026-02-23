using System;
using System.Linq;
using System.Text.RegularExpressions;

using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.Helpers
{
	/// <summary>
	/// Validation helpers
	/// </summary>
	public static class ValidationHelpers
	{
		/// <summary>
		/// List of supported types
		/// </summary>
		private static readonly Type[] _supportedTypes =
		[
			typeof(Undefined), typeof(Boolean), typeof(Int32), typeof(Double), typeof(String)
		];

		/// <summary>
		/// Regular expression for working with JS names
		/// </summary>
		private static readonly Regex _jsNameRegex = new Regex("^" + CommonRegExps.JsNamePattern + "$");

		/// <summary>
		/// Regular expression for working with document names
		/// </summary>
		private static readonly Regex _documentNameRegex = new Regex("^" + CommonRegExps.DocumentNamePattern + "$");


		/// <summary>
		/// Checks whether supports a .NET type
		/// </summary>
		/// <param name="type">.NET type</param>
		/// <returns>Result of check (<c>true</c> - is supported; <c>false</c> - is not supported)</returns>
		public static bool IsSupportedType(Type type)
		{
			bool result = _supportedTypes.Contains(type);

			return result;
		}

		/// <summary>
		/// Checks whether .NET type is primitive
		/// </summary>
		/// <param name="type">.NET type</param>
		/// <returns>Result of check (<c>true</c> - is primitive; <c>false</c> - is not primitive)</returns>
		public static bool IsPrimitiveType(Type type)
		{
			return TypeConverter.IsPrimitiveType(type);
		}

		/// <summary>
		/// Checks a format of the name
		/// </summary>
		/// <param name="name">The name</param>
		/// <returns>Result of check (<c>true</c> - correct format; <c>false</c> - wrong format)</returns>
		public static bool CheckNameFormat(string name)
		{
			return _jsNameRegex.IsMatch(name);
		}

		/// <summary>
		/// Checks a format of the document name
		/// </summary>
		/// <param name="name">The document name</param>
		/// <returns>Result of check (<c>true</c> - correct format; <c>false</c> - wrong format)</returns>
		public static bool CheckDocumentNameFormat(string name)
		{
			return _documentNameRegex.IsMatch(name);
		}
	}
}
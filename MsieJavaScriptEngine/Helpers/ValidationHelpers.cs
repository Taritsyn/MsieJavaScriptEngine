namespace MsieJavaScriptEngine.Helpers
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Validation helpers
	/// </summary>
	public static class ValidationHelpers
	{
		/// <summary>
		/// List of supported types
		/// </summary>
		private static readonly Type[] _supportedTypes =
		{
			typeof(Undefined), typeof(Boolean), typeof(Int32), typeof(Double), typeof(String)		 		
		};

		/// <summary>
		/// Regular expression for working with JS-names
		/// </summary>
		private static readonly Regex _jsNameRegex = new Regex(@"^[A-Za-z_\$]+[0-9A-Za-z_\$]*$",
			RegexOptions.Compiled);

		/// <summary>
		/// Regular expression for working with property names
		/// </summary>
		private static readonly Regex _jsPropertyNameRegex =
			new Regex(@"^[A-Za-z_\$]+[0-9A-Za-z_\$]*(\.[A-Za-z_\$]+[0-9A-Za-z_\$]*)*$",
				RegexOptions.Compiled);

		/// <summary>
		/// List of reserved words of JavaScript language
		/// </summary>
		private static readonly string[] _jsReservedWords =
		{
			"abstract",
			"boolean", "break", "byte",
			"case", "catch", "char", "class", "const", "continue",
			"debugger", "default", "delete", "do", "double",
			"else", "enum", "export", "extends",
			"false", "final", "finally", "float", "for", "function",
			"goto",
			"if", "implements", "import", "in", "instanceof", "int",
			"interface",
			"long",
			"native", "new", "null",
			"package", "private", "protected", "public",
			"return",
			"short", "static", "super", "switch", "synchronized",
			"this", "throw", "throws", "transient", "true", "try", "typeof",
			"var", "volatile", "void",
			"while", "with"
		};


		/// <summary>
		/// Checks whether supports a .NET type
		/// </summary>
		/// <param name="type">.NET type</param>
		/// <returns>Result of check (true - is supported; false - is not supported)</returns>
		public static bool IsSupportedType(Type type)
		{
			bool result = _supportedTypes.Contains(type);

			return result;
		}

		/// <summary>
		/// Checks a format of the name
		/// </summary>
		/// <param name="name">The name</param>
		/// <returns>Result of check (true - correct format; false - wrong format)</returns>
		public static bool CheckNameFormat(string name)
		{
			return _jsNameRegex.IsMatch(name);
		}

		/// <summary>
		/// Checks a allowability of the name (compares with the list of 
		/// reserved words of JavaScript language)
		/// </summary>
		/// <param name="name">The name</param>
		/// <returns>Result of check (true - allowed; false - forbidden)</returns>
		public static bool CheckNameAllowability(string name)
		{
			return !_jsReservedWords.Contains(name);
		}

		/// <summary>
		/// Checks a format of property name
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <returns>Result of check (true - correct format; false - wrong format)</returns>
		public static bool CheckPropertyNameFormat(string propertyName)
		{
			return _jsPropertyNameRegex.IsMatch(propertyName);
		}

		/// <summary>
		/// Checks a allowability of property name (compares one of its parts with the 
		/// list of reserved words of JavaScript language)
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <returns>Result of check (true - allowed; false - forbidden)</returns>
		public static bool CheckPropertyNameAllowability(string propertyName)
		{
			bool isAllowed = false;
			string[] parts = propertyName.Split('.');

			foreach (string part in parts)
			{
				isAllowed = CheckNameAllowability(part);
				if (!isAllowed)
				{
					break;
				}
			}

			return isAllowed;
		}
	}
}
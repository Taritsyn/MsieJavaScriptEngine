namespace MsieJavaScriptEngine.Helpers
{
	using System;
	using System.Globalization;
	using System.Text;

	using Resources;
	using Utilities;

	/// <summary>
	/// JavaScript engine error helpers
	/// </summary>
	public static class JsErrorHelpers
	{
		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="jsEngineLoadException">JavaScript engine load exception</param>
		/// <returns>Detailed error message</returns>
		public static string Format(JsEngineLoadException jsEngineLoadException)
		{
			var errorMessage = new StringBuilder();
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_Message,
				jsEngineLoadException.Message);
			if (!string.IsNullOrWhiteSpace(jsEngineLoadException.EngineMode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_EngineMode,
					jsEngineLoadException.EngineMode);
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="jsRuntimeException">JavaScript runtime exception</param>
		/// <returns>Detailed error message</returns>
		public static string Format(JsRuntimeException jsRuntimeException)
		{
			var errorMessage = new StringBuilder();
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_Message,
				jsRuntimeException.Message);
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.EngineMode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_EngineMode,
					jsRuntimeException.EngineMode);
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.ErrorCode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_ErrorCode,
					jsRuntimeException.ErrorCode);
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.Category))
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_Category,
					jsRuntimeException.Category);
			}
			if (jsRuntimeException.LineNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_LineNumber,
					jsRuntimeException.LineNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (jsRuntimeException.ColumnNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_ColumnNumber,
					jsRuntimeException.ColumnNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.SourceFragment))
			{
				errorMessage.AppendFormatLine("{1}:{0}{0}{2}", Environment.NewLine,
					Strings.ErrorDetails_SourceFragment,
					jsRuntimeException.SourceFragment);
			}

			return errorMessage.ToString();
		}
	}
}
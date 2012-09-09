namespace MsieJavaScriptEngine.ActiveScript
{
	using System.Text;

	using Resources;
	
	/// <summary>
	/// Responsible for formatting Active Script Error Message
	/// </summary>
	public static class ActiveScriptErrorFormatter
	{
		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="activeScriptException">Active script exception</param>
		/// <param name="filePath">File path</param>
		/// <returns>Detailed error message</returns>
		public static string Format(ActiveScriptException activeScriptException, string filePath)
		{
			var errorMessage = new StringBuilder();
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_Message,
				activeScriptException.Message);
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_ErrorCode,
				activeScriptException.ErrorCode);
			if (activeScriptException.ErrorWCode != 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_ErrorWCode,
					activeScriptException.ErrorWCode);
			}
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_Subcategory,
				activeScriptException.Subcategory);
			if (!string.IsNullOrWhiteSpace(activeScriptException.HelpLink))
			{
				errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_HelpKeyword,
					activeScriptException.HelpLink);
			}
			errorMessage.AppendFormatLine("{0}: {1}", Strings.ErrorDetails_File, filePath);

			return errorMessage.ToString();
		}
	}
}

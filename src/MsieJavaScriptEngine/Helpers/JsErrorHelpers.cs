using System;
using System.Globalization;
using System.Text;

using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.Helpers
{
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
			errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Message,
				jsEngineLoadException.Message);
			if (!string.IsNullOrWhiteSpace(jsEngineLoadException.EngineMode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_EngineMode,
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
			errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Message,
				jsRuntimeException.Message);
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.EngineMode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_EngineMode,
					jsRuntimeException.EngineMode);
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.ErrorCode))
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_ErrorCode,
					jsRuntimeException.ErrorCode);
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.Category))
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Category,
					jsRuntimeException.Category);
			}
			if (jsRuntimeException.LineNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_LineNumber,
					jsRuntimeException.LineNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (jsRuntimeException.ColumnNumber > 0)
			{
				errorMessage.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_ColumnNumber,
					jsRuntimeException.ColumnNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.SourceFragment))
			{
				errorMessage.AppendFormatLine("{1}:{0}{0}{2}", Environment.NewLine,
					CommonStrings.ErrorDetails_SourceFragment,
					jsRuntimeException.SourceFragment);
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// Writes a information about error location to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="documentName">Document name</param>
		internal static void WriteErrorLocation(StringBuilder buffer, string documentName)
		{
			WriteErrorLocation(buffer, documentName, 0, 0);
		}

		/// <summary>
		/// Writes a information about error location to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		internal static void WriteErrorLocation(StringBuilder buffer, int lineNumber, int columnNumber)
		{
			WriteErrorLocation(buffer, string.Empty, lineNumber, columnNumber);
		}

		/// <summary>
		/// Writes a information about error location to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="documentName">Document name</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		internal static void WriteErrorLocation(StringBuilder buffer, string documentName,
			int lineNumber, int columnNumber)
		{
			bool documentNameNotEmpty = !string.IsNullOrWhiteSpace(documentName);

			if (documentNameNotEmpty || lineNumber > 0)
			{
				buffer.Append("   at ");
				if (documentNameNotEmpty)
				{
					buffer.Append(documentName);
				}
				if (lineNumber > 0)
				{
					if (documentNameNotEmpty)
					{
						buffer.Append(":");
					}
					buffer.Append(lineNumber);
					if (columnNumber > 0)
					{
						buffer.AppendFormat(":{0}", columnNumber);
					}
				}
			}
		}
	}
}
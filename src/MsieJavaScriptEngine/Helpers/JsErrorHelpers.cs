using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.Helpers
{
	/// <summary>
	/// JS error helpers
	/// </summary>
	public static class JsErrorHelpers
	{
		#region Call stack

		/// <summary>
		/// Regular expression for working with line of the script call stack
		/// </summary>
		private static readonly Regex _callStackLineRegex =
			new Regex(@"^[ ]{3}at " +
				@"(?<functionName>[A-Za-z][A-Za-z ]*|" + CommonRegExps.JsNamePattern + @") " +
				@"\((?<documentName>" + CommonRegExps.DocumentNamePattern + @"):" +
				@"(?<lineNumber>\d+):(?<columnNumber>\d+)\)$");

		/// <summary>
		/// Parses a string representation of the script call stack to produce an array of
		/// <see cref="CallStackItem"/> instances
		/// </summary>
		/// <param name="callStack">String representation of the script call stack</param>
		/// <returns>An array of <see cref="CallStackItem"/> instances</returns>
		internal static CallStackItem[] ParseCallStack(string callStack)
		{
			if (string.IsNullOrWhiteSpace(callStack))
			{
				return new CallStackItem[0];
			}

			var callStackItems = new List<CallStackItem>();
			string[] lines = callStack.SplitToLines();
			int lineCount = lines.Length;

			for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
			{
				string line = lines[lineIndex];
				Match lineMatch = _callStackLineRegex.Match(line);

				if (lineMatch.Success)
				{
					GroupCollection lineGroups = lineMatch.Groups;

					var callStackItem = new CallStackItem
					{
						FunctionName = lineGroups["functionName"].Value,
						DocumentName = lineGroups["documentName"].Value,
						LineNumber = int.Parse(lineGroups["lineNumber"].Value),
						ColumnNumber = int.Parse(lineGroups["columnNumber"].Value)
					};
					callStackItems.Add(callStackItem);
				}
				else
				{
					Debug.WriteLine(string.Format(CommonStrings.Runtime_InvalidCallStackLineFormat, line));
					return new CallStackItem[0];
				}
			}

			return callStackItems.ToArray();
		}

		/// <summary>
		/// Produces a string representation of the script call stack from array of
		/// <see cref="CallStackItem"/> instances
		/// </summary>
		/// <param name="callStackItems">An array of <see cref="CallStackItem"/> instances</param>
		/// <returns>String representation of the script call stack</returns>
		internal static string StringifyCallStackItems(CallStackItem[] callStackItems)
		{
			if (callStackItems == null)
			{
				throw new ArgumentException(nameof(callStackItems));
			}

			int stackItemCount = callStackItems.Length;
			if (stackItemCount == 0)
			{
				return string.Empty;
			}

			StringBuilder stackBuilder = StringBuilderPool.GetBuilder();

			for (int stackItemIndex = 0; stackItemIndex < stackItemCount; stackItemIndex++)
			{
				CallStackItem stackItem = callStackItems[stackItemIndex];

				if (stackItemIndex > 0)
				{
					stackBuilder.AppendLine();
				}
				WriteErrorLocation(stackBuilder, stackItem.FunctionName, stackItem.DocumentName,
					stackItem.LineNumber, stackItem.ColumnNumber);
			}

			string callStack = stackBuilder.ToString();
			StringBuilderPool.ReleaseBuilder(stackBuilder);

			return callStack;
		}

		#endregion

		#region Generation of error messages

		/// <summary>
		/// Generates a error message
		/// </summary>
		/// <param name="type">Type of the script error</param>
		/// <param name="description">Description of error</param>
		/// <param name="documentName">Document name</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		/// <param name="sourceFragment">Source fragment</param>
		/// <returns>Error message</returns>
		internal static string GenerateErrorMessage(string type, string description,
			string documentName, int lineNumber, int columnNumber, string sourceFragment)
		{
			return GenerateErrorMessage(type, description, documentName, lineNumber, columnNumber,
				sourceFragment, string.Empty);
		}

		/// <summary>
		/// Generates a error message
		/// </summary>
		/// <param name="type">Type of the script error</param>
		/// <param name="description">Description of error</param>
		/// <param name="callStack">String representation of the script call stack</param>
		/// <returns>Error message</returns>
		internal static string GenerateErrorMessage(string type, string description, string callStack)
		{
			return GenerateErrorMessage(type, description, string.Empty, 0, 0, string.Empty, callStack);
		}

		/// <summary>
		/// Generates a error message
		/// </summary>
		/// <param name="type">Type of the script error</param>
		/// <param name="description">Description of error</param>
		/// <param name="documentName">Document name</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		/// <param name="sourceFragment">Source fragment</param>
		/// <param name="callStack">String representation of the script call stack</param>
		/// <returns>Error message</returns>
		internal static string GenerateErrorMessage(string type, string description, string documentName,
			int lineNumber, int columnNumber, string sourceFragment, string callStack)
		{
			if (description == null)
			{
				throw new ArgumentNullException(nameof(description));
			}

			if (string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(description)),
					nameof(description)
				);
			}

			StringBuilder messageBuilder = StringBuilderPool.GetBuilder();
			if (!string.IsNullOrWhiteSpace(type))
			{
				messageBuilder.AppendFormat("{0}: ", type);
			}
			messageBuilder.Append(description);

			if (!string.IsNullOrWhiteSpace(callStack))
			{
				messageBuilder.AppendLine();
				messageBuilder.Append(callStack);
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(documentName) || lineNumber > 0)
				{
					messageBuilder.AppendLine();
					WriteErrorLocation(messageBuilder, string.Empty, documentName, lineNumber, columnNumber,
						sourceFragment);
				}
			}

			string errorMessage = messageBuilder.ToString();
			StringBuilderPool.ReleaseBuilder(messageBuilder);

			return errorMessage;
		}

		#endregion

		#region Generation of error details

		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="jsException">JS exception</param>
		/// <param name="omitMessage">Flag for whether to omit message</param>
		/// <returns>Detailed error message</returns>
		public static string GenerateErrorDetails(JsException jsException, bool omitMessage = false)
		{
			if (jsException == null)
			{
				throw new ArgumentNullException(nameof(jsException));
			}

			StringBuilder detailsBuilder = StringBuilderPool.GetBuilder();
			WriteCommonErrorDetails(detailsBuilder, jsException, omitMessage);

			var jsScriptException = jsException as JsScriptException;
			if (jsScriptException != null)
			{
				WriteScriptErrorDetails(detailsBuilder, jsScriptException);

				var jsRuntimeException = jsScriptException as JsRuntimeException;
				if (jsRuntimeException != null)
				{
					WriteRuntimeErrorDetails(detailsBuilder, jsRuntimeException);
				}
			}

			string errorDetails = detailsBuilder.ToString();
			StringBuilderPool.ReleaseBuilder(detailsBuilder);

			return errorDetails;
		}

		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="jsScriptException">JS script exception</param>
		/// <param name="omitMessage">Flag for whether to omit message</param>
		/// <returns>Detailed error message</returns>
		public static string GenerateErrorDetails(JsScriptException jsScriptException,
			bool omitMessage = false)
		{
			if (jsScriptException == null)
			{
				throw new ArgumentNullException(nameof(jsScriptException));
			}

			StringBuilder detailsBuilder = StringBuilderPool.GetBuilder();
			WriteCommonErrorDetails(detailsBuilder, jsScriptException, omitMessage);
			WriteScriptErrorDetails(detailsBuilder, jsScriptException);

			var jsRuntimeException = jsScriptException as JsRuntimeException;
			if (jsRuntimeException != null)
			{
				WriteRuntimeErrorDetails(detailsBuilder, jsRuntimeException);
			}

			string errorDetails = detailsBuilder.ToString();
			StringBuilderPool.ReleaseBuilder(detailsBuilder);

			return errorDetails;
		}

		/// <summary>
		/// Generates a detailed error message
		/// </summary>
		/// <param name="jsRuntimeException">JS runtime exception</param>
		/// <param name="omitMessage">Flag for whether to omit message</param>
		/// <returns>Detailed error message</returns>
		public static string GenerateErrorDetails(JsRuntimeException jsRuntimeException,
			bool omitMessage = false)
		{
			if (jsRuntimeException == null)
			{
				throw new ArgumentNullException(nameof(jsRuntimeException));
			}

			StringBuilder detailsBuilder = StringBuilderPool.GetBuilder();
			WriteCommonErrorDetails(detailsBuilder, jsRuntimeException, omitMessage);
			WriteScriptErrorDetails(detailsBuilder, jsRuntimeException);
			WriteRuntimeErrorDetails(detailsBuilder, jsRuntimeException);

			string errorDetails = detailsBuilder.ToString();
			StringBuilderPool.ReleaseBuilder(detailsBuilder);

			return errorDetails;
		}

		/// <summary>
		/// Writes a detailed error message to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="jsException">JS exception</param>
		/// <param name="omitMessage">Flag for whether to omit message</param>
		private static void WriteCommonErrorDetails(StringBuilder buffer, JsException jsException,
			bool omitMessage = false)
		{
			if (!omitMessage)
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Message,
					jsException.Message);
			}
			if (!string.IsNullOrWhiteSpace(jsException.EngineMode))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_EngineMode,
					jsException.EngineMode);
			}
			if (!string.IsNullOrWhiteSpace(jsException.Category))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Category,
					jsException.Category);
			}
			if (!string.IsNullOrWhiteSpace(jsException.Description))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Description,
					jsException.Description);
			}
		}

		/// <summary>
		/// Writes a detailed error message to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="jsScriptException">JS script exception</param>
		private static void WriteScriptErrorDetails(StringBuilder buffer,
			JsScriptException jsScriptException)
		{
			if (!string.IsNullOrWhiteSpace(jsScriptException.Type))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_Type,
					jsScriptException.Type);
			}
			if (!string.IsNullOrWhiteSpace(jsScriptException.DocumentName))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_DocumentName,
					jsScriptException.DocumentName);
			}
			if (jsScriptException.LineNumber > 0)
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_LineNumber,
					jsScriptException.LineNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (jsScriptException.ColumnNumber > 0)
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_ColumnNumber,
					jsScriptException.ColumnNumber.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrWhiteSpace(jsScriptException.SourceFragment))
			{
				buffer.AppendFormatLine("{0}: {1}", CommonStrings.ErrorDetails_SourceFragment,
					jsScriptException.SourceFragment);
			}
		}

		/// <summary>
		/// Writes a detailed error message to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="jsRuntimeException">JS runtime exception</param>
		private static void WriteRuntimeErrorDetails(StringBuilder buffer,
			JsRuntimeException jsRuntimeException)
		{
			if (!string.IsNullOrWhiteSpace(jsRuntimeException.CallStack))
			{
				buffer.AppendFormatLine("{1}:{0}{0}{2}", Environment.NewLine,
					CommonStrings.ErrorDetails_CallStack,
					jsRuntimeException.CallStack);
			}
		}

		#endregion

		#region Misc

		/// <summary>
		/// Gets a fragment from the source line
		/// </summary>
		/// <param name="sourceLine">Content of the source line</param>
		/// <param name="columnNumber">Column number</param>
		/// <param name="maxFragmentLength">Maximum length of the source fragment</param>
		internal static string GetSourceFragment(string sourceLine, int columnNumber,
			int maxFragmentLength = 100)
		{
			if (string.IsNullOrWhiteSpace(sourceLine))
			{
				return string.Empty;
			}

			string fragment;
			int lineLength = sourceLine.Length;

			if (lineLength > maxFragmentLength)
			{
				const string ellipsisSymbol = "…";
				string startPart = string.Empty;
				string endPart = string.Empty;

				var leftOffset = (int)Math.Floor((double)maxFragmentLength / 2);
				int fragmentStartPosition = columnNumber - leftOffset - 1;
				if (fragmentStartPosition < 0)
				{
					fragmentStartPosition = 0;
				}
				int fragmentLength = maxFragmentLength;

				if (fragmentStartPosition > 0)
				{
					startPart = ellipsisSymbol;
					fragmentLength--;
				}
				if (fragmentStartPosition + maxFragmentLength < lineLength)
				{
					endPart = ellipsisSymbol;
					fragmentLength--;
				}

				StringBuilder fragmentBuilder = StringBuilderPool.GetBuilder();
				if (startPart.Length > 0)
				{
					fragmentBuilder.Append(startPart);
				}
				fragmentBuilder.Append(sourceLine.Substring(fragmentStartPosition, fragmentLength));
				if (endPart.Length > 0)
				{
					fragmentBuilder.Append(endPart);
				}

				fragment = fragmentBuilder.ToString();
				StringBuilderPool.ReleaseBuilder(fragmentBuilder);
			}
			else
			{
				fragment = sourceLine;
			}

			return fragment;
		}

		/// <summary>
		/// Writes a information about error location to the buffer
		/// </summary>
		/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
		/// <param name="functionName">Function name</param>
		/// <param name="documentName">Document name</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		/// <param name="sourceFragment">Source fragment</param>
		internal static void WriteErrorLocation(StringBuilder buffer, string functionName,
			string documentName, int lineNumber, int columnNumber, string sourceFragment = "")
		{
			bool functionNameNotEmpty = !string.IsNullOrWhiteSpace(functionName);
			bool documentNameNotEmpty = !string.IsNullOrWhiteSpace(documentName);

			if (functionNameNotEmpty || documentNameNotEmpty || lineNumber > 0)
			{
				buffer.Append("   at ");
				if (functionNameNotEmpty)
				{
					buffer.Append(functionName);
				}
				if (documentNameNotEmpty || lineNumber > 0)
				{
					if (functionNameNotEmpty)
					{
						buffer.Append(" (");
					}
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
					if (functionNameNotEmpty)
					{
						buffer.Append(")");
					}
					if (!string.IsNullOrWhiteSpace(sourceFragment))
					{
						buffer.Append(" -> ");
						buffer.Append(sourceFragment);
					}
				}
			}
		}

		#endregion
	}
}
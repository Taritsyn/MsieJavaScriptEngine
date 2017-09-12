using System;
#if NETSTANDARD1_3
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif
using System.Text;
using System.Text.RegularExpressions;

using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// JsRT version of Chakra JS engine
	/// </summary>
	internal abstract class ChakraJsRtJsEngineBase : InnerJsEngineBase
	{
		/// <summary>
		/// Regular expression for working with the string representation of error
		/// </summary>
		private static readonly Regex _errorStringRegex =
			new Regex(@"[ ]{3,5}at (?:[A-Za-z_\$][0-9A-Za-z_\$ ]* )?" +
				@"\([^\s*?""<>|][^\t\n\r*?""<>|]*?:(?<lineNumber>\d+):(?<columnNumber>\d+)\)");

		/// <summary>
		/// JS source context
		/// </summary>
		protected JsSourceContext _jsSourceContext = JsSourceContext.FromIntPtr(IntPtr.Zero);

		/// <summary>
		/// Flag indicating whether debugging started
		/// </summary>
		private StatedFlag _debuggingStartedFlag;
#if NETSTANDARD1_3

		/// <summary>
		/// Set of external objects
		/// </summary>
		protected readonly HashSet<object> _externalObjects = new HashSet<object>();

		/// <summary>
		/// Callback for finalization of external object
		/// </summary>
		protected JsObjectFinalizeCallback _externalObjectFinalizeCallback;
#endif

		/// <summary>
		/// Script dispatcher
		/// </summary>
		protected readonly ScriptDispatcher _dispatcher = new ScriptDispatcher();


		/// <summary>
		/// Constructs an instance of the Chakra JsRT engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		protected ChakraJsRtJsEngineBase(JsEngineSettings settings)
			: base(settings)
		{
#if NETSTANDARD1_3
			_externalObjectFinalizeCallback = ExternalObjectFinalizeCallback;
#endif
		}


		/// <summary>
		/// Generates a error message with location
		/// </summary>
		/// <param name="category">Error category</param>
		/// <param name="message">Error message</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		/// <returns>Error message with location</returns>
		protected static string GenerateErrorMessageWithLocation(string category, string message,
			int lineNumber, int columnNumber)
		{
			var messageBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(category))
			{
				messageBuilder.AppendFormat("{0}: ", category);
			}
			messageBuilder.Append(message);
			if (lineNumber > 0)
			{
				messageBuilder.AppendLine();
				JsErrorHelpers.WriteErrorLocation(messageBuilder, lineNumber, columnNumber);
			}

			string errorMessage = messageBuilder.ToString();
			messageBuilder.Clear();

			return errorMessage;
		}

		/// <summary>
		/// Gets a error coordinates from message
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="lineNumber">Line number</param>
		/// <param name="columnNumber">Column number</param>
		protected static void GetErrorCoordinatesFromMessage(string message, out int lineNumber,
			out int columnNumber)
		{
			lineNumber = 0;
			columnNumber = 0;

			if (!string.IsNullOrWhiteSpace(message))
			{
				Match errorStringMatch = _errorStringRegex.Match(message);
				if (errorStringMatch.Success)
				{
					GroupCollection errorStringGroups = errorStringMatch.Groups;

					lineNumber = int.Parse(errorStringGroups["lineNumber"].Value);
					columnNumber = int.Parse(errorStringGroups["columnNumber"].Value);
				}
			}
		}

		/// <summary>
		/// Starts debugging
		/// </summary>
		protected void StartDebugging()
		{
			if (_debuggingStartedFlag.Set())
			{
				InnerStartDebugging();
			}
		}

		protected abstract void InnerStartDebugging();
#if NETSTANDARD1_3

		private void ExternalObjectFinalizeCallback(IntPtr data)
		{
			if (data == IntPtr.Zero)
			{
				return;
			}

			GCHandle handle = GCHandle.FromIntPtr(data);
			object obj = handle.Target;

			if (obj == null)
			{
				return;
			}

			if (_externalObjects != null)
			{
				_externalObjects.Remove(obj);
			}
		}
#endif

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of
		/// managed objects contained in fields of class</param>
		protected virtual void Dispose(bool disposing)
		{
#if NETSTANDARD1_3
			if (disposing)
			{
				if (_externalObjects != null)
				{
					_externalObjects.Clear();
				}

				_externalObjectFinalizeCallback = null;
			}
#endif
		}

		#endregion
	}
}
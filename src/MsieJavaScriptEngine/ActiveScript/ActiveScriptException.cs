#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MsieJavaScriptEngine.ActiveScript
{
	[Serializable]
	internal sealed class ActiveScriptException : Exception
	{
		/// <summary>
		/// Error code
		/// </summary>
		private int _errorCode;

		/// <summary>
		/// WCode
		/// </summary>
		private short _errorWCode;

		/// <summary>
		/// Application specific source context
		/// </summary>
		private uint _sourceContext;

		/// <summary>
		/// Subcategory of error
		/// </summary>
		private string _subcategory = string.Empty;

		/// <summary>
		/// Line number on which the error occurred
		/// </summary>
		private uint _lineNumber;

		/// <summary>
		/// Column number on which the error occurred
		/// </summary>
		private int _columnNumber;

		/// <summary>
		/// Content of the line on which the error occurred
		/// </summary>
		private string _sourceError = string.Empty;

		/// <summary>
		/// Gets or sets a error code
		/// </summary>
		public int ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}

		/// <summary>
		/// Gets or sets a WCode
		/// </summary>
		public short ErrorWCode
		{
			get { return _errorWCode; }
			set { _errorWCode = value; }
		}

		/// <summary>
		/// Gets or sets a application specific source context
		/// </summary>
		public uint SourceContext
		{
			get { return _sourceContext; }
			set { _sourceContext = value; }
		}

		/// <summary>
		/// Gets or sets a subcategory of error
		/// </summary>
		public string Subcategory
		{
			get { return _subcategory; }
			set { _subcategory = value; }
		}

		/// <summary>
		/// Gets or sets a line number on which the error occurred
		/// </summary>
		public uint LineNumber
		{
			get { return _lineNumber; }
			set { _lineNumber = value; }
		}

		/// <summary>
		/// Gets or sets a column number on which the error occurred
		/// </summary>
		public int ColumnNumber
		{
			get { return _columnNumber; }
			set { _columnNumber = value; }
		}

		/// <summary>
		/// Gets or sets a content of the line on which the error occurred
		/// </summary>
		public string SourceError
		{
			get { return _sourceError; }
			set { _sourceError = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message</param>
		public ActiveScriptException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="innerException">The inner exception</param>
		public ActiveScriptException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		private ActiveScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_errorCode = info.GetInt32("ErrorCode");
				_errorWCode = info.GetInt16("ErrorWCode");
				_sourceContext = info.GetUInt32("SourceContext");
				_subcategory = info.GetString("Subcategory");
				_lineNumber = info.GetUInt32("LineNumber");
				_columnNumber = info.GetInt32("ColumnNumber");
				_sourceError = info.GetString("SourceError");
			}
		}


		internal static ActiveScriptException Create(IActiveScriptError error)
		{
			string message = string.Empty;
			int errorCode = 0;
			short errorWCode = 0;
			uint sourceContext = 0;
			string subcategory = string.Empty;
			string helpLink = string.Empty;
			uint lineNumber = 0;
			int columnNumber = 0;
			string sourceError = string.Empty;

			try
			{
				error.GetSourceLineText(out sourceError);
			}
			catch
			{
				// Do nothing
			}

			try
			{
				error.GetSourcePosition(out sourceContext, out lineNumber, out columnNumber);
				++lineNumber;
				++columnNumber;
			}
			catch
			{
				// Do nothing
			}

			try
			{
				EXCEPINFO excepInfo;
				error.GetExceptionInfo(out excepInfo);

				message = excepInfo.bstrDescription;
				subcategory = excepInfo.bstrSource;
				errorCode = excepInfo.scode;
				errorWCode = excepInfo.wCode;
				if (!string.IsNullOrWhiteSpace(excepInfo.bstrHelpFile)
					&& excepInfo.dwHelpContext != 0)
				{
					helpLink = string.Format("{0}: {1}", excepInfo.bstrHelpFile, excepInfo.dwHelpContext);
				}
				else if (!string.IsNullOrWhiteSpace(excepInfo.bstrHelpFile))
				{
					helpLink = excepInfo.bstrHelpFile;
				}
			}
			catch
			{
				// Do nothing
			}

			var activeScriptException = new ActiveScriptException(message)
			{
				ErrorCode = errorCode,
				ErrorWCode = errorWCode,
				SourceContext = sourceContext,
				Subcategory = subcategory,
				LineNumber = lineNumber,
				ColumnNumber = columnNumber,
				SourceError = sourceError,
				HelpLink = helpLink,
			};

			return activeScriptException;
		}

		#region Exception overrides

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data</param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", _errorCode);
			info.AddValue("ErrorWCode", _errorWCode);
			info.AddValue("SourceContext", _sourceContext);
			info.AddValue("Subcategory", _subcategory);
			info.AddValue("LineNumber", _lineNumber);
			info.AddValue("ColumnNumber", _columnNumber);
			info.AddValue("SourceError", _sourceError);
		}

		#endregion
	}
}
#endif
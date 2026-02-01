#if NETFRAMEWORK
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MsieJavaScriptEngine.ActiveScript
{
	[Serializable]
	public sealed class ActiveScriptException : Exception
	{
		/// <summary>
		/// The <c>HRESULT</c> of the error
		/// </summary>
		private int _errorCode;

		/// <summary>
		/// Type of the script error
		/// </summary>
		private string _type = string.Empty;

		/// <summary>
		/// Category of error
		/// </summary>
		private string _category = string.Empty;

		/// <summary>
		/// Description of error
		/// </summary>
		private string _description = string.Empty;

		/// <summary>
		/// Document name
		/// </summary>
		private string _documentName = string.Empty;

		/// <summary>
		/// Line number on which the error occurred
		/// </summary>
		private uint _lineNumber;

		/// <summary>
		/// Column number on which the error occurred
		/// </summary>
		private int _columnNumber;

		/// <summary>
		/// String representation of the script call stack
		/// </summary>
		private string _callStack = string.Empty;

		/// <summary>
		/// Source fragment
		/// </summary>
		private string _sourceFragment = string.Empty;

		/// <summary>
		/// Gets or sets a <c>HRESULT</c> of the error
		/// </summary>
		public int ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}

		/// <summary>
		/// Gets or sets a type of the script error
		/// </summary>
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>
		/// Gets or sets a category of error
		/// </summary>
		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		/// <summary>
		/// Gets or sets a description of error
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets a document name
		/// </summary>
		public string DocumentName
		{
			get { return _documentName; }
			set { _documentName = value; }
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
		/// Gets or sets a string representation of the script call stack
		/// </summary>
		public string CallStack
		{
			get { return _callStack; }
			set { _callStack = value; }
		}

		/// <summary>
		/// Gets or sets a source fragment
		/// </summary>
		public string SourceFragment
		{
			get { return _sourceFragment; }
			set { _sourceFragment = value; }
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
			if (info is not null)
			{
				_errorCode = info.GetInt32("ErrorCode");
				_type = info.GetString("Type");
				_category = info.GetString("Category");
				_description = info.GetString("Description");
				_documentName = info.GetString("DocumentName");
				_lineNumber = info.GetUInt32("LineNumber");
				_columnNumber = info.GetInt32("ColumnNumber");
				_callStack = info.GetString("CallStack");
				_sourceFragment = info.GetString("SourceFragment");
			}
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
			if (info is null)
			{
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", _errorCode);
			info.AddValue("Type", _type);
			info.AddValue("Category", _category);
			info.AddValue("Description", _description);
			info.AddValue("DocumentName", _documentName);
			info.AddValue("LineNumber", _lineNumber);
			info.AddValue("ColumnNumber", _columnNumber);
			info.AddValue("CallStack", _callStack);
			info.AddValue("SourceFragment", _sourceFragment);
		}

		#endregion
	}
}
#endif
﻿using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// The exception that is thrown during the script processing
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public class JsScriptException : JsException
	{
		/// <summary>
		/// Type of the script error
		/// </summary>
		private string _type = string.Empty;

		/// <summary>
		/// Document name
		/// </summary>
		private string _documentName = string.Empty;

		/// <summary>
		/// Line number
		/// </summary>
		private int _lineNumber;

		/// <summary>
		/// Column number
		/// </summary>
		private int _columnNumber;

		/// <summary>
		/// Source fragment
		/// </summary>
		private string _sourceFragment = string.Empty;

		/// <summary>
		/// Gets or sets a type of the script error
		/// </summary>
		public string Type
		{
			get;
			set;
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
		/// Gets or sets a line number
		/// </summary>
		public int LineNumber
		{
			get { return _lineNumber; }
			set { _lineNumber = value; }
		}

		/// <summary>
		/// Gets or sets a column number
		/// </summary>
		public int ColumnNumber
		{
			get { return _columnNumber; }
			set { _columnNumber = value; }
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
		/// Initializes a new instance of the <see cref="JsScriptException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsScriptException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsScriptException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		public JsScriptException(string message, string engineMode)
			: base(message, engineMode)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsScriptException(string message, string engineMode, Exception innerException)
			: base(message, engineMode, innerException)
		{ }
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		protected JsScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_type = info.GetString("Type");
				_documentName = info.GetString("DocumentName");
				_lineNumber = info.GetInt32("LineNumber");
				_columnNumber = info.GetInt32("ColumnNumber");
				_sourceFragment = info.GetString("SourceFragment");
			}
		}


		#region JsException overrides

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
				throw new ArgumentNullException(nameof(info));
			}

			base.GetObjectData(info, context);
			info.AddValue("Type", _type);
			info.AddValue("DocumentName", _documentName);
			info.AddValue("LineNumber", _lineNumber);
			info.AddValue("ColumnNumber", _columnNumber);
			info.AddValue("SourceFragment", _sourceFragment);
		}

		#endregion
#endif
	}
}
namespace MsieJavaScriptEngine.ActiveScript
{
	using System;
	using System.Runtime.Serialization;
	using System.Runtime.InteropServices.ComTypes;

	[Serializable]
	internal sealed class ActiveScriptException : Exception
	{
		/// <summary>
		/// Gets or sets a error code
		/// </summary>
		public int ErrorCode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a WCode
		/// </summary>
		public short ErrorWCode
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets a application specific source context
		/// </summary>
		public uint SourceContext
		{
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets a subcategory of error
		/// </summary>
		public string Subcategory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a line number on which the error occurred
		/// </summary>
		public uint LineNumber
		{
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets a column number on which the error occurred
		/// </summary>
		public int ColumnNumber
		{
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets a content of the line on which the error occurred
		/// </summary>
		public string SourceError
		{
			get;
			set;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// </summary>
		public ActiveScriptException() 
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// </summary>
		/// <param name="message">The message</param>
		public ActiveScriptException(string message)
			: base(message) 
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// </summary>
		/// <param name="innerException">The inner exception</param>
		public ActiveScriptException(Exception innerException)
			: base(null, innerException) 
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="innerException">The inner exception</param>
		public ActiveScriptException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveScriptException"/> class
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data about the exception being thrown</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> 
		/// that contains contextual information about the source or destination</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0)
		/// </exception>
		private ActiveScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context) 
		{ }


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
	}
}
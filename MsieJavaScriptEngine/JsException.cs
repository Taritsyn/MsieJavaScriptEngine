namespace MsieJavaScriptEngine
{
	using System;

	/// <summary>
	/// The exception that is thrown during the work of JavaScript engine
	/// </summary>
	public class JsException : Exception
	{
		/// <summary>
		/// Gets a name of JavaScript engine mode
		/// </summary>
		public string EngineMode
		{
			get;
			private set;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class 
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsException(string message)
			: this(message, string.Empty)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class 
		/// with a specified error message and a reference to the inner exception that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsException(string message, Exception innerException)
			: this(message, string.Empty, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class 
		/// with a specified error message and a reference to the inner exception that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JavaScript engine mode</param>
		public JsException(string message, string engineMode)
			: this(message, engineMode, null)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class 
		/// with a specified error message and a reference to the inner exception that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JavaScript engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsException(string message, string engineMode, Exception innerException)
			: base(message, innerException)
		{
			EngineMode = engineMode;
		}
	}
}
namespace MsieJavaScriptEngine.JsRt
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// A script exception.
	/// </summary>
	[Serializable]
	internal sealed class JavaScriptScriptException : JavaScriptException
	{
		/// <summary>
		/// The error.
		/// </summary>
		[NonSerialized]
		private readonly JavaScriptValue _error;

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptScriptException"/> class. 
		/// </summary>
		/// <param name="code">The error code returned.</param>
		/// <param name="error">The JavaScript error object.</param>
		public JavaScriptScriptException(JavaScriptErrorCode code, JavaScriptValue error) :
			this(code, error, "JavaScript Exception")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptScriptException"/> class. 
		/// </summary>
		/// <param name="code">The error code returned.</param>
		/// <param name="error">The JavaScript error object.</param>
		/// <param name="message">The error message.</param>
		public JavaScriptScriptException(JavaScriptErrorCode code, JavaScriptValue error, string message) :
			base(code, message)
		{
			_error = error;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptScriptException"/> class.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private JavaScriptScriptException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		/// <summary>
		/// Gets a JavaScript object representing the script error.
		/// </summary>
		public JavaScriptValue Error
		{
			get
			{
				return _error;
			}
		}
	}
}
#if !NETSTANDARD1_3
using System;
using System.Runtime.Serialization;
#endif

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” script exception
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public sealed class IeJsScriptException : JsException
	{
		/// <summary>
		/// The error
		/// </summary>
#if !NETSTANDARD1_3
		[NonSerialized]
#endif
		private readonly IeJsValue _error;

		/// <summary>
		/// Gets a JavaScript object representing the script error
		/// </summary>
		internal IeJsValue Error
		{
			get { return _error; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsScriptException"/> class
		/// </summary>
		/// <param name="errorCode">The error code returned</param>
		public IeJsScriptException(JsErrorCode errorCode)
			: this(errorCode, "JavaScript Exception")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsScriptException"/> class
		/// </summary>
		/// <param name="errorCode">The error code returned</param>
		/// <param name="message">The error message</param>
		public IeJsScriptException(JsErrorCode errorCode, string message)
			: this(errorCode, IeJsValue.Invalid, message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsScriptException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="errorCode">The error code returned</param>
		/// <param name="error">The JavaScript error object</param>
		/// <param name="message">The error message</param>
		internal IeJsScriptException(JsErrorCode errorCode, IeJsValue error, string message)
			: base(errorCode, message)
		{
			_error = error;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsScriptException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		private IeJsScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif
	}
}
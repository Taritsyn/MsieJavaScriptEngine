#if !NETSTANDARD1_3
using System;
using System.Runtime.Serialization;
#endif

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” script exception
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	internal sealed class EdgeJsScriptException : JsException
	{
		/// <summary>
		/// The error
		/// </summary>
#if !NETSTANDARD1_3
		[NonSerialized]
#endif
		private readonly EdgeJsValue _error;

		/// <summary>
		/// Gets a JavaScript object representing the script error
		/// </summary>
		public EdgeJsValue Error
		{
			get { return _error; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsScriptException"/> class
		/// </summary>
		/// <param name="errorCode">The error code returned</param>
		/// <param name="error">The JavaScript error object</param>
		public EdgeJsScriptException(JsErrorCode errorCode, EdgeJsValue error)
			: this(errorCode, error, "JavaScript Exception")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsScriptException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="errorCode">The error code returned</param>
		/// <param name="error">The JavaScript error object</param>
		/// <param name="message">The error message</param>
		public EdgeJsScriptException(JsErrorCode errorCode, EdgeJsValue error, string message)
			: base(errorCode, message)
		{
			_error = error;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsScriptException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		private EdgeJsScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif
	}
}
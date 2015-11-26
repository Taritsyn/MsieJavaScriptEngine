namespace MsieJavaScriptEngine.JsRt
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The fatal exception
	/// </summary>
	[Serializable]
	internal sealed class JsFatalException : JsException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// </summary>
		/// <param name="code">The error code returned</param>
		public JsFatalException(JsErrorCode code)
			: this(code, "A fatal exception has occurred in a JavaScript runtime")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// </summary>
		/// <param name="code">The error code returned</param>
		/// <param name="message">The error message</param>
		public JsFatalException(JsErrorCode code, string message)
			: base(code, message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// </summary>
		/// <param name="info">The serialization info</param>
		/// <param name="context">The streaming context</param>
		private JsFatalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
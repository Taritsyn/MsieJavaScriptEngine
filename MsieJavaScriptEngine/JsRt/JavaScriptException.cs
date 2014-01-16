namespace MsieJavaScriptEngine.JsRt
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// An exception returned from the Chakra engine.
	/// </summary>
	[Serializable]
	internal class JavaScriptException : Exception
	{
		/// <summary>
		/// The error code.
		/// </summary>
		private readonly JavaScriptErrorCode _code;

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptException"/> class. 
		/// </summary>
		/// <param name="code">The error code returned.</param>
		public JavaScriptException(JavaScriptErrorCode code) :
			this(code, "A fatal exception has occurred in a JavaScript runtime")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptException"/> class. 
		/// </summary>
		/// <param name="code">The error code returned.</param>
		/// <param name="message">The error message.</param>
		public JavaScriptException(JavaScriptErrorCode code, string message) :
			base(message)
		{
			_code = code;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptException"/> class. 
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		protected JavaScriptException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
			if (info != null)
			{
				_code = (JavaScriptErrorCode) info.GetUInt32("code");
			}
		}

		/// <summary>
		/// Serializes the exception information.
		/// </summary>
		/// <param name="info">The serialization information.</param>
		/// <param name="context">The streaming context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("code", (uint)_code);
		}

		/// <summary>
		/// Gets the error code.
		/// </summary>
		public JavaScriptErrorCode ErrorCode
		{
			get { return _code; }
		}
	}
}
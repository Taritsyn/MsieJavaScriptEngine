using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
#endif

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// The exception returned from the Chakra engine
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	internal class JsException : Exception
	{
		/// <summary>
		/// The error code
		/// </summary>
		private readonly JsErrorCode _code;

		/// <summary>
		/// Gets a error code
		/// </summary>
		public JsErrorCode ErrorCode
		{
			get { return _code; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// </summary>
		/// <param name="code">The error code returned</param>
		public JsException(JsErrorCode code)
			: this(code, "A fatal exception has occurred in a JavaScript runtime")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// </summary>
		/// <param name="code">The error code returned</param>
		/// <param name="message">The error message</param>
		public JsException(JsErrorCode code, string message)
			: base(message)
		{
			_code = code;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// </summary>
		/// <param name="info">The serialization info</param>
		/// <param name="context">The streaming context</param>
		protected JsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_code = (JsErrorCode) info.GetUInt32("code");
			}
		}


		/// <summary>
		/// Serializes the exception information
		/// </summary>
		/// <param name="info">The serialization information</param>
		/// <param name="context">The streaming context</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("code", (uint)_code);
		}
#endif
	}
}
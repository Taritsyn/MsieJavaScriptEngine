using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
#endif

using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// The fatal exception occurred
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public sealed class JsFatalException : JsException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsFatalException(string message)
			: base(message)
		{
			Category = JsErrorCategory.Fatal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsFatalException(string message, Exception innerException)
			: base(message, innerException)
		{
			Category = JsErrorCategory.Fatal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		public JsFatalException(string message, string engineMode)
			: base(message, engineMode)
		{
			Category = JsErrorCategory.Fatal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsFatalException(string message, string engineMode, Exception innerException)
			: base(message, engineMode, innerException)
		{
			Category = JsErrorCategory.Fatal;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsFatalException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		private JsFatalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif
	}
}
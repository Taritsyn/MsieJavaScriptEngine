﻿using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
#endif

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// The exception that is thrown when script execution is interrupted by the host
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public sealed class JsScriptInterruptedException : JsException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptInterruptedException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsScriptInterruptedException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptInterruptedException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsScriptInterruptedException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptInterruptedException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		public JsScriptInterruptedException(string message, string engineMode)
			: base(message, engineMode)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptInterruptedException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsScriptInterruptedException(string message, string engineMode, Exception innerException)
			: base(message, engineMode, innerException)
		{ }
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsScriptInterruptedException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		private JsScriptInterruptedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif
	}
}
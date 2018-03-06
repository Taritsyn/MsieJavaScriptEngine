﻿using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// The exception that is thrown during the script execution
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public class JsRuntimeException : JsScriptException
	{
		/// <summary>
		/// String representation of the script call stack
		/// </summary>
		private string _callStack = string.Empty;

		/// <summary>
		/// Gets or sets a string representation of the script call stack
		/// </summary>
		public string CallStack
		{
			get { return _callStack; }
			set { _callStack = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="JsRuntimeException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsRuntimeException(string message)
			: base(message)
		{
			Category = JsErrorCategory.Runtime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsRuntimeException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsRuntimeException(string message, Exception innerException)
			: base(message, innerException)
		{
			Category = JsErrorCategory.Runtime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsRuntimeException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		public JsRuntimeException(string message, string engineMode)
			: base(message, engineMode)
		{
			Category = JsErrorCategory.Runtime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsRuntimeException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsRuntimeException(string message, string engineMode, Exception innerException)
			: base(message, engineMode, innerException)
		{
			Category = JsErrorCategory.Runtime;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsRuntimeException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		protected JsRuntimeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_callStack = info.GetString("CallStack");
			}
		}


		#region JsException overrides

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data</param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			base.GetObjectData(info, context);
			info.AddValue("CallStack", _callStack);
		}

		#endregion
#endif
	}
}
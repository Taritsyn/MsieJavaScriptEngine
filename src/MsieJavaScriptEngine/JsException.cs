using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// The exception that is thrown during the work of JS engine
	/// </summary>
#if !NETSTANDARD1_3
	[Serializable]
#endif
	public class JsException : Exception
	{
		/// <summary>
		/// JS engine mode
		/// </summary>
		private readonly string _engineMode = string.Empty;

		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		public string EngineMode
		{
			get { return _engineMode; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		public JsException(string message, string engineMode)
			: this(message, engineMode, null)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsException(string message, string engineMode, Exception innerException)
			: base(message, innerException)
		{
			_engineMode = engineMode;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Initializes a new instance of the <see cref="JsException"/> class with serialized data
		/// </summary>
		/// <param name="info">The object that holds the serialized data</param>
		/// <param name="context">The contextual information about the source or destination</param>
		protected JsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_engineMode = info.GetString("EngineMode");
			}
		}


		#region Exception overrides

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
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);
			info.AddValue("EngineMode", _engineMode);
		}

		#endregion
#endif
	}
}
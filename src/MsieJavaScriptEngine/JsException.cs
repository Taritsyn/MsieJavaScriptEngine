﻿using System;
using System.Text;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

using AdvancedStringBuilder;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;

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
		/// Name of JS engine mode
		/// </summary>
		private readonly string _engineMode = string.Empty;

		/// <summary>
		/// Error category
		/// </summary>
		private string _category = JsErrorCategory.Unknown;

		/// <summary>
		/// Description of error
		/// </summary>
		private string _description = string.Empty;

		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		public string EngineMode
		{
			get { return _engineMode; }
		}

		/// <summary>
		/// Gets or sets a error category
		/// </summary>
		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		/// <summary>
		/// Gets or sets a description of error
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
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
				_category = info.GetString("Category");
				_description = info.GetString("Description");
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
				throw new ArgumentNullException(nameof(info));
			}

			base.GetObjectData(info, context);
			info.AddValue("EngineMode", _engineMode);
			info.AddValue("Category", _category);
			info.AddValue("Description", _description);
		}

		#endregion
#endif

		#region Object overrides

		/// <summary>
		/// Returns a string that represents the current exception
		/// </summary>
		/// <returns>A string that represents the current exception</returns>
		public override string ToString()
		{
			string errorDetails = JsErrorHelpers.GenerateErrorDetails(this, true);

			var stringBuilderPool = StringBuilderPool.Shared;
			StringBuilder resultBuilder = stringBuilderPool.Rent();
			resultBuilder.Append(this.GetType().FullName);
			resultBuilder.Append(": ");
			resultBuilder.Append(this.Message);

			if (errorDetails.Length > 0)
			{
				resultBuilder.AppendLine();
				resultBuilder.AppendLine();
				resultBuilder.Append(errorDetails);
			}

			if (this.InnerException != null)
			{
				resultBuilder.Append(" ---> ");
				resultBuilder.Append(this.InnerException.ToString());
			}

			if (this.StackTrace != null)
			{
				resultBuilder.AppendLine();
				resultBuilder.AppendLine(this.StackTrace);
			}

			string result = resultBuilder.ToString();
			stringBuilderPool.Return(resultBuilder);

			return result;
		}

		#endregion
	}
}
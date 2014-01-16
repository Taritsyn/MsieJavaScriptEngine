namespace MsieJavaScriptEngine.JsRt
{
	using System;
	using System.Globalization;
	using System.Text;

	using Constants;
	using Resources;

	/// <summary>
	/// JsRT version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraJsRtJsEngine : IInnerJsEngine
	{
		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		const string ENGINE_MODE_NAME = JsEngineModeName.ChakraJsRt;

		/// <summary>
		/// Lowest supported version of Internet Explorer
		/// </summary>
		const string LOWER_IE_VERSION = "11";

		/// <summary>
		/// Instance of JavaScript runtime
		/// </summary>
		private JavaScriptRuntime _jsRuntime;

		/// <summary>
		/// Instance of JavaScript context
		/// </summary>
		private readonly JavaScriptContext _jsContext;

		/// <summary>
		/// Synchronizer of code execution
		/// </summary>
		private readonly object _executionSynchronizer = new object();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Constructs instance of the Chakra JsRT JavaScript engine
		/// </summary>
		public ChakraJsRtJsEngine()
		{
			try
			{
				_jsRuntime = CreateJsRuntime();
				_jsContext = _jsRuntime.CreateContext();
			}
			catch (JavaScriptUsageException e)
			{
				string errorMessage;
				if (e.ErrorCode == JavaScriptErrorCode.WrongThread)
				{
					errorMessage = Strings.Runtime_JsEnginesChakraJsRtAndActiveScriptConflict;
				}
				else
				{
					errorMessage = string.Format(Strings.Runtime_JsEngineNotLoaded,
						ENGINE_MODE_NAME, LOWER_IE_VERSION, e.Message);
				}

				throw new JsEngineLoadException(errorMessage, ENGINE_MODE_NAME);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(Strings.Runtime_JsEngineNotLoaded,
						ENGINE_MODE_NAME, LOWER_IE_VERSION, e.Message), ENGINE_MODE_NAME);
			}
		}

		/// <summary>
		/// Destructs instance of the Chakra JsRT JavaScript engine
		/// </summary>
		~ChakraJsRtJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Creates a instance of JavaScript runtime with special settings
		/// </summary>
		/// <returns>Instance of JavaScript runtime with special settings</returns>
		private static JavaScriptRuntime CreateJsRuntime()
		{
			var jsRuntime = JavaScriptRuntime.Create(JavaScriptRuntimeAttributes.AllowScriptInterrupt, 
				JavaScriptRuntimeVersion.VersionEdge, null);

			return jsRuntime;
		}

		/// <summary>
		/// Checks a support of the Chakra JsRT JavaScript engine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported;
			
			try
			{
				using (CreateJsRuntime())
				{
					isSupported = true;
				}
			}
			catch
			{
				isSupported = false;
			}

			return isSupported;
		}

		/// <summary>
		/// Executes a mapping from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static JavaScriptValue MapToScriptType(object value)
		{
			if (value == null)
			{
				return JavaScriptValue.Null;
			}

			if (value is Undefined)
			{
				return JavaScriptValue.Undefined;
			}

			if (value is bool)
			{
				var processedValue = (bool)value;
				return JavaScriptValue.FromBoolean(processedValue);
			}

			if (value is int)
			{
				var processedValue = (int)value;
				return JavaScriptValue.FromInt32(processedValue);
			}

			if (value is double)
			{
				var processedValue = (double)value;
				return JavaScriptValue.FromDouble(processedValue);
			}

			if (value is string)
			{
				var processedValue = (string)value;
				return JavaScriptValue.FromString(processedValue);
			}

			return JavaScriptValue.FromObject(value);
		}

		/// <summary>
		/// Executes a mapping from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToHostType(JavaScriptValue value)
		{
			JavaScriptValueType valueType = value.ValueType;
			JavaScriptValue processedValue;
			object result;

			switch (valueType)
			{
				case JavaScriptValueType.Null:
					result = null;
					break;
				case JavaScriptValueType.Undefined:
					result = Undefined.Value;
					break;
				case JavaScriptValueType.Boolean:
					processedValue = value.ConvertToBoolean();
					result = processedValue.ToBoolean();
					break;
				case JavaScriptValueType.Number:
					processedValue = value.ConvertToNumber();
					result = processedValue.ToDouble();
					break;
				case JavaScriptValueType.String:
					processedValue = value.ConvertToString();
					result = processedValue.ToString();
					break;
				case JavaScriptValueType.Object:
				case JavaScriptValueType.Function:
				case JavaScriptValueType.Error:
				case JavaScriptValueType.Array:
					processedValue = value.ConvertToObject();
					result = processedValue.ToObject();
					break;
				default:
					throw new Exception();
			}

			return result;
		}

		private JsRuntimeException ConvertJavaScriptExceptionToJsRuntimeException(
			JavaScriptException jsException)
		{
			string message = jsException.Message;
			string category = string.Empty;

			if (jsException is JavaScriptUsageException)
			{
				category = "Usage error";
			}
			else if (jsException is JavaScriptEngineException)
			{
				category = "Engine error";
			}
			else if (jsException is JavaScriptFatalException)
			{
				category = "Fatal error";
			}
			else if (jsException is JavaScriptScriptException)
			{
				category = "Script error";

				var jsScriptException = (JavaScriptScriptException)jsException;
				JavaScriptPropertyId messageName = JavaScriptPropertyId.FromString("message");
				JavaScriptValue messageValue = jsScriptException.Error.GetProperty(messageName);

				string scriptMessage = messageValue.ToString();
				if (!string.IsNullOrWhiteSpace(message))
				{
					message = string.Format("{0}: {1}", message.TrimEnd('.'), scriptMessage);
				}
			}

			var jsEngineException = new JsRuntimeException(message, ENGINE_MODE_NAME)
			{
				Category = category,
				ErrorCode = ((uint)jsException.ErrorCode).ToString(CultureInfo.InvariantCulture),
				HelpLink = jsException.HelpLink
			};

			return jsEngineException;
		}

		private bool InnerHasProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId)
		{
			bool propertyExist;

			try
			{
				propertyExist = obj.HasProperty(propertyId);
			}
			catch (JavaScriptException e)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(e);
			}

			if (propertyExist)
			{
				JavaScriptValue propertyValue = InnerGetProperty(obj, propertyId);
				propertyExist = (propertyValue.ValueType != JavaScriptValueType.Undefined);
			}

			return propertyExist;
		}

		private JavaScriptValue InnerGetProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId)
		{
			JavaScriptValue propertyValue;

			try
			{
				propertyValue = obj.GetProperty(propertyId);
			}
			catch (JavaScriptException e)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(e);
			}

			return propertyValue;
		}

		private void InnerSetProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, 
			JavaScriptValue propertyValue)
		{
			try
			{
				obj.SetProperty(propertyId, propertyValue, true);
			}
			catch (JavaScriptException e)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(e);
			}
		}

		private void InnerDeleteProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId)
		{
			try
			{
				obj.DeleteProperty(propertyId, true);
			}
			catch (JavaScriptException e)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(e);
			}
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				_jsRuntime.Dispose();
			}
		}

		#region IInnerJsEngine implementation

		public string Mode
		{
			get { return ENGINE_MODE_NAME; }
		}

		public object Evaluate(string expression)
		{
			object result;

			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue resultValue;

				try
				{
					resultValue = JavaScriptContext.RunScript(expression);
				}
				catch (JavaScriptException e)
				{
					throw ConvertJavaScriptExceptionToJsRuntimeException(e);
				}

				result = MapToHostType(resultValue);
			}

			return result;
		}

		public void Execute(string code)
		{
			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				try
				{
					JavaScriptContext.RunScript(code);
				}
				catch (JavaScriptException e)
				{
					throw ConvertJavaScriptExceptionToJsRuntimeException(e);
				}
			}
		}

		public object CallFunction(string functionName, params object[] args)
		{
			string serializedArguments = string.Empty;
			int argumentCount = args.Length;

			if (argumentCount == 1)
			{
				object value = args[0];
				serializedArguments = JsTypeConverter.Serialize(value);
			}
			else if (argumentCount > 1)
			{
				var serializedArgumentsBuilder = new StringBuilder();

				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object value = args[argumentIndex];
					string serializedValue = JsTypeConverter.Serialize(value);

					if (argumentIndex > 0)
					{
						serializedArgumentsBuilder.Append(", ");
					}
					serializedArgumentsBuilder.Append(serializedValue);
				}

				serializedArguments = serializedArgumentsBuilder.ToString();
			}

			object result = Evaluate(string.Format("{0}({1});", functionName, serializedArguments));

			return result;
		}

		public bool HasVariable(string variableName)
		{
			bool variableExist;

			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				variableExist = InnerHasProperty(JavaScriptValue.GlobalObject, variableId);
			}

			return variableExist;
		}

		public object GetVariableValue(string variableName)
		{
			object result;

			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				JavaScriptValue variableValue = InnerGetProperty(JavaScriptValue.GlobalObject, variableId);

				result = MapToHostType(variableValue);
			}

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				if (value is Undefined)
				{
					InnerDeleteProperty(globalObj, variableId);
				}
				else
				{
					JavaScriptValue inputValue = MapToScriptType(value);
					InnerSetProperty(globalObj, variableId, inputValue);
				}
			}
		}

		public void RemoveVariable(string variableName)
		{
			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				bool variableExist = InnerHasProperty(globalObj, variableId);

				if (variableExist)
				{
					InnerDeleteProperty(globalObj, variableId);
				}
			}
		}

		public bool HasProperty(string variableName, string propertyName)
		{
			bool propertyExist;

			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				bool variableExist = InnerHasProperty(globalObj, variableId);
				if (!variableExist)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_VariableNotExist, variableName));
				}

				JavaScriptValue variableValue = InnerGetProperty(globalObj, variableId);

				if (propertyName.IndexOf('.') != -1)
				{
					string[] propertyPartNames = propertyName.Split('.');
					int propertyPartCount = propertyPartNames.Length;
					JavaScriptValue parentObj = variableValue;
					bool propertyPartExist = false;

					for (int propertyPartIndex = 0; propertyPartIndex < propertyPartCount; propertyPartIndex++)
					{
						string propertyPartName = propertyPartNames[propertyPartIndex];
						JavaScriptPropertyId propertyPartId = JavaScriptPropertyId.FromString(propertyPartName);
						propertyPartExist = InnerHasProperty(parentObj, propertyPartId);

						if (propertyPartExist)
						{
							JavaScriptValue propertyPartValue = InnerGetProperty(parentObj, propertyPartId);
							parentObj = propertyPartValue;
						}
						else
						{
							return false;
						}
					}

					propertyExist = propertyPartExist;
				}
				else
				{
					JavaScriptPropertyId propertyId = JavaScriptPropertyId.FromString(propertyName);
					propertyExist = InnerHasProperty(variableValue, propertyId);
				}
			}

			return propertyExist;
		}

		public object GetPropertyValue(string variableName, string propertyName)
		{
			object result;

			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				bool variableExist = InnerHasProperty(globalObj, variableId);
				if (!variableExist)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_VariableNotExist, variableName));
				}

				JavaScriptValue variableValue = InnerGetProperty(globalObj, variableId);
				JavaScriptValue propertyValue = JavaScriptValue.Undefined;

				if (propertyName.IndexOf('.') != -1)
				{
					string[] propertyPartNames = propertyName.Split('.');
					int propertyPartCount = propertyPartNames.Length;
					int lastpPopertyPartIndex = propertyPartCount - 1;
					var propertyPathBuilder = new StringBuilder();
					JavaScriptValue parentObj = variableValue;

					for (int propertyPartIndex = 0; propertyPartIndex < propertyPartCount; propertyPartIndex++)
					{
						string propertyPartName = propertyPartNames[propertyPartIndex];
						JavaScriptPropertyId propertyPartId = JavaScriptPropertyId.FromString(propertyPartName);

						if (propertyPartIndex > 0)
						{
							propertyPathBuilder.Append(".");
						}
						propertyPathBuilder.Append(propertyPartName);

						if (propertyPartIndex == lastpPopertyPartIndex)
						{
							propertyValue = InnerGetProperty(parentObj, propertyPartId);
							break;
						}

						bool propertyPartExist = InnerHasProperty(parentObj, propertyPartId);
						if (propertyPartExist)
						{
							JavaScriptValue propertyPartValue = InnerGetProperty(parentObj, propertyPartId);
							parentObj = propertyPartValue;
						}
						else
						{
							throw new JsRuntimeException(
								string.Format(Strings.Runtime_PropertyNotExist, 
									variableName, propertyPathBuilder));
						}
					}

					propertyPathBuilder.Clear();
				}
				else
				{
					JavaScriptPropertyId propertyId = JavaScriptPropertyId.FromString(propertyName);
					propertyValue = InnerGetProperty(variableValue, propertyId);
				}

				result = MapToHostType(propertyValue);
			}

			return result;
		}

		public void SetPropertyValue(string variableName, string propertyName, object value)
		{
			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				bool variableExist = InnerHasProperty(globalObj, variableId);
				JavaScriptValue variableValue;

				if (variableExist)
				{
					variableValue = InnerGetProperty(globalObj, variableId);
				}
				else
				{
					variableValue = JavaScriptValue.CreateObject();
					InnerSetProperty(globalObj, variableId, variableValue);
				}

				if (propertyName.IndexOf('.') != -1)
				{
					string[] propertyPartNames = propertyName.Split('.');
					int propertyPartCount = propertyPartNames.Length;
					int lastpPopertyPartIndex = propertyPartCount - 1;
					JavaScriptValue parentObject = variableValue;

					for (int propertyPartIndex = 0; propertyPartIndex < propertyPartCount; propertyPartIndex++)
					{
						string propertyPartName = propertyPartNames[propertyPartIndex];
						JavaScriptPropertyId propertyPartId = JavaScriptPropertyId.FromString(propertyPartName);
							
						if (propertyPartIndex == lastpPopertyPartIndex)
						{
							JavaScriptValue propertyValue = MapToScriptType(value);
							InnerSetProperty(parentObject, propertyPartId, propertyValue);
							break;
						}

						bool propertyPartExist = InnerHasProperty(parentObject, propertyPartId);
						JavaScriptValue propertyPartValue;

						if (propertyPartExist)
						{
							propertyPartValue = InnerGetProperty(parentObject, propertyPartId);
						}
						else
						{
							propertyPartValue = JavaScriptValue.CreateObject();
							InnerSetProperty(parentObject, propertyPartId, propertyPartValue);
						}

						parentObject = propertyPartValue;
					}
				}
				else
				{
					JavaScriptPropertyId propertyId = JavaScriptPropertyId.FromString(propertyName);
					JavaScriptValue propertyValue = MapToScriptType(value);

					InnerSetProperty(variableValue, propertyId, propertyValue);
				}
			}
		}

		public void RemoveProperty(string variableName, string propertyName)
		{
			lock (_executionSynchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				bool variableExist = InnerHasProperty(globalObj, variableId);
				if (!variableExist)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_VariableNotExist, variableName));
				}

				JavaScriptValue variableValue = InnerGetProperty(globalObj, variableId);

				if (propertyName.IndexOf('.') != -1)
				{
					string[] propertyPartNames = propertyName.Split('.');
					int propertyPartCount = propertyPartNames.Length;
					int lastpPopertyPartIndex = propertyPartCount - 1;
					var propertyPathBuilder = new StringBuilder();
					JavaScriptValue parentObject = variableValue;
						
					for (int propertyPartIndex = 0; propertyPartIndex < propertyPartCount; propertyPartIndex++)
					{
						string propertyPartName = propertyPartNames[propertyPartIndex];
						JavaScriptPropertyId propertyPartId = JavaScriptPropertyId.FromString(propertyPartName);

						if (propertyPartIndex > 0)
						{
							propertyPathBuilder.Append(".");
						}
						propertyPathBuilder.Append(propertyPartName);

						bool propertyPartExist = InnerHasProperty(parentObject, propertyPartId);

						if (propertyPartIndex == lastpPopertyPartIndex)
						{
							if (propertyPartExist)
							{
								InnerDeleteProperty(parentObject, propertyPartId);
							}

							break;
						}

						if (propertyPartExist)
						{
							JavaScriptValue propertyPartValue = InnerGetProperty(parentObject, propertyPartId);
							parentObject = propertyPartValue;
						}
						else
						{
							throw new JsRuntimeException(
								string.Format(Strings.Runtime_PropertyNotExist, 
									variableName, propertyPathBuilder));
						}
					}

					propertyPathBuilder.Clear();
				}
				else
				{
					JavaScriptPropertyId propertyId = JavaScriptPropertyId.FromString(propertyName);
					bool propertyExist = InnerHasProperty(variableValue, propertyId);

					if (propertyExist)
					{
						InnerDeleteProperty(variableValue, propertyId);
					}
				}
			}
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
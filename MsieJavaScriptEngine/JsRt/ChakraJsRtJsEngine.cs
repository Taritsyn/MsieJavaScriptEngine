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
		/// Synchronizer
		/// </summary>
		private readonly object _synchronizer = new object();

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

			var typeCode = Type.GetTypeCode(value.GetType());

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return JavaScriptValue.FromBoolean((bool)value);
				case TypeCode.Int32:
					return JavaScriptValue.FromInt32((int)value);
				case TypeCode.Double:
					return JavaScriptValue.FromDouble((double)value);
				case TypeCode.String:
					return JavaScriptValue.FromString((string)value);
				default:
					return JavaScriptValue.FromObject(value);
			}
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
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}

		private JsRuntimeException ConvertJavaScriptExceptionToJsRuntimeException(
			JavaScriptException jsException)
		{
			string message = jsException.Message;
			string category = string.Empty;
			int lineNumber = 0;
			int columnNumber = 0;
			string sourceFragment = string.Empty;

			if (jsException is JavaScriptScriptException)
			{
				category = "Script error";

				var jsScriptException = (JavaScriptScriptException)jsException;
				JavaScriptValue errorValue = jsScriptException.Error;

				JavaScriptPropertyId messagePropertyId = JavaScriptPropertyId.FromString("message");
				JavaScriptValue messagePropertyValue = errorValue.GetProperty(messagePropertyId);
				string scriptMessage = messagePropertyValue.ConvertToString().ToString();
				if (!string.IsNullOrWhiteSpace(scriptMessage))
				{
					message = string.Format("{0}: {1}", message.TrimEnd('.'), scriptMessage);
				}

				JavaScriptPropertyId linePropertyId = JavaScriptPropertyId.FromString("line");
				if (errorValue.HasProperty(linePropertyId))
				{
					JavaScriptValue linePropertyValue = errorValue.GetProperty(linePropertyId);
					lineNumber = (int)linePropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				JavaScriptPropertyId columnPropertyId = JavaScriptPropertyId.FromString("column");
				if (errorValue.HasProperty(columnPropertyId))
				{
					JavaScriptValue columnPropertyValue = errorValue.GetProperty(columnPropertyId);
					columnNumber = (int)columnPropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				JavaScriptPropertyId sourcePropertyId = JavaScriptPropertyId.FromString("source");
				if (errorValue.HasProperty(sourcePropertyId))
				{
					JavaScriptValue sourcePropertyValue = errorValue.GetProperty(sourcePropertyId);
					sourceFragment = sourcePropertyValue.ConvertToString().ToString();
				}
			}
			else if (jsException is JavaScriptUsageException)
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

			var jsEngineException = new JsRuntimeException(message, ENGINE_MODE_NAME)
			{
				ErrorCode = ((uint)jsException.ErrorCode).ToString(CultureInfo.InvariantCulture),
				Category = category,
				LineNumber = lineNumber,
				ColumnNumber = columnNumber,
				SourceFragment = sourceFragment,
				HelpLink = jsException.HelpLink
			};

			return jsEngineException;
		}

		private void InvokeScript(Action action)
		{
			lock (_synchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				try
				{
					action();
				}
				catch (JavaScriptException e)
				{
					throw ConvertJavaScriptExceptionToJsRuntimeException(e);
				}
			}
		}

		private T InvokeScript<T>(Func<T> func)
		{
			lock (_synchronizer)
			using (new JavaScriptContext.Scope(_jsContext))
			{
				try
				{
					return func();
				}
				catch (JavaScriptException e)
				{
					throw ConvertJavaScriptExceptionToJsRuntimeException(e);
				}
			}
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			lock (_synchronizer)
			{
				if (!_disposed)
				{
					_disposed = true;

					_jsRuntime.Dispose();
				}
			}
		}

		#region IInnerJsEngine implementation

		public string Mode
		{
			get { return ENGINE_MODE_NAME; }
		}

		public object Evaluate(string expression)
		{
			object result = InvokeScript(() =>
			{
				JavaScriptValue resultValue = JavaScriptContext.RunScript(expression);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public void Execute(string code)
		{
			InvokeScript(() => JavaScriptContext.RunScript(code));
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
			bool result = InvokeScript(() =>
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				bool variableExist = globalObj.HasProperty(variableId);

				if (variableExist)
				{
					JavaScriptValue variableValue = globalObj.GetProperty(variableId);
					variableExist = (variableValue.ValueType != JavaScriptValueType.Undefined);
				}

				return variableExist;
			});

			return result;
		}

		public object GetVariableValue(string variableName)
		{
			object result = InvokeScript(() =>
			{
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				JavaScriptValue variableValue = JavaScriptValue.GlobalObject.GetProperty(variableId);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);
				JavaScriptValue inputValue = MapToScriptType(value);

				JavaScriptValue.GlobalObject.SetProperty(variableId, inputValue, true);
			});
		}

		public void RemoveVariable(string variableName)
		{
			InvokeScript(() =>
			{
				JavaScriptValue globalObj = JavaScriptValue.GlobalObject;
				JavaScriptPropertyId variableId = JavaScriptPropertyId.FromString(variableName);

				if (globalObj.HasProperty(variableId))
				{
					globalObj.SetProperty(variableId, JavaScriptValue.Undefined, true);
				}
			});
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
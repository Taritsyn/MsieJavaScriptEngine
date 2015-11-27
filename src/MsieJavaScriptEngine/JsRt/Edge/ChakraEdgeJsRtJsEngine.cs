namespace MsieJavaScriptEngine.JsRt.Edge
{
	using System;
	using System.Globalization;
	using System.Text;

	using Constants;
	using Resources;
	using Utilities;

	/// <summary>
	/// “Edge” JsRT version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraEdgeJsRtJsEngine : IInnerJsEngine
	{
		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		const string ENGINE_MODE_NAME = JsEngineModeName.ChakraEdgeJsRt;

		/// <summary>
		/// Instance of JavaScript runtime
		/// </summary>
		private EdgeJsRuntime _jsRuntime;

		/// <summary>
		/// Instance of JavaScript context
		/// </summary>
		private readonly EdgeJsContext _jsContext;

		/// <summary>
		/// Synchronizer
		/// </summary>
		private readonly object _synchronizer = new object();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Constructs instance of the Chakra “Edge” JsRT JavaScript engine
		/// </summary>
		public ChakraEdgeJsRtJsEngine()
		{
			try
			{
				_jsRuntime = CreateJsRuntime();
				_jsContext = _jsRuntime.CreateContext();
			}
			catch (JsUsageException e)
			{
				string errorMessage;
				if (e.ErrorCode == JsErrorCode.WrongThread)
				{
					errorMessage = Strings.Runtime_JsEnginesChakraJsRtAndActiveScriptConflict;
				}
				else
				{
					errorMessage = string.Format(Strings.Runtime_EdgeJsEngineNotLoaded, e.Message);
				}

				throw new JsEngineLoadException(errorMessage, ENGINE_MODE_NAME);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(Strings.Runtime_EdgeJsEngineNotLoaded, e.Message), ENGINE_MODE_NAME);
			}
		}

		/// <summary>
		/// Destructs instance of the Chakra “Edge” JsRT JavaScript engine
		/// </summary>
		~ChakraEdgeJsRtJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Creates a instance of JavaScript runtime with special settings
		/// </summary>
		/// <returns>Instance of JavaScript runtime with special settings</returns>
		private static EdgeJsRuntime CreateJsRuntime()
		{
			var jsRuntime = EdgeJsRuntime.Create(JsRuntimeAttributes.AllowScriptInterrupt, null);

			return jsRuntime;
		}

		/// <summary>
		/// Checks a support of the Chakra “Edge” JsRT JavaScript engine
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
		private static EdgeJsValue MapToScriptType(object value)
		{
			if (value == null)
			{
				return EdgeJsValue.Null;
			}

			if (value is Undefined)
			{
				return EdgeJsValue.Undefined;
			}

			var typeCode = Type.GetTypeCode(value.GetType());

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return EdgeJsValue.FromBoolean((bool)value);
				case TypeCode.Int32:
					return EdgeJsValue.FromInt32((int)value);
				case TypeCode.Double:
					return EdgeJsValue.FromDouble((double)value);
				case TypeCode.String:
					return EdgeJsValue.FromString((string)value);
				default:
					return EdgeJsValue.FromObject(value);
			}
		}

		/// <summary>
		/// Executes a mapping from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToHostType(EdgeJsValue value)
		{
			JsValueType valueType = value.ValueType;
			EdgeJsValue processedValue;
			object result;

			switch (valueType)
			{
				case JsValueType.Null:
					result = null;
					break;
				case JsValueType.Undefined:
					result = Undefined.Value;
					break;
				case JsValueType.Boolean:
					processedValue = value.ConvertToBoolean();
					result = processedValue.ToBoolean();
					break;
				case JsValueType.Number:
					processedValue = value.ConvertToNumber();
					result = processedValue.ToDouble();
					break;
				case JsValueType.String:
					processedValue = value.ConvertToString();
					result = processedValue.ToString();
					break;
				case JsValueType.Object:
				case JsValueType.Function:
				case JsValueType.Error:
				case JsValueType.Array:
					processedValue = value.ConvertToObject();
					result = processedValue.ToObject();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}

		private JsRuntimeException ConvertJsExceptionToJsRuntimeException(
			JsException jsException)
		{
			string message = jsException.Message;
			string category = string.Empty;
			int lineNumber = 0;
			int columnNumber = 0;
			string sourceFragment = string.Empty;

			var jsScriptException = jsException as EdgeJsScriptException;
			if (jsScriptException != null)
			{
				category = "Script error";
				EdgeJsValue errorValue = jsScriptException.Error;

				EdgeJsPropertyId messagePropertyId = EdgeJsPropertyId.FromString("message");
				EdgeJsValue messagePropertyValue = errorValue.GetProperty(messagePropertyId);
				string scriptMessage = messagePropertyValue.ConvertToString().ToString();
				if (!string.IsNullOrWhiteSpace(scriptMessage))
				{
					message = string.Format("{0}: {1}", message.TrimEnd('.'), scriptMessage);
				}

				EdgeJsPropertyId linePropertyId = EdgeJsPropertyId.FromString("line");
				if (errorValue.HasProperty(linePropertyId))
				{
					EdgeJsValue linePropertyValue = errorValue.GetProperty(linePropertyId);
					lineNumber = (int)linePropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				EdgeJsPropertyId columnPropertyId = EdgeJsPropertyId.FromString("column");
				if (errorValue.HasProperty(columnPropertyId))
				{
					EdgeJsValue columnPropertyValue = errorValue.GetProperty(columnPropertyId);
					columnNumber = (int)columnPropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				EdgeJsPropertyId sourcePropertyId = EdgeJsPropertyId.FromString("source");
				if (errorValue.HasProperty(sourcePropertyId))
				{
					EdgeJsValue sourcePropertyValue = errorValue.GetProperty(sourcePropertyId);
					sourceFragment = sourcePropertyValue.ConvertToString().ToString();
				}
			}
			else if (jsException is JsUsageException)
			{
				category = "Usage error";
			}
			else if (jsException is JsEngineException)
			{
				category = "Engine error";
			}
			else if (jsException is JsFatalException)
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
			using (new EdgeJsScope(_jsContext))
			{
				try
				{
					action();
				}
				catch (JsException e)
				{
					throw ConvertJsExceptionToJsRuntimeException(e);
				}
			}
		}

		private T InvokeScript<T>(Func<T> func)
		{
			lock (_synchronizer)
			using (new EdgeJsScope(_jsContext))
			{
				try
				{
					return func();
				}
				catch (JsException e)
				{
					throw ConvertJsExceptionToJsRuntimeException(e);
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
				EdgeJsValue resultValue = EdgeJsContext.RunScript(expression);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public void Execute(string code)
		{
			InvokeScript(() => EdgeJsContext.RunScript(code));
		}

		public object CallFunction(string functionName, params object[] args)
		{
			string serializedArguments = string.Empty;
			int argumentCount = args.Length;

			if (argumentCount == 1)
			{
				object value = args[0];
				serializedArguments = SimplisticJsSerializer.Serialize(value);
			}
			else if (argumentCount > 1)
			{
				var serializedArgumentsBuilder = new StringBuilder();

				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object value = args[argumentIndex];
					string serializedValue = SimplisticJsSerializer.Serialize(value);

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
				EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
				bool variableExist = globalObj.HasProperty(variableId);

				if (variableExist)
				{
					EdgeJsValue variableValue = globalObj.GetProperty(variableId);
					variableExist = (variableValue.ValueType != JsValueType.Undefined);
				}

				return variableExist;
			});

			return result;
		}

		public object GetVariableValue(string variableName)
		{
			object result = InvokeScript(() =>
			{
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
				EdgeJsValue variableValue = EdgeJsValue.GlobalObject.GetProperty(variableId);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
				EdgeJsValue inputValue = MapToScriptType(value);

				EdgeJsValue.GlobalObject.SetProperty(variableId, inputValue, true);
			});
		}

		public void RemoveVariable(string variableName)
		{
			InvokeScript(() =>
			{
				EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);

				if (globalObj.HasProperty(variableId))
				{
					globalObj.SetProperty(variableId, EdgeJsValue.Undefined, true);
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
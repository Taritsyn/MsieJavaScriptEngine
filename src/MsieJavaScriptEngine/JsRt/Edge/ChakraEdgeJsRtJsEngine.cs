namespace MsieJavaScriptEngine.JsRt.Edge
{
	using System;
	using System.Globalization;
	using System.Linq;

	using Constants;
	using Resources;
	using Utilities;

	/// <summary>
	/// “Edge” JsRT version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraEdgeJsRtJsEngine : ChakraJsRtJsEngineBase
	{
		/// <summary>
		/// Instance of JavaScript runtime
		/// </summary>
		private EdgeJsRuntime _jsRuntime;

		/// <summary>
		/// Instance of JavaScript context
		/// </summary>
		private readonly EdgeJsContext _jsContext;

		/// <summary>
		/// Flag indicating whether this JavaScript engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static readonly object _supportSynchronizer = new object();

		/// <summary>
		/// Run synchronizer
		/// </summary>
		private readonly object _runSynchronizer = new object();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Constructs instance of the Chakra “Edge” JsRT JavaScript engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ChakraEdgeJsRtJsEngine(bool enableDebugging)
			: base(JsEngineMode.ChakraEdgeJsRt, enableDebugging)
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
					errorMessage = Strings.Runtime_JsEnginesConflictOnMachine;
				}
				else
				{
					errorMessage = string.Format(Strings.Runtime_EdgeJsEngineNotLoaded, e.Message);
				}

				throw new JsEngineLoadException(errorMessage, _engineModeName);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(Strings.Runtime_EdgeJsEngineNotLoaded, e.Message), _engineModeName);
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
			if (_isSupported.HasValue)
			{
				return _isSupported.Value;
			}

			lock (_supportSynchronizer)
			{
				if (_isSupported.HasValue)
				{
					return _isSupported.Value;
				}

				try
				{
					using (CreateJsRuntime())
					{
						_isSupported = true;
					}
				}
				catch (DllNotFoundException e)
				{
					if (e.Message.IndexOf("'" + DllName.Chakra + "'", StringComparison.OrdinalIgnoreCase) != -1)
					{
						_isSupported = false;
					}
					else
					{
						_isSupported = null;
					}
				}
				catch
				{
					_isSupported = null;
				}

				return _isSupported.HasValue && _isSupported.Value;
			}
		}

		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private EdgeJsValue MapToScriptType(object value)
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
					object processedValue = !TypeConverter.IsPrimitiveType(typeCode) ?
						new HostObject(value, _engineMode) : value;
					return EdgeJsValue.FromObject(processedValue);
			}
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private EdgeJsValue[] MapToScriptType(object[] args)
		{
			return args.Select(MapToScriptType).ToArray();
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private object MapToHostType(EdgeJsValue value)
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
					object obj = processedValue.ToObject();

					if (!TypeConverter.IsPrimitiveType(obj.GetType()))
					{
						var hostObj = obj as HostObject;
						result = hostObj != null ? hostObj.Target : obj;
					}
					else
					{
						result = obj;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}

		/// <summary>
		/// Makes a mapping of array items from the script type to a host type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToHostType(EdgeJsValue[] args)
		{
			return args.Select(MapToHostType).ToArray();
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

			var jsEngineException = new JsRuntimeException(message, _engineModeName)
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

		protected override void InnerStartDebugging()
		{
			EdgeJsContext.StartDebugging();
		}

		private void InvokeScript(Action action)
		{
			lock (_runSynchronizer)
			using (new EdgeJsScope(_jsContext))
			{
				if (_enableDebugging)
				{
					StartDebugging();
				}

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
			lock (_runSynchronizer)
			using (new EdgeJsScope(_jsContext))
			{
				if (_enableDebugging)
				{
					StartDebugging();
				}

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
			lock (_runSynchronizer)
			{
				if (!_disposed)
				{
					_disposed = true;

					_jsRuntime.Dispose();
				}
			}
		}

		#region IInnerJsEngine implementation

		public override string Mode
		{
			get { return _engineModeName; }
		}

		public override object Evaluate(string expression)
		{
			object result = InvokeScript(() =>
			{
				EdgeJsValue resultValue = EdgeJsContext.RunScript(expression);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public override void Execute(string code)
		{
			InvokeScript(() => EdgeJsContext.RunScript(code));
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object result = InvokeScript(() =>
			{
				EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
				EdgeJsPropertyId functionId = EdgeJsPropertyId.FromString(functionName);

				bool functionExist = globalObj.HasProperty(functionId);
				if (!functionExist)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_FunctionNotExist, functionName));
				}

				var processedArgs = MapToScriptType(args);
				var allProcessedArgs = new[] { globalObj }.Concat(processedArgs).ToArray();

				EdgeJsValue functionValue = globalObj.GetProperty(functionId);
				EdgeJsValue resultValue = functionValue.CallFunction(allProcessedArgs);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public override bool HasVariable(string variableName)
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

		public override object GetVariableValue(string variableName)
		{
			object result = InvokeScript(() =>
			{
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
				EdgeJsValue variableValue = EdgeJsValue.GlobalObject.GetProperty(variableId);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
				EdgeJsValue inputValue = MapToScriptType(value);

				EdgeJsValue.GlobalObject.SetProperty(variableId, inputValue, true);
			});
		}

		public override void RemoveVariable(string variableName)
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

		public override void EmbedHostObject(string itemName, object value)
		{
			InvokeScript(() =>
			{
				EdgeJsValue processedValue = MapToScriptType(value);
				EdgeJsPropertyId itemId = EdgeJsPropertyId.FromString(itemName);

				EdgeJsValue.GlobalObject.SetProperty(itemId, processedValue, true);
			});
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public override void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
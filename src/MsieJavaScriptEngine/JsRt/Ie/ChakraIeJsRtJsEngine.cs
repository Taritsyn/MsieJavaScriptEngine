namespace MsieJavaScriptEngine.JsRt.Ie
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	using Constants;
	using Helpers;
	using Resources;
	using Utilities;

	/// <summary>
	/// “IE” JsRT version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraIeJsRtJsEngine : IInnerJsEngine
	{
		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		private readonly JsEngineMode _engineMode;

		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		private readonly string _engineModeName;

		/// <summary>
		/// Lowest supported version of Internet Explorer
		/// </summary>
		const string LOWER_IE_VERSION = "11";

		/// <summary>
		/// Instance of JavaScript runtime
		/// </summary>
		private IeJsRuntime _jsRuntime;

		/// <summary>
		/// Instance of JavaScript context
		/// </summary>
		private readonly IeJsContext _jsContext;

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
		/// Constructs instance of the Chakra “IE” JsRT JavaScript engine
		/// </summary>
		public ChakraIeJsRtJsEngine()
		{
			_engineMode = JsEngineMode.ChakraIeJsRt;
			_engineModeName = JsEngineModeHelpers.GetModeName(_engineMode);

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
					errorMessage = string.Format(Strings.Runtime_IeJsEngineNotLoaded,
						_engineModeName, LOWER_IE_VERSION, e.Message);
				}

				throw new JsEngineLoadException(errorMessage, _engineModeName);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(Strings.Runtime_IeJsEngineNotLoaded,
						_engineModeName, LOWER_IE_VERSION, e.Message), _engineModeName);
			}
		}

		/// <summary>
		/// Destructs instance of the Chakra “IE” JsRT JavaScript engine
		/// </summary>
		~ChakraIeJsRtJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Creates a instance of JavaScript runtime with special settings
		/// </summary>
		/// <returns>Instance of JavaScript runtime with special settings</returns>
		private static IeJsRuntime CreateJsRuntime()
		{
			var jsRuntime = IeJsRuntime.Create(JsRuntimeAttributes.AllowScriptInterrupt,
				JsRuntimeVersion.VersionEdge, null);

			return jsRuntime;
		}

		/// <summary>
		/// Checks a support of the Chakra “IE” JsRT JavaScript engine
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
					if (e.Message.IndexOf("'" + DllName.JScript9 + "'", StringComparison.OrdinalIgnoreCase) != -1)
					{
						_isSupported = false;
					}
					else
					{
						_isSupported = null;
					}
				}
				catch (EntryPointNotFoundException e)
				{
					string message = e.Message;
					if (message.IndexOf("'" + DllName.JScript9 + "'", StringComparison.OrdinalIgnoreCase) != -1
						&& message.IndexOf("'JsCreateRuntime'", StringComparison.OrdinalIgnoreCase) != -1)
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
		private IeJsValue MapToScriptType(object value)
		{
			if (value == null)
			{
				return IeJsValue.Null;
			}

			if (value is Undefined)
			{
				return IeJsValue.Undefined;
			}

			var typeCode = Type.GetTypeCode(value.GetType());

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return IeJsValue.FromBoolean((bool)value);
				case TypeCode.Int32:
					return IeJsValue.FromInt32((int)value);
				case TypeCode.Double:
					return IeJsValue.FromDouble((double)value);
				case TypeCode.String:
					return IeJsValue.FromString((string)value);
				default:
					object processedValue = !TypeConverter.IsPrimitiveType(typeCode) ?
						new HostObject(value, _engineMode) : value;
					return IeJsValue.FromObject(processedValue);
			}
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private IeJsValue[] MapToScriptType(object[] args)
		{
			return args.Select(MapToScriptType).ToArray();
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private object MapToHostType(IeJsValue value)
		{
			JsValueType valueType = value.ValueType;
			IeJsValue processedValue;
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
		/// Makes a mapping of array itemp from the script type to a host type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToHostType(IeJsValue[] args)
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

			var jsScriptException = jsException as IeJsScriptException;
			if (jsScriptException != null)
			{
				category = "Script error";
				IeJsValue errorValue = jsScriptException.Error;

				IeJsPropertyId messagePropertyId = IeJsPropertyId.FromString("message");
				IeJsValue messagePropertyValue = errorValue.GetProperty(messagePropertyId);
				string scriptMessage = messagePropertyValue.ConvertToString().ToString();
				if (!string.IsNullOrWhiteSpace(scriptMessage))
				{
					message = string.Format("{0}: {1}", message.TrimEnd('.'), scriptMessage);
				}

				IeJsPropertyId linePropertyId = IeJsPropertyId.FromString("line");
				if (errorValue.HasProperty(linePropertyId))
				{
					IeJsValue linePropertyValue = errorValue.GetProperty(linePropertyId);
					lineNumber = (int)linePropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				IeJsPropertyId columnPropertyId = IeJsPropertyId.FromString("column");
				if (errorValue.HasProperty(columnPropertyId))
				{
					IeJsValue columnPropertyValue = errorValue.GetProperty(columnPropertyId);
					columnNumber = (int)columnPropertyValue.ConvertToNumber().ToDouble() + 1;
				}

				IeJsPropertyId sourcePropertyId = IeJsPropertyId.FromString("source");
				if (errorValue.HasProperty(sourcePropertyId))
				{
					IeJsValue sourcePropertyValue = errorValue.GetProperty(sourcePropertyId);
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

		private void InvokeScript(Action action)
		{
			lock (_runSynchronizer)
			using (new IeJsScope(_jsContext))
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
			lock (_runSynchronizer)
			using (new IeJsScope(_jsContext))
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

		public string Mode
		{
			get { return _engineModeName; }
		}

		public object Evaluate(string expression)
		{
			object result = InvokeScript(() =>
			{
				IeJsValue resultValue = IeJsContext.RunScript(expression);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public void Execute(string code)
		{
			InvokeScript(() => IeJsContext.RunScript(code));
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
				IeJsValue globalObj = IeJsValue.GlobalObject;
				IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);
				bool variableExist = globalObj.HasProperty(variableId);

				if (variableExist)
				{
					IeJsValue variableValue = globalObj.GetProperty(variableId);
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
				IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);
				IeJsValue variableValue = IeJsValue.GlobalObject.GetProperty(variableId);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);
				IeJsValue inputValue = MapToScriptType(value);

				IeJsValue.GlobalObject.SetProperty(variableId, inputValue, true);
			});
		}

		public void RemoveVariable(string variableName)
		{
			InvokeScript(() =>
			{
				IeJsValue globalObj = IeJsValue.GlobalObject;
				IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);

				if (globalObj.HasProperty(variableId))
				{
					globalObj.SetProperty(variableId, IeJsValue.Undefined, true);
				}
			});
		}

		public void EmbedHostObject(string itemName, object value)
		{
			InvokeScript(() =>
			{
				IeJsValue processedValue = MapToScriptType(value);
				IeJsPropertyId itemId = IeJsPropertyId.FromString(itemName);

				IeJsValue.GlobalObject.SetProperty(itemId, processedValue, true);
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
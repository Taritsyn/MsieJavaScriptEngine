using System;
using System.Text;

using AdvancedStringBuilder;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Extensions;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;

using WrapperCompilationException = MsieJavaScriptEngine.JsCompilationException;
using WrapperEngineException = MsieJavaScriptEngine.JsEngineException;
using WrapperEngineLoadException = MsieJavaScriptEngine.JsEngineLoadException;
using WrapperException = MsieJavaScriptEngine.JsException;
using WrapperFatalException = MsieJavaScriptEngine.JsFatalException;
using WrapperInterruptedException = MsieJavaScriptEngine.JsInterruptedException;
using WrapperRuntimeException = MsieJavaScriptEngine.JsRuntimeException;
using WrapperScriptException = MsieJavaScriptEngine.JsScriptException;
using WrapperUsageException = MsieJavaScriptEngine.JsUsageException;

using OriginalEngineException = MsieJavaScriptEngine.JsRt.JsEngineException;
using OriginalException = MsieJavaScriptEngine.JsRt.JsException;
using OriginalFatalException = MsieJavaScriptEngine.JsRt.JsFatalException;
using OriginalScriptException = MsieJavaScriptEngine.JsRt.Ie.IeJsScriptException;
using OriginalUsageException = MsieJavaScriptEngine.JsRt.JsUsageException;

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” JsRT version of Chakra JS engine
	/// </summary>
	internal sealed class ChakraIeJsRtJsEngine : ChakraJsRtJsEngineBase
	{
		/// <summary>
		/// Lowest supported version of Internet Explorer
		/// </summary>
		const string LOWER_IE_VERSION = "11";

		/// <summary>
		/// Instance of JS runtime
		/// </summary>
		private IeJsRuntime _jsRuntime;

		/// <summary>
		/// Instance of JS context
		/// </summary>
		private IeJsContext _jsContext;

		/// <summary>
		/// Type mapper
		/// </summary>
		private IeTypeMapper _typeMapper;

		/// <summary>
		/// Flag indicating whether this JS engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static readonly object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs an instance of the Chakra “IE” JsRT engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		public ChakraIeJsRtJsEngine(JsEngineSettings settings)
			: base(settings)
		{
			_typeMapper = new IeTypeMapper();

			try
			{
				_dispatcher.Invoke(() =>
				{
					_jsRuntime = CreateJsRuntime();
					_jsContext = _jsRuntime.CreateContext();
					if (_jsContext.IsValid)
					{
						_jsContext.AddRef();

						if (_settings.EnableDebugging)
						{
							using (new IeJsScope(_jsContext))
							{
								IeJsContext.StartDebugging();
							}
						}
					}
				});
			}
			catch (DllNotFoundException e)
			{
				throw WrapTypeLoadException(e);
			}
#if NETSTANDARD1_3
			catch (TypeLoadException e)
#else
			catch (EntryPointNotFoundException e)
#endif
			{
				throw WrapTypeLoadException(e);
			}
			catch (Exception e)
			{
				throw JsErrorHelpers.WrapEngineLoadException(e, _engineModeName, true);
			}
			finally
			{
				if (!_jsContext.IsValid)
				{
					Dispose();
				}
			}
		}

		/// <summary>
		/// Destructs an instance of the Chakra “IE” JsRT engine
		/// </summary>
		~ChakraIeJsRtJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Creates a instance of JS runtime with special settings
		/// </summary>
		/// <returns>Instance of JS runtime with special settings</returns>
		private static IeJsRuntime CreateJsRuntime()
		{
			return IeJsRuntime.Create(JsRuntimeAttributes.AllowScriptInterrupt, JsRuntimeVersion.VersionEdge, null);
		}

		/// <summary>
		/// Checks a support of the Chakra “IE” JsRT engine
		/// </summary>
		/// <returns>Result of check (<c>true</c> - supports; <c>false</c> - does not support)</returns>
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
					if (e.Message.ContainsQuotedValue(DllName.JScript9))
					{
						_isSupported = false;
					}
					else
					{
						_isSupported = null;
					}
				}
#if NETSTANDARD1_3
				catch (TypeLoadException e)
#else
				catch (EntryPointNotFoundException e)
#endif
				{
					string message = e.Message;
					if (message.ContainsQuotedValue(DllName.JScript9)
						&& message.ContainsQuotedValue("JsCreateRuntime"))
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
		/// Adds a reference to the value
		/// </summary>
		/// <param name="value">The value</param>
		private static void AddReferenceToValue(IeJsValue value)
		{
			if (CanHaveReferences(value))
			{
				value.AddRef();
			}
		}

		/// <summary>
		/// Removes a reference to the value
		/// </summary>
		/// <param name="value">The value</param>
		private static void RemoveReferenceToValue(IeJsValue value)
		{
			if (CanHaveReferences(value))
			{
				value.Release();
			}
		}

		/// <summary>
		/// Checks whether the value can have references
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>Result of check (<c>true</c> - may have; <c>false</c> - may not have)</returns>
		private static bool CanHaveReferences(IeJsValue value)
		{
			JsValueType valueType = value.ValueType;

			switch (valueType)
			{
				case JsValueType.Null:
				case JsValueType.Undefined:
				case JsValueType.Boolean:
					return false;
				default:
					return true;
			}
		}

		#region Mapping

		private WrapperException WrapJsException(OriginalException originalException,
			string defaultDocumentName = null)
		{
			WrapperException wrapperException;
			JsErrorCode errorCode = originalException.ErrorCode;
			string description = originalException.Message;
			string message = description;
			string type = string.Empty;
			string documentName = defaultDocumentName ?? string.Empty;
			int lineNumber = 0;
			int columnNumber = 0;
			string callStack = string.Empty;
			string sourceFragment = string.Empty;

			var originalScriptException = originalException as OriginalScriptException;
			if (originalScriptException != null)
			{
				IeJsValue errorValue = originalScriptException.Error;

				if (errorValue.IsValid)
				{
					JsValueType errorValueType = errorValue.ValueType;

					if (errorValueType == JsValueType.Error
						|| errorValue.ValueType == JsValueType.Object)
					{
						IeJsValue messagePropertyValue = errorValue.GetProperty("message");
						string localDescription = messagePropertyValue.ConvertToString().ToString();
						if (!string.IsNullOrWhiteSpace(localDescription))
						{
							description = localDescription;
						}

						IeJsValue namePropertyValue = errorValue.GetProperty("name");
						type = namePropertyValue.ValueType == JsValueType.String ?
							namePropertyValue.ToString() : string.Empty;

						IeJsPropertyId descriptionPropertyId = IeJsPropertyId.FromString("description");
						if (errorValue.HasProperty(descriptionPropertyId))
						{
							IeJsValue descriptionPropertyValue = errorValue.GetProperty(descriptionPropertyId);
							localDescription = descriptionPropertyValue.ConvertToString().ToString();
							if (!string.IsNullOrWhiteSpace(localDescription))
							{
								description = localDescription;
							}
						}

						if (type == JsErrorType.Syntax)
						{
							errorCode = JsErrorCode.ScriptCompile;
						}
						else
						{
							IeJsPropertyId numberPropertyId = IeJsPropertyId.FromString("number");
							if (errorValue.HasProperty(numberPropertyId))
							{
								IeJsValue numberPropertyValue = errorValue.GetProperty(numberPropertyId);
								int errorNumber = numberPropertyValue.ValueType == JsValueType.Number ?
									numberPropertyValue.ToInt32() : 0;
								errorCode = (JsErrorCode)errorNumber;
							}
						}

						IeJsPropertyId stackPropertyId = IeJsPropertyId.FromString("stack");
						if (errorValue.HasProperty(stackPropertyId))
						{
							IeJsValue stackPropertyValue = errorValue.GetProperty(stackPropertyId);
							string messageWithTypeAndCallStack = stackPropertyValue.ValueType == JsValueType.String ?
								stackPropertyValue.ToString() : string.Empty;
							string messageWithType = errorValue.ConvertToString().ToString();
							string rawCallStack = messageWithTypeAndCallStack
								.TrimStart(messageWithType)
								.TrimStart(new char[] { '\n', '\r' })
								;

							CallStackItem[] callStackItems = JsErrorHelpers.ParseCallStack(rawCallStack);
							if (callStackItems.Length > 0)
							{
								FixCallStackItems(callStackItems);

								CallStackItem firstCallStackItem = callStackItems[0];
								if (firstCallStackItem.DocumentName.Length > 0)
								{
									documentName = firstCallStackItem.DocumentName;
								}
								lineNumber = firstCallStackItem.LineNumber;
								columnNumber = firstCallStackItem.ColumnNumber;
								callStack = JsErrorHelpers.StringifyCallStackItems(callStackItems);
							}

							message = JsErrorHelpers.GenerateScriptErrorMessage(type, description, callStack);
						}
						else
						{
							type = errorCode == JsErrorCode.ScriptCompile ? JsErrorType.Syntax : type;

							IeJsPropertyId linePropertyId = IeJsPropertyId.FromString("line");
							if (errorValue.HasProperty(linePropertyId))
							{
								IeJsValue linePropertyValue = errorValue.GetProperty(linePropertyId);
								lineNumber = linePropertyValue.ValueType == JsValueType.Number ?
									linePropertyValue.ToInt32() + 1 : 0;
							}

							IeJsPropertyId columnPropertyId = IeJsPropertyId.FromString("column");
							if (errorValue.HasProperty(columnPropertyId))
							{
								IeJsValue columnPropertyValue = errorValue.GetProperty(columnPropertyId);
								columnNumber = columnPropertyValue.ValueType == JsValueType.Number ?
									columnPropertyValue.ToInt32() + 1 : 0;
							}

							string sourceLine = string.Empty;
							IeJsPropertyId sourcePropertyId = IeJsPropertyId.FromString("source");
							if (errorValue.HasProperty(sourcePropertyId))
							{
								IeJsValue sourcePropertyValue = errorValue.GetProperty(sourcePropertyId);
								sourceLine = sourcePropertyValue.ValueType == JsValueType.String ?
									sourcePropertyValue.ToString() : string.Empty;
								if (sourceLine != "undefined")
								{
									sourceFragment = TextHelpers.GetTextFragmentFromLine(sourceLine, columnNumber);
								}
							}

							message = JsErrorHelpers.GenerateScriptErrorMessage(type, description, documentName,
								lineNumber, columnNumber, sourceFragment);
						}
					}
					else if (errorValueType == JsValueType.String)
					{
						message = errorValue.ToString();
						description = message;
					}
					else
					{
						message = errorValue.ConvertToString().ToString();
						description = message;
					}
				}

				WrapperScriptException wrapperScriptException;
				if (errorCode == JsErrorCode.ScriptCompile)
				{
					wrapperScriptException = new WrapperCompilationException(message, _engineModeName,
						originalScriptException);
				}
				else if (errorCode == JsErrorCode.ScriptTerminated)
				{
					wrapperScriptException = new WrapperInterruptedException(CommonStrings.Runtime_ScriptInterrupted,
						_engineModeName, originalScriptException);

					// Restore a JS engine after interruption
					_jsRuntime.Disabled = false;
				}
				else
				{
					wrapperScriptException = new WrapperRuntimeException(message, _engineModeName,
						originalScriptException)
					{
						CallStack = callStack
					};
				}
				wrapperScriptException.Type = type;
				wrapperScriptException.DocumentName = documentName;
				wrapperScriptException.LineNumber = lineNumber;
				wrapperScriptException.ColumnNumber = columnNumber;
				wrapperScriptException.SourceFragment = sourceFragment;

				wrapperException = wrapperScriptException;
			}
			else
			{
				if (originalException is OriginalUsageException)
				{
					wrapperException = new WrapperUsageException(message, _engineModeName, originalException);
				}
				else if (originalException is OriginalEngineException)
				{
					wrapperException = new WrapperEngineException(message, _engineModeName, originalException);
				}
				else if (originalException is OriginalFatalException)
				{
					wrapperException = new WrapperFatalException(message, _engineModeName, originalException);
				}
				else
				{
					wrapperException = new WrapperException(message, _engineModeName, originalException);
				}
			}

			wrapperException.Description = description;

			return wrapperException;
		}

		/// <summary>
		/// Fixes a function name in call stack items
		/// </summary>
		/// <param name="callStackItems">An array of <see cref="CallStackItem"/> instances</param>
		private static void FixCallStackItems(CallStackItem[] callStackItems)
		{
			foreach (CallStackItem callStackItem in callStackItems)
			{
				if (callStackItem.FunctionName == "Unknown script code")
				{
					callStackItem.FunctionName = "Global code";
				}
			}
		}

		private WrapperEngineLoadException WrapTypeLoadException(TypeLoadException originalTypeLoadException)
		{
			string originalMessage = originalTypeLoadException.Message;
			bool isDllNotFound = originalTypeLoadException is DllNotFoundException;
			string description;
			string message;

			if (originalMessage.ContainsQuotedValue(DllName.JScript9)
				&& (isDllNotFound || originalMessage.ContainsQuotedValue("JsCreateRuntime")))
			{
				var stringBuilderPool = StringBuilderPool.Shared;
				StringBuilder descriptionBuilder = stringBuilderPool.Rent();
				if (isDllNotFound)
				{
					descriptionBuilder.AppendFormat(CommonStrings.Engine_AssemblyNotRegistered, DllName.JScript9);
					descriptionBuilder.Append(" ");
				}
				descriptionBuilder.AppendFormat(CommonStrings.Engine_IeInstallationRequired, LOWER_IE_VERSION);

				description = descriptionBuilder.ToString();
				stringBuilderPool.Return(descriptionBuilder);

				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName);
			}
			else
			{
				description = originalMessage;
				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName, true);
			}

			var wrapperEngineLoadException = new WrapperEngineLoadException(message, _engineModeName,
				originalTypeLoadException)
			{
				Description = description
			};

			return wrapperEngineLoadException;
		}

		#endregion

		#region ChakraJsRtJsEngineBase overrides

		#region IInnerJsEngine implementation

		public override bool SupportsScriptPrecompilation
		{
			get { return true; }
		}


		public override PrecompiledScript Precompile(string code, string documentName)
		{
			PrecompiledScript precompiledScript = _dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						byte[] cachedBytes = IeJsContext.SerializeScript(code);

						return new PrecompiledScript(_engineModeName, code, cachedBytes, documentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e, documentName);
					}
				}
			});

			return precompiledScript;
		}

		public override object Evaluate(string expression, string documentName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue resultValue = IeJsContext.RunScript(expression, _jsSourceContext++,
							documentName);

						return _typeMapper.MapToHostType(resultValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override void Execute(string code, string documentName)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsContext.RunScript(code, _jsSourceContext++, documentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void Execute(PrecompiledScript precompiledScript)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsContext.RunSerializedScript(precompiledScript.Code, precompiledScript.CachedBytes,
							_jsSourceContext++, precompiledScript.DocumentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
					finally
					{
						GC.KeepAlive(precompiledScript);
					}
				}
			});
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue globalObj = IeJsValue.GlobalObject;
						IeJsPropertyId functionId = IeJsPropertyId.FromString(functionName);

						bool functionExist = globalObj.HasProperty(functionId);
						if (!functionExist)
						{
							throw new WrapperRuntimeException(
								string.Format(CommonStrings.Runtime_FunctionNotExist, functionName),
								_engineModeName
							);
						}

						IeJsValue resultValue;
						IeJsValue functionValue = globalObj.GetProperty(functionId);

						int argCount = args.Length;
						if (argCount > 0)
						{
							int processedArgCount = argCount + 1;
							var processedArgs = new IeJsValue[processedArgCount];
							processedArgs[0] = globalObj;

							for (int argIndex = 0; argIndex < argCount; argIndex++)
							{
								IeJsValue processedArg = _typeMapper.MapToScriptType(args[argIndex]);
								AddReferenceToValue(processedArg);

								processedArgs[argIndex + 1] = processedArg;
							}

							try
							{
								resultValue = functionValue.CallFunction(processedArgs);
							}
							finally
							{
								for (int argIndex = 1; argIndex < processedArgCount; argIndex++)
								{
									RemoveReferenceToValue(processedArgs[argIndex]);
								}
							}
						}
						else
						{
							resultValue = functionValue.CallFunction(globalObj);
						}

						return _typeMapper.MapToHostType(resultValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override bool HasVariable(string variableName)
		{
			bool result = _dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue globalObj = IeJsValue.GlobalObject;
						IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);
						bool variableExist = globalObj.HasProperty(variableId);

						if (variableExist)
						{
							IeJsValue variableValue = globalObj.GetProperty(variableId);
							variableExist = variableValue.ValueType != JsValueType.Undefined;
						}

						return variableExist;
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override object GetVariableValue(string variableName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue variableValue = IeJsValue.GlobalObject.GetProperty(variableName);

						return _typeMapper.MapToHostType(variableValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue inputValue = _typeMapper.MapToScriptType(value);
						AddReferenceToValue(inputValue);

						try
						{
							IeJsValue.GlobalObject.SetProperty(variableName, inputValue, true);
						}
						finally
						{
							RemoveReferenceToValue(inputValue);
						}
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void RemoveVariable(string variableName)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue globalObj = IeJsValue.GlobalObject;
						IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);

						if (globalObj.HasProperty(variableId))
						{
							globalObj.SetProperty(variableId, IeJsValue.Undefined, true);
						}
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void EmbedHostObject(string itemName, object value)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue processedValue = _typeMapper.GetOrCreateScriptObject(value);
						IeJsValue.GlobalObject.SetProperty(itemName, processedValue, true);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			_dispatcher.Invoke(() =>
			{
				using (new IeJsScope(_jsContext))
				{
					try
					{
						IeJsValue typeValue = _typeMapper.GetOrCreateScriptType(type);
						IeJsValue.GlobalObject.SetProperty(itemName, typeValue, true);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void Interrupt()
		{
			_jsRuntime.Disabled = true;
		}

		public override void CollectGarbage()
		{
			_jsRuntime.CollectGarbage();
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

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_disposedFlag.Set())
				{
					if (_dispatcher != null)
					{
						_dispatcher.Invoke(DisposeUnmanagedResources);

						_dispatcher.Dispose();
						_dispatcher = null;
					}

					if (_typeMapper != null)
					{
						_typeMapper.Dispose();
						_typeMapper = null;
					}
				}
			}
			else
			{
				DisposeUnmanagedResources();
			}
		}

		private void DisposeUnmanagedResources()
		{
			if (_jsContext.IsValid)
			{
				_jsContext.Release();
			}
			_jsRuntime.Dispose();
		}

		#endregion

		#endregion
	}
}
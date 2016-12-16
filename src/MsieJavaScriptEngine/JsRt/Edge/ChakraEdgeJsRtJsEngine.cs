using System;
#if NETSTANDARD1_3
using System.Collections.Generic;
#endif
using System.Globalization;
using System.Linq;
#if NETSTANDARD1_3
using System.Reflection;
using System.Runtime.InteropServices;
#endif

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.JsRt.Edge
{
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
		private EdgeJsContext _jsContext;

		/// <summary>
		/// Flag indicating whether this JavaScript engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static readonly object _supportSynchronizer = new object();
#if NETSTANDARD1_3

		/// <summary>
		/// List of native function callbacks
		/// </summary>
		private readonly HashSet<EdgeJsNativeFunction> _nativeFunctions = new HashSet<EdgeJsNativeFunction>();
#endif


		/// <summary>
		/// Constructs an instance of the Chakra “Edge” JsRT JavaScript engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ChakraEdgeJsRtJsEngine(bool enableDebugging)
			: base(JsEngineMode.ChakraEdgeJsRt, enableDebugging)
		{
			_dispatcher.Invoke(() =>
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
						errorMessage = CommonStrings.Runtime_JsEnginesConflictOnMachine;
					}
					else
					{
						errorMessage = string.Format(CommonStrings.Runtime_EdgeJsEngineNotLoaded, e.Message);
					}

					throw new JsEngineLoadException(errorMessage, _engineModeName);
				}
				catch (Exception e)
				{
					throw new JsEngineLoadException(
						string.Format(CommonStrings.Runtime_EdgeJsEngineNotLoaded, e.Message), _engineModeName);
				}
			});
		}

		/// <summary>
		/// Destructs an instance of the Chakra “Edge” JsRT JavaScript engine
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
			return EdgeJsRuntime.Create(JsRuntimeAttributes.None, null);
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

			var typeCode = value.GetType().GetTypeCode();

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return (bool)value ? EdgeJsValue.True : EdgeJsValue.False;

				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return EdgeJsValue.FromInt32(Convert.ToInt32(value));

				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return EdgeJsValue.FromDouble(Convert.ToDouble(value));

				case TypeCode.Char:
				case TypeCode.String:
					return EdgeJsValue.FromString((string)value);

				default:
#if NETSTANDARD1_3
					return FromObject(value);
#else
					object processedValue = !TypeConverter.IsPrimitiveType(typeCode) ?
						new HostObject(value, _engineMode) : value;
					return EdgeJsValue.FromObject(processedValue);
#endif
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
					result = NumericHelpers.CastDoubleValueToCorrectType(processedValue.ToDouble());
					break;
				case JsValueType.String:
					processedValue = value.ConvertToString();
					result = processedValue.ToString();
					break;
				case JsValueType.Object:
				case JsValueType.Function:
				case JsValueType.Error:
				case JsValueType.Array:
#if NETSTANDARD1_3
					result = ToObject(value);
#else
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
#endif
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

		/// <summary>
		/// Adds a reference to the value
		/// </summary>
		/// <param name="value">The value</param>
		private static void AddReferenceToValue(EdgeJsValue value)
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
		private static void RemoveReferenceToValue(EdgeJsValue value)
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
		/// <returns>Result of check (true - may have; false - may not have)</returns>
		private static bool CanHaveReferences(EdgeJsValue value)
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
#if NETSTANDARD1_3

		private EdgeJsValue FromObject(object value)
		{
			var del = value as Delegate;
			EdgeJsValue objValue = del != null ? CreateFunctionFromDelegate(del) : CreateExternalObjectFromObject(value);

			return objValue;
		}

		private object ToObject(EdgeJsValue value)
		{
			object result = value.HasExternalData ?
				GCHandle.FromIntPtr(value.ExternalData).Target : value.ConvertToObject();

			return result;
		}

		private EdgeJsValue CreateExternalObjectFromObject(object value)
		{
			GCHandle handle = GCHandle.Alloc(value);
			_externalObjects.Add(value);

			EdgeJsValue objValue = EdgeJsValue.CreateExternalObject(
				GCHandle.ToIntPtr(handle), _externalObjectFinalizeCallback);
			Type type = value.GetType();

			ProjectFields(objValue, type, true);
			ProjectProperties(objValue, type, true);
			ProjectMethods(objValue, type, true);
			FreezeObject(objValue);

			return objValue;
		}

		private EdgeJsValue CreateObjectFromType(Type type)
		{
			EdgeJsValue typeValue = CreateConstructor(type);

			ProjectFields(typeValue, type, false);
			ProjectProperties(typeValue, type, false);
			ProjectMethods(typeValue, type, false);
			FreezeObject(typeValue);

			return typeValue;
		}

		private void FreezeObject(EdgeJsValue objValue)
		{
			EdgeJsValue freezeMethodValue = EdgeJsValue.GlobalObject
				.GetProperty("Object")
				.GetProperty("freeze")
				;
			freezeMethodValue.CallFunction(objValue);
		}

		private EdgeJsValue CreateFunctionFromDelegate(Delegate value)
		{
			EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				object[] processedArgs = MapToHostType(args.Skip(1).ToArray());
				ParameterInfo[] parameters = value.GetMethodInfo().GetParameters();
				EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, parameters);

				object result;

				try
				{
					result = value.DynamicInvoke(processedArgs);
				}
				catch (Exception e)
				{
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostDelegateInvocationFailed, e.Message));
					EdgeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				EdgeJsValue resultValue = MapToScriptType(result);

				return resultValue;
			};
			_nativeFunctions.Add(nativeFunction);

			EdgeJsValue functionValue = EdgeJsValue.CreateFunction(nativeFunction);

			return functionValue;
		}

		private EdgeJsValue CreateConstructor(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(true);
			ConstructorInfo[] constructors = type.GetConstructors(defaultBindingFlags);

			EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				EdgeJsValue resultValue;
				EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

				object[] processedArgs = MapToHostType(args.Skip(1).ToArray());
				object result;

				if (processedArgs.Length == 0 && typeInfo.IsValueType)
				{
					result = Activator.CreateInstance(type);
					resultValue = MapToScriptType(result);

					return resultValue;
				}

				if (constructors.Length == 0)
				{
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorNotFound, typeName));
					EdgeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				var bestFitConstructor = (ConstructorInfo)ReflectionHelpers.GetBestFitMethod(
					constructors, processedArgs);
				if (bestFitConstructor == null)
				{
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateReferenceError(
						string.Format(NetCoreStrings.Runtime_SuitableConstructorOfHostTypeNotFound, typeName));
					EdgeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, bestFitConstructor.GetParameters());

				try
				{
					result = bestFitConstructor.Invoke(processedArgs);
				}
				catch (Exception e)
				{
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorInvocationFailed, typeName, e.Message));
					EdgeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				resultValue = MapToScriptType(result);

				return resultValue;
			};
			_nativeFunctions.Add(nativeFunction);

			EdgeJsValue constructorValue = EdgeJsValue.CreateFunction(nativeFunction);

			return constructorValue;
		}

		private void ProjectFields(EdgeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			FieldInfo[] fields = type.GetFields(defaultBindingFlags);

			foreach (FieldInfo field in fields)
			{
				string fieldName = field.Name;

				EdgeJsValue descriptorValue = EdgeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", EdgeJsValue.True, true);

				EdgeJsNativeFunction nativeGetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					EdgeJsValue thisValue = args[0];
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						thisObj = MapToHostType(thisValue);
					}

					object result;

					try
					{
						result = field.GetValue(thisObj);
					}
					catch (Exception e)
					{
						string errorMessage = instance ?
							string.Format(NetCoreStrings.Runtime_HostObjectFieldGettingFailed, fieldName, e.Message)
							:
							string.Format(NetCoreStrings.Runtime_HostTypeFieldGettingFailed, fieldName, typeName, e.Message)
							;

						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					EdgeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				_nativeFunctions.Add(nativeGetFunction);

				EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeGetFunction);
				descriptorValue.SetProperty("get", getMethodValue, true);

				EdgeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					EdgeJsValue thisValue = args[0];
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						thisObj = MapToHostType(thisValue);
					}

					object value = MapToHostType(args.Skip(1).First());
					ReflectionHelpers.FixFieldValueType(ref value, field);

					try
					{
						field.SetValue(thisObj, value);
					}
					catch (Exception e)
					{
						string errorMessage = instance ?
							string.Format(NetCoreStrings.Runtime_HostObjectFieldSettingFailed, fieldName, e.Message)
							:
							string.Format(NetCoreStrings.Runtime_HostTypeFieldSettingFailed, fieldName, typeName, e.Message)
							;

						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					return undefinedValue;
				};
				_nativeFunctions.Add(nativeSetFunction);

				EdgeJsValue setMethodValue = EdgeJsValue.CreateFunction(nativeSetFunction);
				descriptorValue.SetProperty("set", setMethodValue, true);

				target.DefineProperty(fieldName, descriptorValue);
			}
		}

		private void ProjectProperties(EdgeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			PropertyInfo[] properties = type.GetProperties(defaultBindingFlags);

			foreach (PropertyInfo property in properties)
			{
				string propertyName = property.Name;

				EdgeJsValue descriptorValue = EdgeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", EdgeJsValue.True, true);

				if (property.GetGetMethod() != null)
				{
					EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						EdgeJsValue thisValue = args[0];
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

						object thisObj = null;

						if (instance)
						{
							if (!thisValue.HasExternalData)
							{
								EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
									string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
								EdgeJsErrorHelpers.SetException(errorValue);

								return undefinedValue;
							}

							thisObj = MapToHostType(thisValue);
						}

						object result;

						try
						{
							result = property.GetValue(thisObj, new object[0]);
						}
						catch (Exception e)
						{
							string errorMessage = instance ?
								string.Format(
									NetCoreStrings.Runtime_HostObjectPropertyGettingFailed, propertyName, e.Message)
								:
								string.Format(
									NetCoreStrings.Runtime_HostTypePropertyGettingFailed, propertyName, typeName, e.Message)
								;

							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						EdgeJsValue resultValue = MapToScriptType(result);

						return resultValue;
					};
					_nativeFunctions.Add(nativeFunction);

					EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeFunction);
					descriptorValue.SetProperty("get", getMethodValue, true);
				}

				if (property.GetSetMethod() != null)
				{
					EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						EdgeJsValue thisValue = args[0];
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

						object thisObj = null;

						if (instance)
						{
							if (!thisValue.HasExternalData)
							{
								EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
									string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
								EdgeJsErrorHelpers.SetException(errorValue);

								return undefinedValue;
							}

							thisObj = MapToHostType(thisValue);
						}

						object value = MapToHostType(args.Skip(1).First());
						ReflectionHelpers.FixPropertyValueType(ref value, property);

						try
						{
							property.SetValue(thisObj, value, new object[0]);
						}
						catch (Exception e)
						{
							string errorMessage = instance ?
								string.Format(
									NetCoreStrings.Runtime_HostObjectPropertySettingFailed, propertyName, e.Message)
								:
								string.Format(
									NetCoreStrings.Runtime_HostTypePropertySettingFailed, propertyName, typeName, e.Message)
								;

							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						return undefinedValue;
					};
					_nativeFunctions.Add(nativeFunction);

					EdgeJsValue setMethodValue = EdgeJsValue.CreateFunction(nativeFunction);
					descriptorValue.SetProperty("set", setMethodValue, true);
				}

				target.DefineProperty(propertyName, descriptorValue);
			}
		}

		private void ProjectMethods(EdgeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			MethodInfo[] methods = type.GetMethods(defaultBindingFlags);
			IEnumerable<IGrouping<string, MethodInfo>> methodGroups = methods.GroupBy(m => m.Name);

			foreach (IGrouping<string, MethodInfo> methodGroup in methodGroups)
			{
				string methodName = methodGroup.Key;
				MethodInfo[] methodCandidates = methodGroup.ToArray();

				EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					EdgeJsValue thisValue = args[0];
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectMethod, methodName));
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						thisObj = MapToHostType(thisValue);
					}

					object[] processedArgs = MapToHostType(args.Skip(1).ToArray());

					var bestFitMethod = (MethodInfo)ReflectionHelpers.GetBestFitMethod(
						methodCandidates, processedArgs);
					if (bestFitMethod == null)
					{
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateReferenceError(
							string.Format(NetCoreStrings.Runtime_SuitableMethodOfHostObjectNotFound, methodName));
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					ReflectionHelpers.FixArgumentTypes(ref processedArgs, bestFitMethod.GetParameters());

					object result;

					try
					{
						result = bestFitMethod.Invoke(thisObj, processedArgs);
					}
					catch (Exception e)
					{
						string errorMessage = instance ?
							string.Format(
								NetCoreStrings.Runtime_HostObjectMethodInvocationFailed, methodName, e.Message)
							:
							string.Format(
								NetCoreStrings.Runtime_HostTypeMethodInvocationFailed, methodName, typeName, e.Message)
							;

						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					EdgeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				_nativeFunctions.Add(nativeFunction);

				EdgeJsValue methodValue = EdgeJsValue.CreateFunction(nativeFunction);
				target.SetProperty(methodName, methodValue, true);
			}
		}
#endif

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
				SourceFragment = sourceFragment
			};

			return jsEngineException;
		}

		protected override void InnerStartDebugging()
		{
			EdgeJsContext.StartDebugging();
		}

		private void InvokeScript(Action action)
		{
			_dispatcher.Invoke(() =>
			{
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
			});
		}

		private T InvokeScript<T>(Func<T> func)
		{
			return _dispatcher.Invoke(() =>
			{
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
			});
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
						string.Format(CommonStrings.Runtime_FunctionNotExist, functionName));
				}

				EdgeJsValue resultValue;
				EdgeJsValue functionValue = globalObj.GetProperty(functionId);

				if (args.Length > 0)
				{
					EdgeJsValue[] processedArgs = MapToScriptType(args);

					foreach (EdgeJsValue processedArg in processedArgs)
					{
						AddReferenceToValue(processedArg);
					}

					EdgeJsValue[] allProcessedArgs = new[] { globalObj }.Concat(processedArgs).ToArray();
					resultValue = functionValue.CallFunction(allProcessedArgs);

					foreach (EdgeJsValue processedArg in processedArgs)
					{
						RemoveReferenceToValue(processedArg);
					}
				}
				else
				{
					resultValue = functionValue.CallFunction(globalObj);
				}

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
					variableExist = variableValue.ValueType != JsValueType.Undefined;
				}

				return variableExist;
			});

			return result;
		}

		public override object GetVariableValue(string variableName)
		{
			object result = InvokeScript(() =>
			{
				EdgeJsValue variableValue = EdgeJsValue.GlobalObject.GetProperty(variableName);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				EdgeJsValue inputValue = MapToScriptType(value);
				EdgeJsValue.GlobalObject.SetProperty(variableName, inputValue, true);
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
				EdgeJsValue.GlobalObject.SetProperty(itemName, processedValue, true);
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			InvokeScript(() =>
			{
#if NETSTANDARD1_3
				EdgeJsValue typeValue = CreateObjectFromType(type);
#else
				EdgeJsValue typeValue = EdgeJsValue.FromObject(new HostType(type, _engineMode));
#endif
				EdgeJsValue.GlobalObject.SetProperty(itemName, typeValue, true);
			});
		}

		public override void CollectGarbage()
		{
			_dispatcher.Invoke(() => _jsRuntime.CollectGarbage());
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
		/// <param name="disposing">Flag, allowing destruction of
		/// managed objects contained in fields of class</param>
		protected override void Dispose(bool disposing)
		{
			if (_disposedFlag.Set())
			{
				_dispatcher.Invoke(() =>
				{
					_jsRuntime.Dispose();
					base.Dispose(disposing);
#if NETSTANDARD1_3

					if (disposing)
					{
						if (_nativeFunctions != null)
						{
							_nativeFunctions.Clear();
						}
					}
#endif
				});
				_dispatcher.Dispose();
			}
		}

		#endregion
	}
}
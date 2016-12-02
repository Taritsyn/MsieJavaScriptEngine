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

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” JsRT version of Chakra JavaScript engine
	/// </summary>
	internal sealed class ChakraIeJsRtJsEngine : ChakraJsRtJsEngineBase
	{
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
#if NETSTANDARD1_3

		/// <summary>
		/// List of native function callbacks
		/// </summary>
		private readonly HashSet<IeJsNativeFunction> _nativeFunctions = new HashSet<IeJsNativeFunction>();
#endif

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private StatedFlag _disposedFlag = new StatedFlag();


		/// <summary>
		/// Constructs instance of the Chakra “IE” JsRT JavaScript engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ChakraIeJsRtJsEngine(bool enableDebugging)
			: base(JsEngineMode.ChakraIeJsRt, enableDebugging)
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
					errorMessage = string.Format(CommonStrings.Runtime_IeJsEngineNotLoaded,
						_engineModeName, LOWER_IE_VERSION, e.Message);
				}

				throw new JsEngineLoadException(errorMessage, _engineModeName);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(CommonStrings.Runtime_IeJsEngineNotLoaded,
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
			return IeJsRuntime.Create(JsRuntimeAttributes.None, JsRuntimeVersion.VersionEdge, null);
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
#if NETSTANDARD1_3
				catch (TypeLoadException e)
#else
				catch (EntryPointNotFoundException e)
#endif
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

			var typeCode = value.GetType().GetTypeCode();

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return (bool)value ? IeJsValue.True : IeJsValue.False;

				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return IeJsValue.FromInt32(Convert.ToInt32(value));

				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return IeJsValue.FromDouble(Convert.ToDouble(value));

				case TypeCode.Char:
				case TypeCode.String:
					return IeJsValue.FromString((string)value);

				default:
#if NETSTANDARD1_3
					return FromObject(value);
#else
					object processedValue = !TypeConverter.IsPrimitiveType(typeCode) ?
						new HostObject(value, _engineMode) : value;
					return IeJsValue.FromObject(processedValue);
#endif
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
		/// Makes a mapping of array itemp from the script type to a host type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToHostType(IeJsValue[] args)
		{
			return args.Select(MapToHostType).ToArray();
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
		/// <returns>Result of check (true - may have; false - may not have)</returns>
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
#if NETSTANDARD1_3

		private IeJsValue FromObject(object value)
		{
			var del = value as Delegate;
			IeJsValue objValue = del != null ? CreateFunctionFromDelegate(del) : CreateExternalObjectFromObject(value);

			return objValue;
		}

		private object ToObject(IeJsValue value)
		{
			object result = value.HasExternalData ?
				GCHandle.FromIntPtr(value.ExternalData).Target : value.ConvertToObject();

			return result;
		}

		private IeJsValue CreateExternalObjectFromObject(object value)
		{
			GCHandle handle = GCHandle.Alloc(value);
			_externalObjects.Add(value);

			IeJsValue objValue = IeJsValue.CreateExternalObject(
				GCHandle.ToIntPtr(handle), _externalObjectFinalizeCallback);
			Type type = value.GetType();

			ProjectFields(objValue, type, true);
			ProjectProperties(objValue, type, true);
			ProjectMethods(objValue, type, true);
			FreezeObject(objValue);

			return objValue;
		}

		private IeJsValue CreateObjectFromType(Type type)
		{
			IeJsValue typeValue = CreateConstructor(type);

			ProjectFields(typeValue, type, false);
			ProjectProperties(typeValue, type, false);
			ProjectMethods(typeValue, type, false);
			FreezeObject(typeValue);

			return typeValue;
		}

		private void FreezeObject(IeJsValue objValue)
		{
			IeJsValue objectValue = IeJsValue.GlobalObject.GetProperty("Object");
			IeJsValue freezeMethodValue = objectValue.GetProperty("freeze");

			freezeMethodValue.CallFunction(objectValue, objValue);
		}

		private IeJsValue CreateFunctionFromDelegate(Delegate value)
		{
			IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				object[] processedArgs = MapToHostType(args.Skip(1).ToArray());
				ParameterInfo[] parameters = value.GetMethodInfo().GetParameters();
				IeJsValue undefinedValue = IeJsValue.Undefined;

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, parameters);

				object result;

				try
				{
					result = value.DynamicInvoke(processedArgs);
				}
				catch (Exception e)
				{
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostDelegateInvocationFailed, e.Message));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				IeJsValue resultValue = MapToScriptType(result);

				return resultValue;
			};
			_nativeFunctions.Add(nativeFunction);

			IeJsValue functionValue = IeJsValue.CreateFunction(nativeFunction);

			return functionValue;
		}

		private IeJsValue CreateConstructor(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(true);
			ConstructorInfo[] constructors = type.GetConstructors(defaultBindingFlags);

			IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				IeJsValue resultValue;
				IeJsValue undefinedValue = IeJsValue.Undefined;

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
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorNotFound, typeName));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				var bestFitConstructor = (ConstructorInfo)ReflectionHelpers.GetBestFitMethod(
					constructors, processedArgs);
				if (bestFitConstructor == null)
				{
					IeJsValue errorValue = IeJsErrorHelpers.CreateReferenceError(
						string.Format(NetCoreStrings.Runtime_SuitableConstructorOfHostTypeNotFound, typeName));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, bestFitConstructor.GetParameters());

				try
				{
					result = bestFitConstructor.Invoke(processedArgs);
				}
				catch (Exception e)
				{
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorInvocationFailed, typeName, e.Message));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				resultValue = MapToScriptType(result);

				return resultValue;
			};
			_nativeFunctions.Add(nativeFunction);

			IeJsValue constructorValue = IeJsValue.CreateFunction(nativeFunction);

			return constructorValue;
		}

		private void ProjectFields(IeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			FieldInfo[] fields = type.GetFields(defaultBindingFlags);

			foreach (FieldInfo field in fields)
			{
				string fieldName = field.Name;

				IeJsValue descriptorValue = IeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", IeJsValue.True, true);

				IeJsNativeFunction nativeGetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					IeJsValue thisValue = args[0];
					IeJsValue undefinedValue = IeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
							IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					IeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				_nativeFunctions.Add(nativeGetFunction);

				IeJsValue getMethodValue = IeJsValue.CreateFunction(nativeGetFunction);
				descriptorValue.SetProperty("get", getMethodValue, true);

				IeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					IeJsValue thisValue = args[0];
					IeJsValue undefinedValue = IeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
							IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					return undefinedValue;
				};
				_nativeFunctions.Add(nativeSetFunction);

				IeJsValue setMethodValue = IeJsValue.CreateFunction(nativeSetFunction);
				descriptorValue.SetProperty("set", setMethodValue, true);

				target.DefineProperty(fieldName, descriptorValue);
			}
		}

		private void ProjectProperties(IeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			PropertyInfo[] properties = type.GetProperties(defaultBindingFlags);

			foreach (PropertyInfo property in properties)
			{
				string propertyName = property.Name;

				IeJsValue descriptorValue = IeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", IeJsValue.True, true);

				if (property.GetGetMethod() != null)
				{
					IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						IeJsValue thisValue = args[0];
						IeJsValue undefinedValue = IeJsValue.Undefined;

						object thisObj = null;

						if (instance)
						{
							if (!thisValue.HasExternalData)
							{
								IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
									string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
								IeJsErrorHelpers.SetException(errorValue);

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

							IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
							IeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						IeJsValue resultValue = MapToScriptType(result);

						return resultValue;
					};
					_nativeFunctions.Add(nativeFunction);

					IeJsValue getMethodValue = IeJsValue.CreateFunction(nativeFunction);
					descriptorValue.SetProperty("get", getMethodValue, true);
				}

				if (property.GetSetMethod() != null)
				{
					IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						IeJsValue thisValue = args[0];
						IeJsValue undefinedValue = IeJsValue.Undefined;

						object thisObj = null;

						if (instance)
						{
							if (!thisValue.HasExternalData)
							{
								IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
									string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
								IeJsErrorHelpers.SetException(errorValue);

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

							IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
							IeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						return undefinedValue;
					};
					_nativeFunctions.Add(nativeFunction);

					IeJsValue setMethodValue = IeJsValue.CreateFunction(nativeFunction);
					descriptorValue.SetProperty("set", setMethodValue, true);
				}

				target.DefineProperty(propertyName, descriptorValue);
			}
		}

		private void ProjectMethods(IeJsValue target, Type type, bool instance)
		{
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			MethodInfo[] methods = type.GetMethods(defaultBindingFlags);
			IEnumerable<IGrouping<string, MethodInfo>> methodGroups = methods.GroupBy(m => m.Name);

			foreach (IGrouping<string, MethodInfo> methodGroup in methodGroups)
			{
				string methodName = methodGroup.Key;
				MethodInfo[] methodCandidates = methodGroup.ToArray();

				IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					IeJsValue thisValue = args[0];
					IeJsValue undefinedValue = IeJsValue.Undefined;

					object thisObj = null;

					if (instance)
					{
						if (!thisValue.HasExternalData)
						{
							IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectMethod, methodName));
							IeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						thisObj = MapToHostType(thisValue);
					}

					object[] processedArgs = MapToHostType(args.Skip(1).ToArray());

					var bestFitMethod = (MethodInfo)ReflectionHelpers.GetBestFitMethod(
						methodCandidates, processedArgs);
					if (bestFitMethod == null)
					{
						IeJsValue errorValue = IeJsErrorHelpers.CreateReferenceError(
							string.Format(NetCoreStrings.Runtime_SuitableMethodOfHostObjectNotFound, methodName));
						IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					IeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				_nativeFunctions.Add(nativeFunction);

				IeJsValue methodValue = IeJsValue.CreateFunction(nativeFunction);
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
				SourceFragment = sourceFragment
			};

			return jsEngineException;
		}

		protected override void InnerStartDebugging()
		{
			if (Utils.Is64BitProcess())
			{
				var processDebugManager64 = (IProcessDebugManager64)new ProcessDebugManager();
				IDebugApplication64 debugApplication64;
				processDebugManager64.GetDefaultApplication(out debugApplication64);

				IeJsContext.StartDebugging(debugApplication64);
			}
			else
			{
				var processDebugManager32 = (IProcessDebugManager32)new ProcessDebugManager();
				IDebugApplication32 debugApplication32;
				processDebugManager32.GetDefaultApplication(out debugApplication32);

				IeJsContext.StartDebugging(debugApplication32);
			}
		}

		private void InvokeScript(Action action)
		{
			lock (_executionSynchronizer)
			using (new IeJsScope(_jsContext))
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
			lock (_executionSynchronizer)
			using (new IeJsScope(_jsContext))
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
			lock (_executionSynchronizer)
			{
				if (_disposedFlag.Set())
				{
					_jsRuntime.Dispose();
					base.Dispose();
#if NETSTANDARD1_3

					if (_nativeFunctions != null)
					{
						_nativeFunctions.Clear();
					}
#endif
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
				IeJsValue resultValue = IeJsContext.RunScript(expression);

				return MapToHostType(resultValue);
			});

			return result;
		}

		public override void Execute(string code)
		{
			InvokeScript(() => IeJsContext.RunScript(code));
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object result = InvokeScript(() =>
			{
				IeJsValue globalObj = IeJsValue.GlobalObject;
				IeJsPropertyId functionId = IeJsPropertyId.FromString(functionName);

				bool functionExist = globalObj.HasProperty(functionId);
				if (!functionExist)
				{
					throw new JsRuntimeException(
						string.Format(CommonStrings.Runtime_FunctionNotExist, functionName));
				}

				IeJsValue resultValue;
				IeJsValue functionValue = globalObj.GetProperty(functionId);

				if (args.Length > 0)
				{
					IeJsValue[] processedArgs = MapToScriptType(args);

					foreach (IeJsValue processedArg in processedArgs)
					{
						AddReferenceToValue(processedArg);
					}

					IeJsValue[] allProcessedArgs = new[] { globalObj }.Concat(processedArgs).ToArray();
					resultValue = functionValue.CallFunction(allProcessedArgs);

					foreach (IeJsValue processedArg in processedArgs)
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
				IeJsValue globalObj = IeJsValue.GlobalObject;
				IeJsPropertyId variableId = IeJsPropertyId.FromString(variableName);
				bool variableExist = globalObj.HasProperty(variableId);

				if (variableExist)
				{
					IeJsValue variableValue = globalObj.GetProperty(variableId);
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
				IeJsValue variableValue = IeJsValue.GlobalObject.GetProperty(variableName);

				return MapToHostType(variableValue);
			});

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			InvokeScript(() =>
			{
				IeJsValue inputValue = MapToScriptType(value);
				IeJsValue.GlobalObject.SetProperty(variableName, inputValue, true);
			});
		}

		public override void RemoveVariable(string variableName)
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

		public override void EmbedHostObject(string itemName, object value)
		{
			InvokeScript(() =>
			{
				IeJsValue processedValue = MapToScriptType(value);
				IeJsValue.GlobalObject.SetProperty(itemName, processedValue, true);
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			InvokeScript(() =>
			{
#if NETSTANDARD1_3
				IeJsValue typeValue = CreateObjectFromType(type);
#else
				IeJsValue typeValue = IeJsValue.FromObject(new HostType(type, _engineMode));
#endif
				IeJsValue.GlobalObject.SetProperty(itemName, typeValue, true);
			});
		}

		public override void CollectGarbage()
		{
			lock (_executionSynchronizer)
			{
				_jsRuntime.CollectGarbage();
			}
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
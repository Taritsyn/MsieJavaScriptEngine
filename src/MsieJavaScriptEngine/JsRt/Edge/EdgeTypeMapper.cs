using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.Extensions;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.JsRt.Embedding;
using MsieJavaScriptEngine.Resources;

using WrapperException = MsieJavaScriptEngine.JsException;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	using EdgeEmbeddedItem = EmbeddedItem<EdgeJsValue, EdgeJsNativeFunction>;
	using EdgeEmbeddedObject = EmbeddedObject<EdgeJsValue, EdgeJsNativeFunction>;
	using EdgeEmbeddedType = EmbeddedType<EdgeJsValue, EdgeJsNativeFunction>;

	/// <summary>
	/// “Edge” type mapper
	/// </summary>
	internal sealed class EdgeTypeMapper : TypeMapper<EdgeJsValue, EdgeJsNativeFunction>
	{
		/// <summary>
		/// Constructs an instance of the “Edge” type mapper
		/// </summary>
		/// <param name="allowReflection">Flag for whether to allow the usage of reflection API in the script code</param>
		public EdgeTypeMapper(bool allowReflection)
			: base(allowReflection)
		{ }


		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public override EdgeJsValue MapToScriptType(object value)
		{
			if (value is null)
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
					return value is EdgeJsValue ? (EdgeJsValue)value : GetOrCreateScriptObject(value);
			}
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public override object MapToHostType(EdgeJsValue value)
		{
			JsValueType valueType = value.ValueType;
			object result = null;

			switch (valueType)
			{
				case JsValueType.Null:
					result = null;
					break;
				case JsValueType.Undefined:
					result = Undefined.Value;
					break;
				case JsValueType.Boolean:
					result = value.ToBoolean();
					break;
				case JsValueType.Number:
					result = NumericHelpers.CastDoubleValueToCorrectType(value.ToDouble());
					break;
				case JsValueType.String:
					result = value.ToString();
					break;
				case JsValueType.Function:
					EdgeJsPropertyId externalObjectPropertyId = EdgeJsPropertyId.FromString(ExternalObjectPropertyName);
					if (value.HasProperty(externalObjectPropertyId))
					{
						EdgeJsValue externalObjectValue = value.GetProperty(externalObjectPropertyId);
						result = externalObjectValue.HasExternalData ?
							GCHandle.FromIntPtr(externalObjectValue.ExternalData).Target : null;
					}

					result = result ?? value.ConvertToObject();
					break;
				case JsValueType.Object:
				case JsValueType.Error:
				case JsValueType.Array:
					result = value.HasExternalData ?
						GCHandle.FromIntPtr(value.ExternalData).Target : value.ConvertToObject();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}

		protected override EdgeEmbeddedObject CreateEmbeddedObjectOrFunction(object obj)
		{
			var del = obj as Delegate;
			EdgeEmbeddedObject embeddedObject = del is not null ?
				CreateEmbeddedFunction(del) : CreateEmbeddedObject(obj);

			return embeddedObject;
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private EdgeEmbeddedObject CreateEmbeddedObject(object obj)
		{
			GCHandle objHandle = GCHandle.Alloc(obj);
			IntPtr objPtr = GCHandle.ToIntPtr(objHandle);
			EdgeJsValue objValue = EdgeJsValue.CreateExternalObject(objPtr, _embeddedObjectFinalizeCallback);

			var embeddedObject = new EdgeEmbeddedObject(obj, objValue);

			ProjectFields(embeddedObject);
			ProjectProperties(embeddedObject);
			ProjectMethods(embeddedObject);
			FreezeObject(objValue);

			return embeddedObject;
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private EdgeEmbeddedObject CreateEmbeddedFunction(Delegate del)
		{
			EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
#if NET40
				MethodInfo method = del.Method;
#else
				MethodInfo method = del.GetMethodInfo();
#endif
				ParameterInfo[] parameters = method.GetParameters();
				object[] processedArgs = GetHostItemMemberArguments(args, parameters.Length);

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, parameters);

				object result;

				try
				{
					result = del.DynamicInvoke(processedArgs);
				}
				catch (Exception e)
				{
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
					Exception exception = UnwrapException(e);
					var wrapperException = exception as WrapperException;
					EdgeJsValue errorValue = wrapperException is not null ?
						CreateErrorFromWrapperException(wrapperException)
						:
						EdgeJsErrorHelpers.CreateError(string.Format(
							CommonStrings.Runtime_HostDelegateInvocationFailed, exception.Message))
						;
					EdgeJsContext.SetException(errorValue);

					return undefinedValue;
				}

				EdgeJsValue resultValue = MapToScriptType(result);

				return resultValue;
			};

			GCHandle delHandle = GCHandle.Alloc(del);
			IntPtr delPtr = GCHandle.ToIntPtr(delHandle);
			EdgeJsValue objValue = EdgeJsValue.CreateExternalObject(delPtr, _embeddedObjectFinalizeCallback);

			EdgeJsValue functionValue = EdgeJsValue.CreateFunction(nativeFunction);
			SetNonEnumerableProperty(functionValue, ExternalObjectPropertyName, objValue);

			var embeddedObject = new EdgeEmbeddedObject(del, functionValue, [nativeFunction]);

			return embeddedObject;
		}

		protected override EdgeEmbeddedType CreateEmbeddedType(Type type)
		{
#if NET40
			Type typeInfo = type;
#else
			TypeInfo typeInfo = type.GetTypeInfo();
#endif
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(true);
			ConstructorInfo[] constructors = type.GetConstructors(defaultBindingFlags);

			EdgeJsNativeFunction nativeConstructorFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				object result;
				EdgeJsValue resultValue;
				object[] processedArgs = GetHostItemMemberArguments(args);

				if (processedArgs.Length == 0 && typeInfo.IsValueType)
				{
					result = Activator.CreateInstance(type);
					resultValue = MapToScriptType(result);

					return resultValue;
				}

				EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

				if (constructors.Length == 0)
				{
					CreateAndSetError(string.Format(CommonStrings.Runtime_HostTypeConstructorNotFound, typeName));
					return undefinedValue;
				}

				var bestFitConstructor = (ConstructorInfo)ReflectionHelpers.GetBestFitMethod(
					constructors, processedArgs);
				if (bestFitConstructor is null)
				{
					CreateAndSetReferenceError(string.Format(
						CommonStrings.Runtime_SuitableConstructorOfHostTypeNotFound, typeName));
					return undefinedValue;
				}

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, bestFitConstructor.GetParameters());

				try
				{
					result = bestFitConstructor.Invoke(processedArgs);
				}
				catch (Exception e)
				{
					Exception exception = UnwrapException(e);
					var wrapperException = exception as WrapperException;
					EdgeJsValue errorValue = wrapperException is not null ?
						CreateErrorFromWrapperException(wrapperException)
						:
						EdgeJsErrorHelpers.CreateError(string.Format(
							CommonStrings.Runtime_HostTypeConstructorInvocationFailed, typeName, exception.Message))
						;
					EdgeJsContext.SetException(errorValue);

					return undefinedValue;
				}

				resultValue = MapToScriptType(result);

				return resultValue;
			};

			GCHandle embeddedTypeHandle = GCHandle.Alloc(type);
			IntPtr embeddedTypePtr = GCHandle.ToIntPtr(embeddedTypeHandle);
			EdgeJsValue objValue = EdgeJsValue.CreateExternalObject(embeddedTypePtr,
				_embeddedTypeFinalizeCallback);

			EdgeJsValue typeValue = EdgeJsValue.CreateFunction(nativeConstructorFunction);
			SetNonEnumerableProperty(typeValue, ExternalObjectPropertyName, objValue);

			var embeddedType = new EdgeEmbeddedType(type, typeValue,
				new List<EdgeJsNativeFunction> { nativeConstructorFunction });

			ProjectFields(embeddedType);
			ProjectProperties(embeddedType);
			ProjectMethods(embeddedType);
			FreezeObject(typeValue);

			return embeddedType;
		}

		private void ProjectFields(EdgeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			EdgeJsValue typeValue = externalItem.ScriptValue;
			bool instance = externalItem.IsInstance;
			IList<EdgeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;

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
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

					if (instance && obj is null)
					{
						CreateAndSetTypeError(string.Format(
							CommonStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
						return undefinedValue;
					}

					object result;

					try
					{
						result = field.GetValue(obj);
					}
					catch (Exception e)
					{
						Exception exception = UnwrapException(e);
						var wrapperException = exception as WrapperException;
						EdgeJsValue errorValue;

						if (wrapperException is not null)
						{
							errorValue = CreateErrorFromWrapperException(wrapperException);
						}
						else
						{
							string errorMessage = instance ?
								string.Format(CommonStrings.Runtime_HostObjectFieldGettingFailed, fieldName,
									exception.Message)
								:
								string.Format(CommonStrings.Runtime_HostTypeFieldGettingFailed, fieldName, typeName,
									exception.Message)
								;
							errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						}
						EdgeJsContext.SetException(errorValue);

						return undefinedValue;
					}

					EdgeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				nativeFunctions.Add(nativeGetFunction);

				EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeGetFunction);
				descriptorValue.SetProperty("get", getMethodValue, true);

				if (!field.IsInitOnly)
				{
					EdgeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

						if (instance && obj is null)
						{
							CreateAndSetTypeError(string.Format(
								CommonStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
							return undefinedValue;
						}

						object value = MapToHostType(args[1]);
						ReflectionHelpers.FixFieldValueType(ref value, field);

						try
						{
							field.SetValue(obj, value);
						}
						catch (Exception e)
						{
							Exception exception = UnwrapException(e);
							var wrapperException = exception as WrapperException;
							EdgeJsValue errorValue;

							if (wrapperException is not null)
							{
								errorValue = CreateErrorFromWrapperException(wrapperException);
							}
							else
							{
								string errorMessage = instance ?
									string.Format(CommonStrings.Runtime_HostObjectFieldSettingFailed, fieldName,
										exception.Message)
									:
									string.Format(CommonStrings.Runtime_HostTypeFieldSettingFailed, fieldName, typeName,
										exception.Message)
									;
								errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							}
							EdgeJsContext.SetException(errorValue);

							return undefinedValue;
						}

						return undefinedValue;
					};
					nativeFunctions.Add(nativeSetFunction);

					EdgeJsValue setMethodValue = EdgeJsValue.CreateFunction(nativeSetFunction);
					descriptorValue.SetProperty("set", setMethodValue, true);
				}

				typeValue.DefineProperty(fieldName, descriptorValue);
			}
		}

		private void ProjectProperties(EdgeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			EdgeJsValue typeValue = externalItem.ScriptValue;
			IList<EdgeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;
			bool instance = externalItem.IsInstance;

			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			PropertyInfo[] properties = type.GetProperties(defaultBindingFlags);

			foreach (PropertyInfo property in properties)
			{
				if (!IsAvailableProperty(property))
				{
					continue;
				}

				string propertyName = property.Name;

				EdgeJsValue descriptorValue = EdgeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", EdgeJsValue.True, true);

				if (property.CanRead)
				{
					EdgeJsNativeFunction nativeGetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

						if (instance && obj is null)
						{
							CreateAndSetTypeError(string.Format(
								CommonStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							return undefinedValue;
						}

						object result;

						try
						{
							result = property.GetValue(obj, []);
						}
						catch (Exception e)
						{
							Exception exception = UnwrapException(e);
							var wrapperException = exception as WrapperException;
							EdgeJsValue errorValue;

							if (wrapperException is not null)
							{
								errorValue = CreateErrorFromWrapperException(wrapperException);
							}
							else
							{
								string errorMessage = instance ?
									string.Format(CommonStrings.Runtime_HostObjectPropertyGettingFailed, propertyName,
										exception.Message)
									:
									string.Format(CommonStrings.Runtime_HostTypePropertyGettingFailed, propertyName,
										typeName, exception.Message)
									;
								errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							}
							EdgeJsContext.SetException(errorValue);

							return undefinedValue;
						}

						EdgeJsValue resultValue = MapToScriptType(result);

						return resultValue;
					};
					nativeFunctions.Add(nativeGetFunction);

					EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeGetFunction);
					descriptorValue.SetProperty("get", getMethodValue, true);
				}

				if (property.CanWrite)
				{
					EdgeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

						if (instance && obj is null)
						{
							CreateAndSetTypeError(string.Format(
								CommonStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							return undefinedValue;
						}

						object value = MapToHostType(args[1]);
						ReflectionHelpers.FixPropertyValueType(ref value, property);

						try
						{
							property.SetValue(obj, value, []);
						}
						catch (Exception e)
						{
							Exception exception = UnwrapException(e);
							var wrapperException = exception as WrapperException;
							EdgeJsValue errorValue;

							if (wrapperException is not null)
							{
								errorValue = CreateErrorFromWrapperException(wrapperException);
							}
							else
							{
								string errorMessage = instance ?
									string.Format(CommonStrings.Runtime_HostObjectPropertySettingFailed, propertyName,
										exception.Message)
									:
									string.Format(CommonStrings.Runtime_HostTypePropertySettingFailed, propertyName,
										typeName, exception.Message)
									;
								errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							}
							EdgeJsContext.SetException(errorValue);

							return undefinedValue;
						}

						return undefinedValue;
					};
					nativeFunctions.Add(nativeSetFunction);

					EdgeJsValue setMethodValue = EdgeJsValue.CreateFunction(nativeSetFunction);
					descriptorValue.SetProperty("set", setMethodValue, true);
				}

				typeValue.DefineProperty(propertyName, descriptorValue);
			}
		}

		private void ProjectMethods(EdgeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			EdgeJsValue typeValue = externalItem.ScriptValue;
			IList<EdgeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;
			bool instance = externalItem.IsInstance;

			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(instance);
			MethodInfo[] methods = type.GetMethods(defaultBindingFlags);
			Dictionary<string, List<MethodInfo>> availableMethodGroups = GetAvailableMethodGroups(methods);

			foreach (KeyValuePair<string, List<MethodInfo>> methodGroup in availableMethodGroups)
			{
				string methodName = methodGroup.Key;
				MethodInfo[] methodCandidates = methodGroup.Value.ToArray();

				EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;

					if (instance && obj is null)
					{
						CreateAndSetTypeError(string.Format(
							CommonStrings.Runtime_InvalidThisContextForHostObjectMethod, methodName));
						return undefinedValue;
					}

					object[] processedArgs = GetHostItemMemberArguments(args);

					var bestFitMethod = (MethodInfo)ReflectionHelpers.GetBestFitMethod(
						methodCandidates, processedArgs);
					if (bestFitMethod is null)
					{
						CreateAndSetReferenceError(string.Format(
							CommonStrings.Runtime_SuitableMethodOfHostObjectNotFound, methodName));
						return undefinedValue;
					}

					ReflectionHelpers.FixArgumentTypes(ref processedArgs, bestFitMethod.GetParameters());

					object result;

					try
					{
						result = bestFitMethod.Invoke(obj, processedArgs);
					}
					catch (Exception e)
					{
						Exception exception = UnwrapException(e);
						var wrapperException = exception as WrapperException;
						EdgeJsValue errorValue;

						if (wrapperException is not null)
						{
							errorValue = CreateErrorFromWrapperException(wrapperException);
						}
						else
						{
							string errorMessage = instance ?
								string.Format(CommonStrings.Runtime_HostObjectMethodInvocationFailed, methodName,
									exception.Message)
								:
								string.Format(CommonStrings.Runtime_HostTypeMethodInvocationFailed, methodName, typeName,
									exception.Message)
								;
							errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						}
						EdgeJsContext.SetException(errorValue);

						return undefinedValue;
					}

					EdgeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				nativeFunctions.Add(nativeFunction);

				EdgeJsValue methodValue = EdgeJsValue.CreateFunction(nativeFunction);
				typeValue.SetProperty(methodName, methodValue, true);
			}
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private static void FreezeObject(EdgeJsValue objValue)
		{
			EdgeJsValue freezeMethodValue = EdgeJsValue.GlobalObject
				.GetProperty("Object")
				.GetProperty("freeze")
				;
			freezeMethodValue.CallFunction(objValue);
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private static void SetNonEnumerableProperty(EdgeJsValue objValue, string name, EdgeJsValue value)
		{
			EdgeJsValue descriptorValue = EdgeJsValue.CreateObject();
			descriptorValue.SetProperty("enumerable", EdgeJsValue.False, true);
			descriptorValue.SetProperty("writable", EdgeJsValue.True, true);

			EdgeJsPropertyId id = EdgeJsPropertyId.FromString(name);
			objValue.DefineProperty(id, descriptorValue);
			objValue.SetProperty(id, value, true);
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private static void CreateAndSetError(string message)
		{
			EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(message);
			EdgeJsContext.SetException(errorValue);
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private static void CreateAndSetReferenceError(string message)
		{
			EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateReferenceError(message);
			EdgeJsContext.SetException(errorValue);
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private static void CreateAndSetTypeError(string message)
		{
			EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(message);
			EdgeJsContext.SetException(errorValue);
		}

		private static EdgeJsValue CreateErrorFromWrapperException(WrapperException exception)
		{
			var originalException = (JsException)exception.InnerException;
			var originalScriptException = originalException as EdgeJsScriptException;
			EdgeJsValue errorValue = originalScriptException is not null ?
				originalScriptException.Error
				:
				EdgeJsErrorHelpers.CreateError(exception.Description)
				;

			return errorValue;
		}
	}
}
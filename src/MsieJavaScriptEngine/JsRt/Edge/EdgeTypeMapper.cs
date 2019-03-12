using System;
#if NETSTANDARD
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

using MsieJavaScriptEngine.Extensions;
using MsieJavaScriptEngine.Helpers;
#if NETSTANDARD
using MsieJavaScriptEngine.JsRt.Embedding;
using MsieJavaScriptEngine.Resources;
#endif
#if !NETSTANDARD
using MsieJavaScriptEngine.Utilities;
#endif

namespace MsieJavaScriptEngine.JsRt.Edge
{
#if NETSTANDARD
	using EdgeEmbeddedItem = EmbeddedItem<EdgeJsValue, EdgeJsNativeFunction>;
	using EdgeEmbeddedObject = EmbeddedObject<EdgeJsValue, EdgeJsNativeFunction>;
	using EdgeEmbeddedType = EmbeddedType<EdgeJsValue, EdgeJsNativeFunction>;

#endif
	/// <summary>
	/// “Edge” type mapper
	/// </summary>
	internal sealed class EdgeTypeMapper : TypeMapper<EdgeJsValue, EdgeJsNativeFunction>
	{
		/// <summary>
		/// Constructs an instance of the “Edge” type mapper
		/// </summary>
		public EdgeTypeMapper()
#if !NETSTANDARD
			: base(JsEngineMode.ChakraEdgeJsRt)
#endif
		{ }


#if !NETSTANDARD
		/// <summary>
		/// Creates a JavaScript value from an host object if the it does not already exist
		/// </summary>
		/// <param name="obj">Instance of host type</param>
		/// <returns>JavaScript value created from an host object</returns>
		public override EdgeJsValue GetOrCreateScriptObject(object obj)
		{
			var wrappedObj = new HostObject(obj, _engineMode);
			EdgeJsValue objValue = EdgeJsValue.FromObject(wrappedObj);

			return objValue;
		}

		/// <summary>
		/// Creates a JavaScript value from an host type if the it does not already exist
		/// </summary>
		/// <param name="type">Host type</param>
		/// <returns>JavaScript value created from an host type</returns>
		public override EdgeJsValue GetOrCreateScriptType(Type type)
		{
			var wrappedType = new HostType(type, _engineMode);
			EdgeJsValue typeValue = EdgeJsValue.FromObject(wrappedType);

			return typeValue;
		}
#endif
		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public override EdgeJsValue MapToScriptType(object value)
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
					return GetOrCreateScriptObject(value);
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
#if NETSTANDARD
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
#endif
				case JsValueType.Object:
#if !NETSTANDARD
				case JsValueType.Function:
#endif
				case JsValueType.Error:
				case JsValueType.Array:
#if NETSTANDARD
					result = value.HasExternalData ?
						GCHandle.FromIntPtr(value.ExternalData).Target : value.ConvertToObject();
#else
					EdgeJsValue processedValue = valueType != JsValueType.Object ?
						value.ConvertToObject() : value;
					object obj = processedValue.ToObject();
					var hostObj = obj as HostObject;
					result = hostObj != null ? hostObj.Target : obj;
#endif
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}
#if NETSTANDARD

		protected override EdgeEmbeddedObject CreateEmbeddedObjectOrFunction(object obj)
		{
			var del = obj as Delegate;
			EdgeEmbeddedObject embeddedObject = del != null ?
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
				object[] processedArgs = GetHostItemMemberArguments(args);
				MethodInfo method = del.GetMethodInfo();
				ParameterInfo[] parameters = method.GetParameters();

				ReflectionHelpers.FixArgumentTypes(ref processedArgs, parameters);

				object result;

				try
				{
					result = del.DynamicInvoke(processedArgs);
				}
				catch (Exception e)
				{
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostDelegateInvocationFailed, e.Message));
					EdgeJsErrorHelpers.SetException(errorValue);

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

			var embeddedObject = new EdgeEmbeddedObject(del, functionValue,
				new List<EdgeJsNativeFunction> { nativeFunction });

			return embeddedObject;
		}

		protected override EdgeEmbeddedType CreateEmbeddedType(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
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

				if (constructors.Length == 0)
				{
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorNotFound, typeName));
					EdgeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				var bestFitConstructor = (ConstructorInfo)ReflectionHelpers.GetBestFitMethod(
					constructors, processedArgs);
				if (bestFitConstructor == null)
				{
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
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
					EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
					EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorInvocationFailed, typeName, e.Message));
					EdgeJsErrorHelpers.SetException(errorValue);

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
					if (instance && obj == null)
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					object result;

					try
					{
						result = field.GetValue(obj);
					}
					catch (Exception e)
					{
						string errorMessage = instance ?
							string.Format(NetCoreStrings.Runtime_HostObjectFieldGettingFailed, fieldName, e.Message)
							:
							string.Format(NetCoreStrings.Runtime_HostTypeFieldGettingFailed, fieldName, typeName, e.Message)
							;

						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					EdgeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				nativeFunctions.Add(nativeGetFunction);

				EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeGetFunction);
				descriptorValue.SetProperty("get", getMethodValue, true);

				EdgeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					if (instance && obj == null)
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
						EdgeJsErrorHelpers.SetException(errorValue);

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
						string errorMessage = instance ?
							string.Format(NetCoreStrings.Runtime_HostObjectFieldSettingFailed, fieldName, e.Message)
							:
							string.Format(NetCoreStrings.Runtime_HostTypeFieldSettingFailed, fieldName, typeName, e.Message)
							;

						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					return EdgeJsValue.Undefined;
				};
				nativeFunctions.Add(nativeSetFunction);

				EdgeJsValue setMethodValue = EdgeJsValue.CreateFunction(nativeSetFunction);
				descriptorValue.SetProperty("set", setMethodValue, true);

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
				string propertyName = property.Name;

				EdgeJsValue descriptorValue = EdgeJsValue.CreateObject();
				descriptorValue.SetProperty("enumerable", EdgeJsValue.True, true);

				if (property.GetGetMethod() != null)
				{
					EdgeJsNativeFunction nativeGetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						if (instance && obj == null)
						{
							EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						object result;

						try
						{
							result = property.GetValue(obj, new object[0]);
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

							EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						EdgeJsValue resultValue = MapToScriptType(result);

						return resultValue;
					};
					nativeFunctions.Add(nativeGetFunction);

					EdgeJsValue getMethodValue = EdgeJsValue.CreateFunction(nativeGetFunction);
					descriptorValue.SetProperty("get", getMethodValue, true);
				}

				if (property.GetSetMethod() != null)
				{
					EdgeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						if (instance && obj == null)
						{
							EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						object value = MapToHostType(args[1]);
						ReflectionHelpers.FixPropertyValueType(ref value, property);

						try
						{
							property.SetValue(obj, value, new object[0]);
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

							EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
							EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
							EdgeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						return EdgeJsValue.Undefined;
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
			IEnumerable<MethodInfo> methods = type.GetMethods(defaultBindingFlags)
				.Where(ReflectionHelpers.IsFullyFledgedMethod);
			IEnumerable<IGrouping<string, MethodInfo>> methodGroups = methods.GroupBy(m => m.Name);

			foreach (IGrouping<string, MethodInfo> methodGroup in methodGroups)
			{
				string methodName = methodGroup.Key;
				MethodInfo[] methodCandidates = methodGroup.ToArray();

				EdgeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					if (instance && obj == null)
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectMethod, methodName));
						EdgeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					object[] processedArgs = GetHostItemMemberArguments(args);

					var bestFitMethod = (MethodInfo)ReflectionHelpers.GetBestFitMethod(
						methodCandidates, processedArgs);
					if (bestFitMethod == null)
					{
						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateReferenceError(
							string.Format(NetCoreStrings.Runtime_SuitableMethodOfHostObjectNotFound, methodName));
						EdgeJsErrorHelpers.SetException(errorValue);

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
						string errorMessage = instance ?
							string.Format(
								NetCoreStrings.Runtime_HostObjectMethodInvocationFailed, methodName, e.Message)
							:
							string.Format(
								NetCoreStrings.Runtime_HostTypeMethodInvocationFailed, methodName, typeName, e.Message)
							;

						EdgeJsValue undefinedValue = EdgeJsValue.Undefined;
						EdgeJsValue errorValue = EdgeJsErrorHelpers.CreateError(errorMessage);
						EdgeJsErrorHelpers.SetException(errorValue);

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
#endif
	}
}
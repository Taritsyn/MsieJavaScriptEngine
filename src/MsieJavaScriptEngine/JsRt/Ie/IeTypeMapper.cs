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

namespace MsieJavaScriptEngine.JsRt.Ie
{
#if NETSTANDARD
	using IeEmbeddedObject = EmbeddedObject<IeJsValue, IeJsNativeFunction>;
	using IeEmbeddedType = EmbeddedType<IeJsValue, IeJsNativeFunction>;
	using IeEmbeddedItem = EmbeddedItem<IeJsValue, IeJsNativeFunction>;

#endif
	/// <summary>
	/// “IE” type mapper
	/// </summary>
	internal sealed class IeTypeMapper : TypeMapper<IeJsValue, IeJsNativeFunction>
	{
		/// <summary>
		/// Constructs an instance of the “IE” type mapper
		/// </summary>
		public IeTypeMapper()
#if !NETSTANDARD
			: base(JsEngineMode.ChakraIeJsRt)
#endif
		{ }


#if !NETSTANDARD
		/// <summary>
		/// Creates a JavaScript value from an host object if the it does not already exist
		/// </summary>
		/// <param name="obj">Instance of host type</param>
		/// <returns>JavaScript value created from an host object</returns>
		public override IeJsValue GetOrCreateScriptObject(object obj)
		{
			var wrappedObj  = new HostObject(obj, _engineMode);
			IeJsValue objValue = IeJsValue.FromObject(wrappedObj);

			return objValue;
		}

		/// <summary>
		/// Creates a JavaScript value from an host type if the it does not already exist
		/// </summary>
		/// <param name="type">Host type</param>
		/// <returns>JavaScript value created from an host type</returns>
		public override IeJsValue GetOrCreateScriptType(Type type)
		{
			var wrappedType = new HostType(type, _engineMode);
			IeJsValue typeValue = IeJsValue.FromObject(wrappedType);

			return typeValue;
		}
#endif
		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public override IeJsValue MapToScriptType(object value)
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
					return GetOrCreateScriptObject(value);
			}
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public override object MapToHostType(IeJsValue value)
		{
			JsValueType valueType = value.ValueType;
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
					result = value.ToBoolean();
					break;
				case JsValueType.Number:
					result = NumericHelpers.CastDoubleValueToCorrectType(value.ToDouble());
					break;
				case JsValueType.String:
					result = value.ToString();
					break;
				case JsValueType.Object:
				case JsValueType.Function:
				case JsValueType.Error:
				case JsValueType.Array:
#if NETSTANDARD
					result = value.HasExternalData ?
						GCHandle.FromIntPtr(value.ExternalData).Target : value.ConvertToObject();
#else
					IeJsValue processedValue = valueType != JsValueType.Object ?
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

		protected override IeEmbeddedObject CreateEmbeddedObjectOrFunction(object obj)
		{
			var del = obj as Delegate;
			IeEmbeddedObject embeddedObject = del != null ?
				CreateEmbeddedFunction(del) : CreateEmbeddedObject(obj);

			return embeddedObject;
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private IeEmbeddedObject CreateEmbeddedObject(object obj)
		{
			GCHandle objHandle = GCHandle.Alloc(obj);
			IntPtr objPtr = GCHandle.ToIntPtr(objHandle);
			IeJsValue objValue = IeJsValue.CreateExternalObject(objPtr, _embeddedObjectFinalizeCallback);

			var embeddedObject = new IeEmbeddedObject(obj, objValue);

			ProjectFields(embeddedObject);
			ProjectProperties(embeddedObject);
			ProjectMethods(embeddedObject);
			FreezeObject(objValue);

			return embeddedObject;
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private IeEmbeddedObject CreateEmbeddedFunction(Delegate del)
		{
			IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
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
					IeJsValue undefinedValue = IeJsValue.Undefined;
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostDelegateInvocationFailed, e.Message));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				IeJsValue resultValue = MapToScriptType(result);

				return resultValue;
			};

			GCHandle delHandle = GCHandle.Alloc(del);
			IntPtr delPtr = GCHandle.ToIntPtr(delHandle);
			IeJsValue prototypeValue = IeJsValue.CreateExternalObject(delPtr, _embeddedObjectFinalizeCallback);

			IeJsValue functionValue = IeJsValue.CreateFunction(nativeFunction);
			functionValue.Prototype = prototypeValue;

			var embeddedObject = new IeEmbeddedObject(del, functionValue,
				new List<IeJsNativeFunction> { nativeFunction });

			return embeddedObject;
		}

		protected override IeEmbeddedType CreateEmbeddedType(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			string typeName = type.FullName;
			BindingFlags defaultBindingFlags = ReflectionHelpers.GetDefaultBindingFlags(true);
			ConstructorInfo[] constructors = type.GetConstructors(defaultBindingFlags);

			IeJsNativeFunction nativeConstructorFunction = (callee, isConstructCall, args, argCount, callbackData) =>
			{
				object result;
				IeJsValue resultValue;
				object[] processedArgs = GetHostItemMemberArguments(args);

				if (processedArgs.Length == 0 && typeInfo.IsValueType)
				{
					result = Activator.CreateInstance(type);
					resultValue = MapToScriptType(result);

					return resultValue;
				}

				if (constructors.Length == 0)
				{
					IeJsValue undefinedValue = IeJsValue.Undefined;
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorNotFound, typeName));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				var bestFitConstructor = (ConstructorInfo)ReflectionHelpers.GetBestFitMethod(
					constructors, processedArgs);
				if (bestFitConstructor == null)
				{
					IeJsValue undefinedValue = IeJsValue.Undefined;
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
					IeJsValue undefinedValue = IeJsValue.Undefined;
					IeJsValue errorValue = IeJsErrorHelpers.CreateError(
						string.Format(NetCoreStrings.Runtime_HostTypeConstructorInvocationFailed, typeName, e.Message));
					IeJsErrorHelpers.SetException(errorValue);

					return undefinedValue;
				}

				resultValue = MapToScriptType(result);

				return resultValue;
			};

			string embeddedTypeKey = type.AssemblyQualifiedName;
			GCHandle embeddedTypeKeyHandle = GCHandle.Alloc(embeddedTypeKey);
			IntPtr embeddedTypeKeyPtr = GCHandle.ToIntPtr(embeddedTypeKeyHandle);
			IeJsValue prototypeValue = IeJsValue.CreateExternalObject(embeddedTypeKeyPtr,
				_embeddedTypeFinalizeCallback);

			IeJsValue typeValue = IeJsValue.CreateFunction(nativeConstructorFunction);
			typeValue.Prototype = prototypeValue;

			var embeddedType = new IeEmbeddedType(type, typeValue,
				new List<IeJsNativeFunction> { nativeConstructorFunction });

			ProjectFields(embeddedType);
			ProjectProperties(embeddedType);
			ProjectMethods(embeddedType);
			FreezeObject(typeValue);

			return embeddedType;
		}

		private void ProjectFields(IeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			IeJsValue typeValue = externalItem.ScriptValue;
			bool instance = externalItem.IsInstance;
			IList<IeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;

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
					if (instance && obj == null)
					{
						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
						IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					IeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				nativeFunctions.Add(nativeGetFunction);

				IeJsValue getMethodValue = IeJsValue.CreateFunction(nativeGetFunction);
				descriptorValue.SetProperty("get", getMethodValue, true);

				IeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					if (instance && obj == null)
					{
						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectField, fieldName));
						IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					return IeJsValue.Undefined;
				};
				nativeFunctions.Add(nativeSetFunction);

				IeJsValue setMethodValue = IeJsValue.CreateFunction(nativeSetFunction);
				descriptorValue.SetProperty("set", setMethodValue, true);

				typeValue.DefineProperty(fieldName, descriptorValue);
			}
		}

		private void ProjectProperties(IeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			IeJsValue typeValue = externalItem.ScriptValue;
			IList<IeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;
			bool instance = externalItem.IsInstance;

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
					IeJsNativeFunction nativeGetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						if (instance && obj == null)
						{
							IeJsValue undefinedValue = IeJsValue.Undefined;
							IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							IeJsErrorHelpers.SetException(errorValue);

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

							IeJsValue undefinedValue = IeJsValue.Undefined;
							IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
							IeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						IeJsValue resultValue = MapToScriptType(result);

						return resultValue;
					};
					nativeFunctions.Add(nativeGetFunction);

					IeJsValue getMethodValue = IeJsValue.CreateFunction(nativeGetFunction);
					descriptorValue.SetProperty("get", getMethodValue, true);
				}

				if (property.GetSetMethod() != null)
				{
					IeJsNativeFunction nativeSetFunction = (callee, isConstructCall, args, argCount, callbackData) =>
					{
						if (instance && obj == null)
						{
							IeJsValue undefinedValue = IeJsValue.Undefined;
							IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
								string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectProperty, propertyName));
							IeJsErrorHelpers.SetException(errorValue);

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

							IeJsValue undefinedValue = IeJsValue.Undefined;
							IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
							IeJsErrorHelpers.SetException(errorValue);

							return undefinedValue;
						}

						return IeJsValue.Undefined;
					};
					nativeFunctions.Add(nativeSetFunction);

					IeJsValue setMethodValue = IeJsValue.CreateFunction(nativeSetFunction);
					descriptorValue.SetProperty("set", setMethodValue, true);
				}

				typeValue.DefineProperty(propertyName, descriptorValue);
			}
		}

		private void ProjectMethods(IeEmbeddedItem externalItem)
		{
			Type type = externalItem.HostType;
			object obj = externalItem.HostObject;
			IeJsValue typeValue = externalItem.ScriptValue;
			IList<IeJsNativeFunction> nativeFunctions = externalItem.NativeFunctions;
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

				IeJsNativeFunction nativeFunction = (callee, isConstructCall, args, argCount, callbackData) =>
				{
					if (instance && obj == null)
					{
						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateTypeError(
							string.Format(NetCoreStrings.Runtime_InvalidThisContextForHostObjectMethod, methodName));
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					object[] processedArgs = GetHostItemMemberArguments(args);

					var bestFitMethod = (MethodInfo)ReflectionHelpers.GetBestFitMethod(
						methodCandidates, processedArgs);
					if (bestFitMethod == null)
					{
						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateReferenceError(
							string.Format(NetCoreStrings.Runtime_SuitableMethodOfHostObjectNotFound, methodName));
						IeJsErrorHelpers.SetException(errorValue);

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

						IeJsValue undefinedValue = IeJsValue.Undefined;
						IeJsValue errorValue = IeJsErrorHelpers.CreateError(errorMessage);
						IeJsErrorHelpers.SetException(errorValue);

						return undefinedValue;
					}

					IeJsValue resultValue = MapToScriptType(result);

					return resultValue;
				};
				nativeFunctions.Add(nativeFunction);

				IeJsValue methodValue = IeJsValue.CreateFunction(nativeFunction);
				typeValue.SetProperty(methodName, methodValue, true);
			}
		}

		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private void FreezeObject(IeJsValue objValue)
		{
			IeJsValue objectValue = IeJsValue.GlobalObject.GetProperty("Object");
			IeJsValue freezeMethodValue = objectValue.GetProperty("freeze");

			freezeMethodValue.CallFunction(objectValue, objValue);
		}
#endif
	}
}
using System;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” native methods
	/// </summary>
	internal static class EdgeNativeMethods
	{
#if NETSTANDARD
		private const CallingConvention DefaultCallingConvention = CallingConvention.Cdecl;
#else
		private const CallingConvention DefaultCallingConvention = CallingConvention.StdCall;
#endif


		#region Hosting

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseScript(string script, JsSourceContext sourceContext,
			string sourceUrl, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunScript(string script, JsSourceContext sourceContext,
			string sourceUrl, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref uint bufferSize);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer,
			JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer,
			JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyIdFromName(string name, out EdgeJsPropertyId propertyId);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyNameFromId(EdgeJsPropertyId propertyId, out IntPtr buffer);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPointerToString(string value, UIntPtr stringLength,
			out EdgeJsValue stringValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStringToPointer(EdgeJsValue value, out IntPtr stringValue,
			out UIntPtr stringLength);

		#endregion

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes,
			JsThreadServiceCallback threadService, out EdgeJsRuntime runtime);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCollectGarbage(EdgeJsRuntime handle);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisposeRuntime(EdgeJsRuntime handle);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryUsage(EdgeJsRuntime runtime, out UIntPtr memoryUsage);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryLimit(EdgeJsRuntime runtime, out UIntPtr memoryLimit);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryLimit(EdgeJsRuntime runtime, UIntPtr memoryLimit);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(EdgeJsRuntime runtime,
			IntPtr callbackState, JsMemoryAllocationCallback allocationCallback);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(EdgeJsRuntime runtime,
			IntPtr callbackState, JsBeforeCollectCallback beforeCollectCallback);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode, EntryPoint = "JsAddRef")]
		internal static extern JsErrorCode JsContextAddRef(EdgeJsContext reference, out uint count);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsAddRef(EdgeJsValue reference, out uint count);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode, EntryPoint = "JsRelease")]
		internal static extern JsErrorCode JsContextRelease(EdgeJsContext reference, out uint count);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRelease(EdgeJsValue reference, out uint count);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateContext(EdgeJsRuntime runtime, out EdgeJsContext newContext);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetCurrentContext(out EdgeJsContext currentContext);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetCurrentContext(EdgeJsContext context);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntime(EdgeJsContext context, out EdgeJsRuntime runtime);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartDebugging();

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIdle(out uint nextIdleTick);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetUndefinedValue(out EdgeJsValue undefinedValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetNullValue(out EdgeJsValue nullValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetTrueValue(out EdgeJsValue trueValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetFalseValue(out EdgeJsValue falseValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBoolToBoolean(bool value, out EdgeJsValue booleanValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBooleanToBool(EdgeJsValue booleanValue, out bool boolValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToBoolean(EdgeJsValue value, out EdgeJsValue booleanValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetValueType(EdgeJsValue value, out JsValueType type);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out EdgeJsValue value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIntToNumber(int intValue, out EdgeJsValue value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsNumberToDouble(EdgeJsValue value, out double doubleValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsNumberToInt(EdgeJsValue value, out int intValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToNumber(EdgeJsValue value, out EdgeJsValue numberValue);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetStringLength(EdgeJsValue sringValue, out int length);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToString(EdgeJsValue value, out EdgeJsValue stringValue);
#if !NETSTANDARD

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsVariantToValue([MarshalAs(UnmanagedType.Struct)] ref object var,
			out EdgeJsValue value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsValueToVariant(EdgeJsValue obj,
			[MarshalAs(UnmanagedType.Struct)] out object var);
#endif

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetGlobalObject(out EdgeJsValue globalObject);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateObject(out EdgeJsValue obj);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateExternalObject(IntPtr data,
			JsObjectFinalizeCallback finalizeCallback, out EdgeJsValue obj);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToObject(EdgeJsValue value, out EdgeJsValue obj);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPrototype(EdgeJsValue obj, out EdgeJsValue prototypeObject);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetPrototype(EdgeJsValue obj, EdgeJsValue prototypeObject);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExtensionAllowed(EdgeJsValue obj, out bool value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPreventExtension(EdgeJsValue obj);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			out EdgeJsValue value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyDescriptor(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			out EdgeJsValue propertyDescriptor);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyNames(EdgeJsValue obj, out EdgeJsValue propertyNames);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			EdgeJsValue value, bool useStrictRules);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			out bool hasProperty);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			bool useStrictRules, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDefineProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId,
			EdgeJsValue propertyDescriptor, out bool result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasIndexedProperty(EdgeJsValue obj, EdgeJsValue index, out bool result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetIndexedProperty(EdgeJsValue obj, EdgeJsValue index,
			out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetIndexedProperty(EdgeJsValue obj, EdgeJsValue index,
			EdgeJsValue value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteIndexedProperty(EdgeJsValue obj, EdgeJsValue index);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEquals(EdgeJsValue obj1, EdgeJsValue obj2, out bool result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStrictEquals(EdgeJsValue obj1, EdgeJsValue obj2, out bool result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasExternalData(EdgeJsValue obj, out bool value);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExternalData(EdgeJsValue obj, out IntPtr externalData);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetExternalData(EdgeJsValue obj, IntPtr externalData);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateArray(uint length, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCallFunction(EdgeJsValue function, EdgeJsValue[] arguments,
			ushort argumentCount, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConstructObject(EdgeJsValue function, EdgeJsValue[] arguments,
			ushort argumentCount, out EdgeJsValue result);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateFunction(EdgeJsNativeFunction nativeFunction,
			IntPtr externalData, out EdgeJsValue function);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRangeError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateReferenceError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateSyntaxError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateTypeError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateURIError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasException(out bool hasException);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetAndClearException(out EdgeJsValue exception);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetException(EdgeJsValue exception);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisableRuntimeExecution(EdgeJsRuntime runtime);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEnableRuntimeExecution(EdgeJsRuntime runtime);

		[DllImport(DllName.Chakra, CallingConvention = DefaultCallingConvention, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(EdgeJsRuntime runtime, out bool isDisabled);
	}
}
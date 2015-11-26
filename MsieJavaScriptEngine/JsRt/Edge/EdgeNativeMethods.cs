namespace MsieJavaScriptEngine.JsRt.Edge
{
	using System;
	using System.Runtime.InteropServices;

	using JsRt;

	/// <summary>
	/// “Edge” native methods
	/// </summary>
	internal static class EdgeNativeMethods
	{
		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JsThreadServiceCallback threadService, out EdgeJsRuntime runtime);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCollectGarbage(EdgeJsRuntime handle);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisposeRuntime(EdgeJsRuntime handle);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryUsage(EdgeJsRuntime runtime, out UIntPtr memoryUsage);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryLimit(EdgeJsRuntime runtime, out UIntPtr memoryLimit);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryLimit(EdgeJsRuntime runtime, UIntPtr memoryLimit);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(EdgeJsRuntime runtime, IntPtr callbackState, JsMemoryAllocationCallback allocationCallback);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(EdgeJsRuntime runtime, IntPtr callbackState, JsBeforeCollectCallback beforeCollectCallback);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode, EntryPoint = "JsAddRef")]
		internal static extern JsErrorCode JsContextAddRef(EdgeJsContext reference, out uint count);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsAddRef(EdgeJsValue reference, out uint count);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode, EntryPoint = "JsRelease")]
		internal static extern JsErrorCode JsContextRelease(EdgeJsContext reference, out uint count);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRelease(EdgeJsValue reference, out uint count);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateContext(EdgeJsRuntime runtime, out EdgeJsContext newContext);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetCurrentContext(out EdgeJsContext currentContext);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetCurrentContext(EdgeJsContext context);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntime(EdgeJsContext context, out EdgeJsRuntime runtime);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartDebugging();

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIdle(out uint nextIdleTick);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseScript(string script, JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunScript(string script, JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext, string sourceUrl, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyIdFromName(string name, out EdgeJsPropertyId propertyId);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyNameFromId(EdgeJsPropertyId propertyId, out IntPtr buffer);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetUndefinedValue(out EdgeJsValue undefinedValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetNullValue(out EdgeJsValue nullValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetTrueValue(out EdgeJsValue trueValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetFalseValue(out EdgeJsValue falseValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBoolToBoolean(bool value, out EdgeJsValue booleanValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBooleanToBool(EdgeJsValue booleanValue, out bool boolValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToBoolean(EdgeJsValue value, out EdgeJsValue booleanValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetValueType(EdgeJsValue value, out JsValueType type);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out EdgeJsValue value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIntToNumber(int intValue, out EdgeJsValue value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsNumberToDouble(EdgeJsValue value, out double doubleValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToNumber(EdgeJsValue value, out EdgeJsValue numberValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetStringLength(EdgeJsValue sringValue, out int length);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPointerToString(string value, UIntPtr stringLength, out EdgeJsValue stringValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStringToPointer(EdgeJsValue value, out IntPtr stringValue, out UIntPtr stringLength);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToString(EdgeJsValue value, out EdgeJsValue stringValue);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsVariantToValue([MarshalAs(UnmanagedType.Struct)] ref object var, out EdgeJsValue value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsValueToVariant(EdgeJsValue obj, [MarshalAs(UnmanagedType.Struct)] out object var);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetGlobalObject(out EdgeJsValue globalObject);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateObject(out EdgeJsValue obj);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateExternalObject(IntPtr data, JsObjectFinalizeCallback finalizeCallback, out EdgeJsValue obj);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToObject(EdgeJsValue value, out EdgeJsValue obj);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPrototype(EdgeJsValue obj, out EdgeJsValue prototypeObject);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetPrototype(EdgeJsValue obj, EdgeJsValue prototypeObject);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExtensionAllowed(EdgeJsValue obj, out bool value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPreventExtension(EdgeJsValue obj);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId, out EdgeJsValue value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyDescriptor(EdgeJsValue obj, EdgeJsPropertyId propertyId, out EdgeJsValue propertyDescriptor);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyNames(EdgeJsValue obj, out EdgeJsValue propertyNames);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId, EdgeJsValue value, bool useStrictRules);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId, out bool hasProperty);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId, bool useStrictRules, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDefineProperty(EdgeJsValue obj, EdgeJsPropertyId propertyId, EdgeJsValue propertyDescriptor, out bool result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasIndexedProperty(EdgeJsValue obj, EdgeJsValue index, out bool result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetIndexedProperty(EdgeJsValue obj, EdgeJsValue index, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetIndexedProperty(EdgeJsValue obj, EdgeJsValue index, EdgeJsValue value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteIndexedProperty(EdgeJsValue obj, EdgeJsValue index);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEquals(EdgeJsValue obj1, EdgeJsValue obj2, out bool result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStrictEquals(EdgeJsValue obj1, EdgeJsValue obj2, out bool result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasExternalData(EdgeJsValue obj, out bool value);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExternalData(EdgeJsValue obj, out IntPtr externalData);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetExternalData(EdgeJsValue obj, IntPtr externalData);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateArray(uint length, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCallFunction(EdgeJsValue function, EdgeJsValue[] arguments, ushort argumentCount, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConstructObject(EdgeJsValue function, EdgeJsValue[] arguments, ushort argumentCount, out EdgeJsValue result);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateFunction(EdgeJsNativeFunction nativeFunction, IntPtr externalData, out EdgeJsValue function);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRangeError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateReferenceError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateSyntaxError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateTypeError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateURIError(EdgeJsValue message, out EdgeJsValue error);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasException(out bool hasException);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetAndClearException(out EdgeJsValue exception);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetException(EdgeJsValue exception);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisableRuntimeExecution(EdgeJsRuntime runtime);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEnableRuntimeExecution(EdgeJsRuntime runtime);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(EdgeJsRuntime runtime, out bool isDisabled);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartProfiling(IActiveScriptProfilerCallback callback, ProfilerEventMask eventMask, int context);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStopProfiling(int reason);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEnumerateHeap(out IActiveScriptProfilerHeapEnum enumerator);

		[DllImport("chakra.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIsEnumeratingHeap(out bool isEnumeratingHeap);
	}
}
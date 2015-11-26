namespace MsieJavaScriptEngine.JsRt.Ie
{
	using System;
	using System.Runtime.InteropServices;

	using JsRt;

	/// <summary>
	/// “IE” native methods
	/// </summary>
	internal static class IeNativeMethods
	{
		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JsRuntimeVersion runtimeVersion, JsThreadServiceCallback threadService, out IeJsRuntime runtime);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCollectGarbage(IeJsRuntime handle);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisposeRuntime(IeJsRuntime handle);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryUsage(IeJsRuntime runtime, out UIntPtr memoryUsage);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntimeMemoryLimit(IeJsRuntime runtime, out UIntPtr memoryLimit);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryLimit(IeJsRuntime runtime, UIntPtr memoryLimit);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(IeJsRuntime runtime, IntPtr callbackState, JsMemoryAllocationCallback allocationCallback);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(IeJsRuntime runtime, IntPtr callbackState, JsBeforeCollectCallback beforeCollectCallback);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode, EntryPoint = "JsAddRef")]
		internal static extern JsErrorCode JsContextAddRef(IeJsContext reference, out uint count);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsAddRef(IeJsValue reference, out uint count);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode, EntryPoint = "JsRelease")]
		internal static extern JsErrorCode JsContextRelease(IeJsContext reference, out uint count);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRelease(IeJsValue reference, out uint count);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateContext(IeJsRuntime runtime, IDebugApplication64 debugSite, out IeJsContext newContext);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateContext(IeJsRuntime runtime, IDebugApplication32 debugSite, out IeJsContext newContext);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetCurrentContext(out IeJsContext currentContext);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetCurrentContext(IeJsContext context);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetRuntime(IeJsContext context, out IeJsRuntime runtime);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartDebugging(IDebugApplication64 debugApplication);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartDebugging(IDebugApplication32 debugApplication);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIdle(out uint nextIdleTick);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseScript(string script, JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunScript(string script, JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyIdFromName(string name, out IeJsPropertyId propertyId);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyNameFromId(IeJsPropertyId propertyId, out IntPtr buffer);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetUndefinedValue(out IeJsValue undefinedValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetNullValue(out IeJsValue nullValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetTrueValue(out IeJsValue trueValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetFalseValue(out IeJsValue falseValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBoolToBoolean(bool value, out IeJsValue booleanValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsBooleanToBool(IeJsValue booleanValue, out bool boolValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToBoolean(IeJsValue value, out IeJsValue booleanValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetValueType(IeJsValue value, out JsValueType type);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out IeJsValue value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIntToNumber(int intValue, out IeJsValue value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsNumberToDouble(IeJsValue value, out double doubleValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToNumber(IeJsValue value, out IeJsValue numberValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetStringLength(IeJsValue sringValue, out int length);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPointerToString(string value, UIntPtr stringLength, out IeJsValue stringValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStringToPointer(IeJsValue value, out IntPtr stringValue, out UIntPtr stringLength);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToString(IeJsValue value, out IeJsValue stringValue);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsVariantToValue([MarshalAs(UnmanagedType.Struct)] ref object var, out IeJsValue value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsValueToVariant(IeJsValue obj, [MarshalAs(UnmanagedType.Struct)] out object var);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetGlobalObject(out IeJsValue globalObject);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateObject(out IeJsValue obj);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateExternalObject(IntPtr data, JsObjectFinalizeCallback finalizeCallback, out IeJsValue obj);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConvertValueToObject(IeJsValue value, out IeJsValue obj);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPrototype(IeJsValue obj, out IeJsValue prototypeObject);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetPrototype(IeJsValue obj, IeJsValue prototypeObject);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExtensionAllowed(IeJsValue obj, out bool value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPreventExtension(IeJsValue obj);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetProperty(IeJsValue obj, IeJsPropertyId propertyId, out IeJsValue value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyDescriptor(IeJsValue obj, IeJsPropertyId propertyId, out IeJsValue propertyDescriptor);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetOwnPropertyNames(IeJsValue obj, out IeJsValue propertyNames);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetProperty(IeJsValue obj, IeJsPropertyId propertyId, IeJsValue value, bool useStrictRules);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasProperty(IeJsValue obj, IeJsPropertyId propertyId, out bool hasProperty);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteProperty(IeJsValue obj, IeJsPropertyId propertyId, bool useStrictRules, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDefineProperty(IeJsValue obj, IeJsPropertyId propertyId, IeJsValue propertyDescriptor, out bool result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasIndexedProperty(IeJsValue obj, IeJsValue index, out bool result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetIndexedProperty(IeJsValue obj, IeJsValue index, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetIndexedProperty(IeJsValue obj, IeJsValue index, IeJsValue value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDeleteIndexedProperty(IeJsValue obj, IeJsValue index);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEquals(IeJsValue obj1, IeJsValue obj2, out bool result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStrictEquals(IeJsValue obj1, IeJsValue obj2, out bool result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasExternalData(IeJsValue obj, out bool value);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetExternalData(IeJsValue obj, out IntPtr externalData);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetExternalData(IeJsValue obj, IntPtr externalData);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateArray(uint length, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCallFunction(IeJsValue function, IeJsValue[] arguments, ushort argumentCount, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsConstructObject(IeJsValue function, IeJsValue[] arguments, ushort argumentCount, out IeJsValue result);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateFunction(IeJsNativeFunction nativeFunction, IntPtr externalData, out IeJsValue function);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateRangeError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateReferenceError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateSyntaxError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateTypeError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsCreateURIError(IeJsValue message, out IeJsValue error);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsHasException(out bool hasException);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetAndClearException(out IeJsValue exception);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSetException(IeJsValue exception);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsDisableRuntimeExecution(IeJsRuntime runtime);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEnableRuntimeExecution(IeJsRuntime runtime);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(IeJsRuntime runtime, out bool isDisabled);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStartProfiling(IActiveScriptProfilerCallback callback, ProfilerEventMask eventMask, int context);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsStopProfiling(int reason);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsEnumerateHeap(out IActiveScriptProfilerHeapEnum enumerator);

		[DllImport("jscript9.dll", CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsIsEnumeratingHeap(out bool isEnumeratingHeap);
	}
}
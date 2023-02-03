using System;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” native methods
	/// </summary>
	internal static class IeNativeMethods
	{
		#region Hosting

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseScript(string script, JsSourceContext sourceContext,
			string sourceUrl, out IeJsValue result);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunScript(string script, JsSourceContext sourceContext,
			string sourceUrl, out IeJsValue result);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsSerializeScript(string script, byte[] buffer, ref uint bufferSize);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsParseSerializedScript(string script, byte[] buffer,
			JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsRunSerializedScript(string script, byte[] buffer,
			JsSourceContext sourceContext, string sourceUrl, out IeJsValue result);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsGetPropertyIdFromName(string name, out IeJsPropertyId propertyId);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetPropertyNameFromId(IeJsPropertyId propertyId, out IntPtr buffer);

		[DllImport(DllName.JScript9, CharSet = CharSet.Unicode)]
		internal static extern JsErrorCode JsPointerToString(string value, UIntPtr stringLength,
			out IeJsValue stringValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsStringToPointer(IeJsValue value, out IntPtr stringValue,
			out UIntPtr stringLength);

		#endregion

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes,
			JsRuntimeVersion runtimeVersion, JsThreadServiceCallback threadService, out IeJsRuntime runtime);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCollectGarbage(IeJsRuntime handle);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDisposeRuntime(IeJsRuntime handle);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetRuntimeMemoryUsage(IeJsRuntime runtime, out UIntPtr memoryUsage);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetRuntimeMemoryLimit(IeJsRuntime runtime, out UIntPtr memoryLimit);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetRuntimeMemoryLimit(IeJsRuntime runtime, UIntPtr memoryLimit);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(IeJsRuntime runtime,
			IntPtr callbackState, JsMemoryAllocationCallback allocationCallback);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(IeJsRuntime runtime,
			IntPtr callbackState, JsBeforeCollectCallback beforeCollectCallback);

		[DllImport(DllName.JScript9, EntryPoint = "JsAddRef")]
		internal static extern JsErrorCode JsContextAddRef(IeJsContext reference, out uint count);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsAddRef(IeJsValue reference, out uint count);

		[DllImport(DllName.JScript9, EntryPoint = "JsRelease")]
		internal static extern JsErrorCode JsContextRelease(IeJsContext reference, out uint count);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsRelease(IeJsValue reference, out uint count);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateContext(IeJsRuntime runtime, IDebugApplication64 debugSite,
			out IeJsContext newContext);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateContext(IeJsRuntime runtime, IDebugApplication32 debugSite,
			out IeJsContext newContext);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetCurrentContext(out IeJsContext currentContext);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetCurrentContext(IeJsContext context);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetRuntime(IeJsContext context, out IeJsRuntime runtime);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsStartDebugging(IDebugApplication64 debugApplication);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsStartDebugging(IDebugApplication32 debugApplication);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsIdle(out uint nextIdleTick);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetUndefinedValue(out IeJsValue undefinedValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetNullValue(out IeJsValue nullValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetTrueValue(out IeJsValue trueValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetFalseValue(out IeJsValue falseValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsBoolToBoolean(bool value, out IeJsValue booleanValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsBooleanToBool(IeJsValue booleanValue, out bool boolValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsConvertValueToBoolean(IeJsValue value, out IeJsValue booleanValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetValueType(IeJsValue value, out JsValueType type);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out IeJsValue value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsIntToNumber(int intValue, out IeJsValue value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsNumberToDouble(IeJsValue value, out double doubleValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsConvertValueToNumber(IeJsValue value, out IeJsValue numberValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetStringLength(IeJsValue sringValue, out int length);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsConvertValueToString(IeJsValue value, out IeJsValue stringValue);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetGlobalObject(out IeJsValue globalObject);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateObject(out IeJsValue obj);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateExternalObject(IntPtr data,
			JsFinalizeCallback finalizeCallback, out IeJsValue obj);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsConvertValueToObject(IeJsValue value, out IeJsValue obj);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetPrototype(IeJsValue obj, out IeJsValue prototypeObject);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetPrototype(IeJsValue obj, IeJsValue prototypeObject);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetExtensionAllowed(IeJsValue obj, out bool value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsPreventExtension(IeJsValue obj);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetProperty(IeJsValue obj, IeJsPropertyId propertyId,
			out IeJsValue value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetOwnPropertyDescriptor(IeJsValue obj, IeJsPropertyId propertyId,
			out IeJsValue propertyDescriptor);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetOwnPropertyNames(IeJsValue obj, out IeJsValue propertyNames);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetProperty(IeJsValue obj, IeJsPropertyId propertyId, IeJsValue value,
			bool useStrictRules);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsHasProperty(IeJsValue obj, IeJsPropertyId propertyId,
			out bool hasProperty);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDeleteProperty(IeJsValue obj, IeJsPropertyId propertyId,
			bool useStrictRules, out IeJsValue result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDefineProperty(IeJsValue obj, IeJsPropertyId propertyId,
			IeJsValue propertyDescriptor, out bool result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsHasIndexedProperty(IeJsValue obj, IeJsValue index, out bool result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetIndexedProperty(IeJsValue obj, IeJsValue index,
			out IeJsValue result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetIndexedProperty(IeJsValue obj, IeJsValue index, IeJsValue value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDeleteIndexedProperty(IeJsValue obj, IeJsValue index);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsEquals(IeJsValue obj1, IeJsValue obj2, out bool result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsStrictEquals(IeJsValue obj1, IeJsValue obj2, out bool result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsHasExternalData(IeJsValue obj, out bool value);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetExternalData(IeJsValue obj, out IntPtr externalData);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetExternalData(IeJsValue obj, IntPtr externalData);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateArray(uint length, out IeJsValue result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCallFunction(IeJsValue function, IeJsValue[] arguments,
			ushort argumentCount, out IeJsValue result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsConstructObject(IeJsValue function, IeJsValue[] arguments,
			ushort argumentCount, out IeJsValue result);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateFunction(IeJsNativeFunction nativeFunction,
			IntPtr externalData, out IeJsValue function);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateRangeError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateReferenceError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateSyntaxError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateTypeError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsCreateURIError(IeJsValue message, out IeJsValue error);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsHasException(out bool hasException);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsGetAndClearException(out IeJsValue exception);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsSetException(IeJsValue exception);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsDisableRuntimeExecution(IeJsRuntime runtime);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsEnableRuntimeExecution(IeJsRuntime runtime);

		[DllImport(DllName.JScript9)]
		internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(IeJsRuntime runtime, out bool isDisabled);
	}
}
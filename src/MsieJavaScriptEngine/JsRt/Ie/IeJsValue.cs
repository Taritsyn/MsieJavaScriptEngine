using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” JavaScript value
	/// </summary>
	/// <remarks>
	/// The JavaScript value is one of the following types of values: Undefined, Null, Boolean,
	/// String, Number, or Object.
	/// </remarks>
	internal struct IeJsValue
	{
		/// <summary>
		/// The reference
		/// </summary>
		private readonly IntPtr _reference;

		/// <summary>
		/// Gets a invalid value
		/// </summary>
		public static IeJsValue Invalid
		{
			get { return new IeJsValue(IntPtr.Zero); }
		}

		/// <summary>
		/// Gets a value of <c>undefined</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static IeJsValue Undefined
		{
			get
			{
				IeJsValue value;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetUndefinedValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>null</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static IeJsValue Null
		{
			get
			{
				IeJsValue value;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetNullValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>true</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static IeJsValue True
		{
			get
			{
				IeJsValue value;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetTrueValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>false</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static IeJsValue False
		{
			get
			{
				IeJsValue value;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetFalseValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a global object in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static IeJsValue GlobalObject
		{
			get
			{
				IeJsValue value;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetGlobalObject(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the value is valid
		/// </summary>
		public bool IsValid
		{
			get { return _reference != IntPtr.Zero; }
		}

		/// <summary>
		/// Gets a JavaScript type of the value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The type of the value</returns>
		public JsValueType ValueType
		{
			get
			{
				JsValueType type;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetValueType(this, out type));

				return type;
			}
		}

		/// <summary>
		/// Gets a length of a <c>String</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The length of the string</returns>
		public int StringLength
		{
			get
			{
				int length;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetStringLength(this, out length));

				return length;
			}
		}

		/// <summary>
		/// Gets or sets a prototype of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public IeJsValue Prototype
		{
			get
			{
				IeJsValue prototypeReference;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetPrototype(this, out prototypeReference));

				return prototypeReference;
			}
			set
			{
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsSetPrototype(this, value));
			}
		}

		/// <summary>
		/// Gets a value indicating whether an object is extensible or not
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public bool IsExtensionAllowed
		{
			get
			{
				bool allowed;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetExtensionAllowed(this, out allowed));

				return allowed;
			}
		}

		/// <summary>
		/// Gets a value indicating whether an object is an external object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public bool HasExternalData
		{
			get
			{
				bool hasExternalData;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsHasExternalData(this, out hasExternalData));

				return hasExternalData;
			}
		}

		/// <summary>
		/// Gets or sets the data in an external object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public IntPtr ExternalData
		{
			get
			{
				IntPtr data;
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetExternalData(this, out data));

				return data;
			}
			set
			{
				IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsSetExternalData(this, value));
			}
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsValue"/> struct
		/// </summary>
		/// <param name="reference">The reference</param>
		private IeJsValue(IntPtr reference)
		{
			_reference = reference;
		}


		/// <summary>
		/// Creates a <c>Boolean</c> value from a <c>bool</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted</param>
		/// <returns>The converted value</returns>
		public static IeJsValue FromBoolean(bool value)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsBoolToBoolean(value, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a <c>Number</c> value from a <c>double</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted</param>
		/// <returns>The new <c>Number</c> value</returns>
		public static IeJsValue FromDouble(double value)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsDoubleToNumber(value, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a <c>Number</c> value from a <c>int</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="value">The value to be converted</param>
		/// <returns>The new <c>Number</c> value</returns>
		public static IeJsValue FromInt32(int value)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsIntToNumber(value, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a <c>String</c> value from a string pointer
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="value">The string  to convert to a <c>String</c> value</param>
		/// <returns>The new <c>String</c> value</returns>
		public static IeJsValue FromString(string value)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsPointerToString(value, new UIntPtr((uint)value.Length), out reference));

			return reference;
		}
#if !NETSTANDARD

		/// <summary>
		/// Creates a JavaScript value that is a projection of the passed in object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="value">The object to be projected</param>
		/// <returns>The JavaScript value that is a projection of the object</returns>
		public static IeJsValue FromObject(object value)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsVariantToValue(ref value, out reference));

			return reference;
		}
#endif

		/// <summary>
		/// Creates a new <c>Object</c>
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The new <c>Object</c></returns>
		public static IeJsValue CreateObject()
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateObject(out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new <c>Object</c> that stores some external data
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="data">External data that the object will represent. May be null.</param>
		/// <param name="finalizer">A callback for when the object is finalized. May be null.</param>
		/// <returns>The new <c>Object</c></returns>
		public static IeJsValue CreateExternalObject(IntPtr data, JsObjectFinalizeCallback finalizer)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateExternalObject(data, finalizer, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript function
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="function">The method to call when the function is invoked</param>
		/// <returns>The new function object</returns>
		public static IeJsValue CreateFunction(IeJsNativeFunction function)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateFunction(function, IntPtr.Zero, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript function
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="function">The method to call when the function is invoked</param>
		/// <param name="callbackData">Data to be provided to all function callbacks</param>
		/// <returns>The new function object</returns>
		public static IeJsValue CreateFunction(IeJsNativeFunction function, IntPtr callbackData)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateFunction(function, callbackData, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a JavaScript array object
		/// </summary>
		/// <remarks>
		/// Requires an active script context
		/// </remarks>
		/// <param name="length">The initial length of the array</param>
		/// <returns>The new array object</returns>
		public static IeJsValue CreateArray(uint length)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateArray(length, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript RangeError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateRangeError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateRangeError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript ReferenceError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateReferenceError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateReferenceError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript SyntaxError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateSyntaxError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateSyntaxError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript TypeError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateTypeError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateTypeError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new JavaScript URIError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">Message for the error object</param>
		/// <returns>The new error object</returns>
		public static IeJsValue CreateUriError(IeJsValue message)
		{
			IeJsValue reference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCreateURIError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Adds a reference to the object
		/// </summary>
		/// <remarks>
		/// This only needs to be called on objects that are not going to be stored somewhere on
		/// the stack. Calling AddRef ensures that the JavaScript object the value refers to will not be freed
		/// until Release is called
		/// </remarks>
		/// <returns>The object's new reference count</returns>
		public uint AddRef()
		{
			uint count;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsAddRef(this, out count));

			return count;
		}

		/// <summary>
		/// Releases a reference to the object
		/// </summary>
		/// <remarks>
		/// Removes a reference that was created by AddRef.
		/// </remarks>
		/// <returns>The object's new reference count</returns>
		public uint Release()
		{
			uint count;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsRelease(this, out count));

			return count;
		}

		/// <summary>
		/// Retrieves a <c>bool</c> value of a <c>Boolean</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public bool ToBoolean()
		{
			bool value;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsBooleanToBool(this, out value));

			return value;
		}

		/// <summary>
		/// Retrieves a <c>double</c> value of a <c>Number</c> value
		/// </summary>
		/// <remarks>
		/// <para>
		/// This function retrieves the value of a Number value. It will fail with
		/// <c>InvalidArgument</c> if the type of the value is not <c>Number</c>.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <returns>The <c>double</c> value</returns>
		public double ToDouble()
		{
			double value;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsNumberToDouble(this, out value));

			return value;
		}

		/// <summary>
		/// Retrieves a string pointer of a <c>String</c> value
		/// </summary>
		/// <remarks>
		/// <para>
		/// This function retrieves the string pointer of a <c>String</c> value. It will fail with
		/// <c>InvalidArgument</c> if the type of the value is not <c>String</c>.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <returns>The string</returns>
		public new string ToString()
		{
			IntPtr buffer;
			UIntPtr length;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsStringToPointer(this, out buffer, out length));

			return Marshal.PtrToStringUni(buffer, (int)length);
		}
#if !NETSTANDARD

		/// <summary>
		/// Retrieves a object representation of an <c>Object</c> value
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The object representation of the value</returns>
		public object ToObject()
		{
			object value;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsValueToVariant(this, out value));

			return value;
		}
#endif

		/// <summary>
		/// Converts a value to <c>Boolean</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public IeJsValue ConvertToBoolean()
		{
			IeJsValue booleanReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsConvertValueToBoolean(this, out booleanReference));

			return booleanReference;
		}

		/// <summary>
		/// Converts a value to <c>Number</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public IeJsValue ConvertToNumber()
		{
			IeJsValue numberReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsConvertValueToNumber(this, out numberReference));

			return numberReference;
		}

		/// <summary>
		/// Converts a value to <c>String</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public IeJsValue ConvertToString()
		{
			IeJsValue stringReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsConvertValueToString(this, out stringReference));

			return stringReference;
		}

		/// <summary>
		/// Converts a value to <c>Object</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public IeJsValue ConvertToObject()
		{
			IeJsValue objectReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsConvertValueToObject(this, out objectReference));

			return objectReference;
		}

		/// <summary>
		/// Sets a object to not be extensible
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public void PreventExtension()
		{
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsPreventExtension(this));
		}

		/// <summary>
		/// Gets a property descriptor for an object's own property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">The ID of the property</param>
		/// <returns>The property descriptor</returns>
		public IeJsValue GetOwnPropertyDescriptor(IeJsPropertyId propertyId)
		{
			IeJsValue descriptorReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetOwnPropertyDescriptor(this, propertyId, out descriptorReference));

			return descriptorReference;
		}

		/// <summary>
		/// Gets a list of all properties on the object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The array of property names</returns>
		public IeJsValue GetOwnPropertyNames()
		{
			IeJsValue propertyNamesReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetOwnPropertyNames(this, out propertyNamesReference));

			return propertyNamesReference;
		}

		/// <summary>
		/// Determines whether an object has a property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">The ID of the property</param>
		/// <returns>Whether the object (or a prototype) has the property</returns>
		public bool HasProperty(IeJsPropertyId propertyId)
		{
			bool hasProperty;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsHasProperty(this, propertyId, out hasProperty));

			return hasProperty;
		}

		/// <summary>
		/// Gets a object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="id">The ID of the property</param>
		/// <returns>The value of the property</returns>
		public IeJsValue GetProperty(IeJsPropertyId id)
		{
			IeJsValue propertyReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetProperty(this, id, out propertyReference));

			return propertyReference;
		}

		/// <summary>
		/// Sets a object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="id">The ID of the property</param>
		/// <param name="value">The new value of the property</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules</param>
		public void SetProperty(IeJsPropertyId id, IeJsValue value, bool useStrictRules)
		{
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsSetProperty(this, id, value, useStrictRules));
		}

		/// <summary>
		/// Deletes a object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">The ID of the property</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules</param>
		/// <returns>Whether the property was deleted</returns>
		public IeJsValue DeleteProperty(IeJsPropertyId propertyId, bool useStrictRules)
		{
			IeJsValue returnReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsDeleteProperty(this, propertyId, useStrictRules, out returnReference));

			return returnReference;
		}

		/// <summary>
		/// Defines a new object's own property from a property descriptor
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">The ID of the property</param>
		/// <param name="propertyDescriptor">The property descriptor</param>
		/// <returns>Whether the property was defined</returns>
		public bool DefineProperty(IeJsPropertyId propertyId, IeJsValue propertyDescriptor)
		{
			bool result;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsDefineProperty(this, propertyId, propertyDescriptor, out result));

			return result;
		}

		/// <summary>
		/// Test if an object has a value at the specified index
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to test</param>
		/// <returns>Whether the object has an value at the specified index</returns>
		public bool HasIndexedProperty(IeJsValue index)
		{
			bool hasProperty;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsHasIndexedProperty(this, index, out hasProperty));

			return hasProperty;
		}

		/// <summary>
		/// Retrieve a value at the specified index of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to retrieve</param>
		/// <returns>The retrieved value</returns>
		public IeJsValue GetIndexedProperty(IeJsValue index)
		{
			IeJsValue propertyReference;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsGetIndexedProperty(this, index, out propertyReference));

			return propertyReference;
		}

		/// <summary>
		/// Set a value at the specified index of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to set</param>
		/// <param name="value">The value to set</param>
		public void SetIndexedProperty(IeJsValue index, IeJsValue value)
		{
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsSetIndexedProperty(this, index, value));
		}

		/// <summary>
		/// Deletes a value at the specified index of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to delete</param>
		public void DeleteIndexedProperty(IeJsValue index)
		{
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsDeleteIndexedProperty(this, index));
		}

		/// <summary>
		/// Compare two JavaScript values for equality
		/// </summary>
		/// <remarks>
		/// <para>
		/// This function is equivalent to the "==" operator in JavaScript.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <param name="other">The object to compare</param>
		/// <returns>Whether the values are equal</returns>
		public bool Equals(IeJsValue other)
		{
			bool equals;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsEquals(this, other, out equals));

			return equals;
		}

		/// <summary>
		/// Compare two JavaScript values for strict equality
		/// </summary>
		/// <remarks>
		/// <para>
		/// This function is equivalent to the "===" operator in JavaScript.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <param name="other">The object to compare</param>
		/// <returns>Whether the values are strictly equal</returns>
		public bool StrictEquals(IeJsValue other)
		{
			bool equals;
			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsStrictEquals(this, other, out equals));

			return equals;
		}

		/// <summary>
		/// Invokes a function
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="arguments">The arguments to the call</param>
		/// <returns>The <c>Value</c> returned from the function invocation, if any</returns>
		public IeJsValue CallFunction(params IeJsValue[] arguments)
		{
			IeJsValue returnReference;

			if (arguments.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(arguments));
			}

			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsCallFunction(this, arguments, (ushort)arguments.Length, out returnReference));

			return returnReference;
		}

		/// <summary>
		/// Invokes a function as a constructor
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="arguments">The arguments to the call</param>
		/// <returns>The <c>Value</c> returned from the function invocation</returns>
		public IeJsValue ConstructObject(params IeJsValue[] arguments)
		{
			IeJsValue returnReference;

			if (arguments.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(arguments));
			}

			IeJsErrorHelpers.ThrowIfError(IeNativeMethods.JsConstructObject(this, arguments, (ushort)arguments.Length, out returnReference));

			return returnReference;
		}
	}
}
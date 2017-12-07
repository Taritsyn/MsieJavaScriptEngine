using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” JavaScript value
	/// </summary>
	/// <remarks>
	/// The JavaScript value is one of the following types of values: Undefined, Null, Boolean,
	/// String, Number, or Object.
	/// </remarks>
	internal struct EdgeJsValue
	{
		/// <summary>
		/// The reference
		/// </summary>
		private readonly IntPtr _reference;

		/// <summary>
		/// Gets a invalid value
		/// </summary>
		public static EdgeJsValue Invalid
		{
			get { return new EdgeJsValue(IntPtr.Zero); }
		}

		/// <summary>
		/// Gets a value of <c>undefined</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static EdgeJsValue Undefined
		{
			get
			{
				EdgeJsValue value;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetUndefinedValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>null</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static EdgeJsValue Null
		{
			get
			{
				EdgeJsValue value;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetNullValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>true</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static EdgeJsValue True
		{
			get
			{
				EdgeJsValue value;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetTrueValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a value of <c>false</c> in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static EdgeJsValue False
		{
			get
			{
				EdgeJsValue value;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetFalseValue(out value));

				return value;
			}
		}

		/// <summary>
		/// Gets a global object in the current script context
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public static EdgeJsValue GlobalObject
		{
			get
			{
				EdgeJsValue value;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetGlobalObject(out value));

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
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetValueType(this, out type));

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
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetStringLength(this, out length));

				return length;
			}
		}

		/// <summary>
		/// Gets or sets a prototype of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public EdgeJsValue Prototype
		{
			get
			{
				EdgeJsValue prototypeReference;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetPrototype(this, out prototypeReference));

				return prototypeReference;
			}
			set
			{
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetPrototype(this, value));
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
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetExtensionAllowed(this, out allowed));

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
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsHasExternalData(this, out hasExternalData));

				return hasExternalData;
			}
		}

		/// <summary>
		/// Gets or sets a data in an external object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		public IntPtr ExternalData
		{
			get
			{
				IntPtr data;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetExternalData(this, out data));

				return data;
			}
			set
			{
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetExternalData(this, value));
			}
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsValue"/> struct.
		/// </summary>
		/// <param name="reference">The reference</param>
		private EdgeJsValue(IntPtr reference)
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
		public static EdgeJsValue FromBoolean(bool value)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsBoolToBoolean(value, out reference));

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
		public static EdgeJsValue FromDouble(double value)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsDoubleToNumber(value, out reference));

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
		public static EdgeJsValue FromInt32(int value)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsIntToNumber(value, out reference));

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
		public static EdgeJsValue FromString(string value)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsPointerToString(value, new UIntPtr((uint)value.Length), out reference));

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
		public static EdgeJsValue FromObject(object value)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsVariantToValue(ref value, out reference));

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
		public static EdgeJsValue CreateObject()
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateObject(out reference));

			return reference;
		}

		/// <summary>
		/// Creates a new <c>Object</c> that stores some external data
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="data">External data that the object will represent. May be null</param>
		/// <param name="finalizer">The callback for when the object is finalized. May be null.</param>
		/// <returns>The new <c>Object</c></returns>
		public static EdgeJsValue CreateExternalObject(IntPtr data, JsObjectFinalizeCallback finalizer)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateExternalObject(data, finalizer, out reference));

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
		public static EdgeJsValue CreateFunction(EdgeJsNativeFunction function)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateFunction(function, IntPtr.Zero, out reference));

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
		public static EdgeJsValue CreateFunction(EdgeJsNativeFunction function, IntPtr callbackData)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateFunction(function, callbackData, out reference));

			return reference;
		}

		/// <summary>
		/// Creates a JavaScript array object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="length">The initial length of the array</param>
		/// <returns>The new array object</returns>
		public static EdgeJsValue CreateArray(uint length)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateArray(length, out reference));

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
		public static EdgeJsValue CreateError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateError(message, out reference));

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
		public static EdgeJsValue CreateRangeError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateRangeError(message, out reference));

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
		public static EdgeJsValue CreateReferenceError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateReferenceError(message, out reference));

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
		public static EdgeJsValue CreateSyntaxError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateSyntaxError(message, out reference));

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
		public static EdgeJsValue CreateTypeError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateTypeError(message, out reference));

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
		public static EdgeJsValue CreateUriError(EdgeJsValue message)
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCreateURIError(message, out reference));

			return reference;
		}

		/// <summary>
		/// Adds a reference to the object
		/// </summary>
		/// <remarks>
		/// This only needs to be called on objects that are not going to be stored somewhere on
		/// the stack. Calling AddRef ensures that the JavaScript object the value refers to will not be freed
		/// until Release is called.
		/// </remarks>
		/// <returns>The object's new reference count</returns>
		public uint AddRef()
		{
			uint count;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsAddRef(this, out count));

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
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsRelease(this, out count));

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
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsBooleanToBool(this, out value));

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
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsNumberToDouble(this, out value));

			return value;
		}

		/// <summary>
		/// Retrieves a <c>int</c> value of a <c>Number</c> value
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
		/// <returns>The <c>int</c> value</returns>
		public int ToInt32()
		{
			int value;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsNumberToInt(this, out value));

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

			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsStringToPointer(this, out buffer, out length));

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
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsValueToVariant(this, out value));

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
		public EdgeJsValue ConvertToBoolean()
		{
			EdgeJsValue booleanReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsConvertValueToBoolean(this, out booleanReference));

			return booleanReference;
		}

		/// <summary>
		/// Converts a value to <c>Number</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public EdgeJsValue ConvertToNumber()
		{
			EdgeJsValue numberReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsConvertValueToNumber(this, out numberReference));

			return numberReference;
		}

		/// <summary>
		/// Converts a value to <c>String</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public EdgeJsValue ConvertToString()
		{
			EdgeJsValue stringReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsConvertValueToString(this, out stringReference));

			return stringReference;
		}

		/// <summary>
		/// Converts a value to <c>Object</c> using regular JavaScript semantics
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>The converted value</returns>
		public EdgeJsValue ConvertToObject()
		{
			EdgeJsValue objectReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsConvertValueToObject(this, out objectReference));

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
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsPreventExtension(this));
		}

		/// <summary>
		/// Gets a property descriptor for an object's own property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="propertyId">The ID of the property</param>
		/// <returns>The property descriptor</returns>
		public EdgeJsValue GetOwnPropertyDescriptor(EdgeJsPropertyId propertyId)
		{
			EdgeJsValue descriptorReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetOwnPropertyDescriptor(this, propertyId, out descriptorReference));

			return descriptorReference;
		}

		/// <summary>
		/// Gets a list of all properties on the object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <returns>An array of property names</returns>
		public EdgeJsValue GetOwnPropertyNames()
		{
			EdgeJsValue propertyNamesReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetOwnPropertyNames(this, out propertyNamesReference));

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
		public bool HasProperty(EdgeJsPropertyId propertyId)
		{
			bool hasProperty;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsHasProperty(this, propertyId, out hasProperty));

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
		public EdgeJsValue GetProperty(EdgeJsPropertyId id)
		{
			EdgeJsValue propertyReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetProperty(this, id, out propertyReference));

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
		public void SetProperty(EdgeJsPropertyId id, EdgeJsValue value, bool useStrictRules)
		{
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetProperty(this, id, value, useStrictRules));
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
		public EdgeJsValue DeleteProperty(EdgeJsPropertyId propertyId, bool useStrictRules)
		{
			EdgeJsValue returnReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsDeleteProperty(this, propertyId, useStrictRules, out returnReference));

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
		public bool DefineProperty(EdgeJsPropertyId propertyId, EdgeJsValue propertyDescriptor)
		{
			bool result;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsDefineProperty(this, propertyId, propertyDescriptor, out result));

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
		public bool HasIndexedProperty(EdgeJsValue index)
		{
			bool hasProperty;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsHasIndexedProperty(this, index, out hasProperty));

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
		public EdgeJsValue GetIndexedProperty(EdgeJsValue index)
		{
			EdgeJsValue propertyReference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetIndexedProperty(this, index, out propertyReference));

			return propertyReference;
		}

		/// <summary>
		/// Sets a value at the specified index of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to set</param>
		/// <param name="value">The value to set</param>
		public void SetIndexedProperty(EdgeJsValue index, EdgeJsValue value)
		{
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetIndexedProperty(this, index, value));
		}

		/// <summary>
		/// Delete a value at the specified index of an object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="index">The index to delete</param>
		public void DeleteIndexedProperty(EdgeJsValue index)
		{
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsDeleteIndexedProperty(this, index));
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
		public bool Equals(EdgeJsValue other)
		{
			bool equals;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsEquals(this, other, out equals));

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
		public bool StrictEquals(EdgeJsValue other)
		{
			bool equals;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsStrictEquals(this, other, out equals));

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
		public EdgeJsValue CallFunction(params EdgeJsValue[] arguments)
		{
			EdgeJsValue returnReference;

			if (arguments.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException("arguments");
			}

			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsCallFunction(this, arguments, (ushort)arguments.Length, out returnReference));

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
		public EdgeJsValue ConstructObject(params EdgeJsValue[] arguments)
		{
			EdgeJsValue returnReference;

			if (arguments.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException("arguments");
			}

			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsConstructObject(this, arguments, (ushort)arguments.Length, out returnReference));

			return returnReference;
		}
	}
}
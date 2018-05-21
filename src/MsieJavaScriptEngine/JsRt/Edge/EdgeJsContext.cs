using System;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” script context
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each script context contains its own global object, distinct from the global object in
	/// other script contexts.
	/// </para>
	/// <para>
	/// Many Chakra hosting APIs require an "active" script context, which can be set using
	/// Current. Chakra hosting APIs that require a current context to be set will note
	/// that explicitly in their documentation.
	/// </para>
	/// </remarks>
	internal struct EdgeJsContext
	{
		/// <summary>
		/// The reference
		/// </summary>
		private readonly IntPtr _reference;

		/// <summary>
		/// Gets a invalid context
		/// </summary>
		public static EdgeJsContext Invalid
		{
			get { return new EdgeJsContext(IntPtr.Zero); }
		}

		/// <summary>
		/// Gets or sets a current script context on the thread
		/// </summary>
		public static EdgeJsContext Current
		{
			get
			{
				EdgeJsContext reference;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetCurrentContext(out reference));

				return reference;
			}
			set
			{
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetCurrentContext(value));
			}
		}

		/// <summary>
		/// Gets a value indicating whether the runtime of the current context is in an exception state
		/// </summary>
		/// <remarks>
		/// <para>
		/// If a call into the runtime results in an exception (either as the result of running a
		/// script or due to something like a conversion failure), the runtime is placed into an
		/// "exception state." All calls into any context created by the runtime (except for the
		/// exception APIs) will fail with <c>InExceptionState</c> until the exception is
		/// cleared.
		/// </para>
		/// <para>
		/// If the runtime of the current context is in the exception state when a callback returns
		/// into the engine, the engine will automatically rethrow the exception.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		public static bool HasException
		{
			get
			{
				bool hasException;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsHasException(out hasException));

				return hasException;
			}
		}

		/// <summary>
		/// Gets a runtime that the context belongs to
		/// </summary>
		public EdgeJsRuntime Runtime
		{
			get
			{
				EdgeJsRuntime handle;
				EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetRuntime(this, out handle));

				return handle;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the context is a valid context or not
		/// </summary>
		public bool IsValid
		{
			get { return _reference != IntPtr.Zero; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsContext"/> struct
		/// </summary>
		/// <param name="reference">The reference</param>
		internal EdgeJsContext(IntPtr reference)
		{
			_reference = reference;
		}


		/// <summary>
		/// Tells a runtime to do any idle processing it need to do
		/// </summary>
		/// <remarks>
		/// <para>
		/// If idle processing has been enabled for the current runtime, calling <c>Idle</c> will
		/// inform the current runtime that the host is idle and that the runtime can perform
		/// memory cleanup tasks.
		/// </para>
		/// <para>
		/// <c>Idle</c> will also return the number of system ticks until there will be more idle work
		/// for the runtime to do. Calling <c>Idle</c> before this number of ticks has passed will do
		/// no work.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <returns>The next system tick when there will be more idle work to do. Returns the
		/// maximum number of ticks if there no upcoming idle work to do.</returns>
		public static uint Idle()
		{
			uint ticks;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsIdle(out ticks));

			return ticks;
		}

		/// <summary>
		/// Parses a script and returns a function representing the script
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to parse</param>
		/// <returns>A function representing the script code</returns>
		public static EdgeJsValue ParseScript(string script)
		{
			return ParseScript(script, JsSourceContext.None, string.Empty);
		}

		/// <summary>
		/// Parses a script and returns a function representing the script
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to parse</param>
		/// <param name="sourceContext">A cookie identifying the script that can be used
		/// by script contexts that have debugging enabled</param>
		/// <param name="sourceName">The location the script came from</param>
		/// <returns>A function representing the script code</returns>
		public static EdgeJsValue ParseScript(string script, JsSourceContext sourceContext, string sourceName)
		{
			EdgeJsValue result;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsParseScript(script, sourceContext, sourceName,
				out result));

			return result;
		}

		/// <summary>
		/// Parses a serialized script and returns a function representing the script
		/// </summary>
		/// <remarks>
		/// <para>Requires an active script context.</para>
		/// <para>The runtime will hold on to the buffer until all instances of any functions created from
		/// the buffer are garbage collected.</para>
		/// </remarks>
		/// <param name="script">The script to parse</param>
		/// <param name="buffer">The serialized script</param>
		/// <returns>A function representing the script code</returns>
		public static EdgeJsValue ParseSerializedScript(string script, byte[] buffer)
		{
			return ParseSerializedScript(script, buffer, JsSourceContext.None, string.Empty);
		}

		/// <summary>
		/// Parses a serialized script and returns a function representing the script
		/// </summary>
		/// <remarks>
		/// <para>Requires an active script context.</para>
		/// <para>The runtime will hold on to the buffer until all instances of any functions created from
		/// the buffer are garbage collected.</para>
		/// </remarks>
		/// <param name="script">The script to parse</param>
		/// <param name="buffer">The serialized script</param>
		/// <param name="sourceContext">A cookie identifying the script that can be used
		/// by script contexts that have debugging enabled</param>
		/// <param name="sourceName">The location the script came from</param>
		/// <returns>A function representing the script code</returns>
		public static EdgeJsValue ParseSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext,
			string sourceName)
		{
			EdgeJsValue result;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsParseSerializedScript(script, buffer, sourceContext,
				sourceName, out result));

			return result;
		}

		/// <summary>
		/// Executes a script
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to run</param>
		/// <returns>The result of the script, if any</returns>
		public static EdgeJsValue RunScript(string script)
		{
			return RunScript(script, JsSourceContext.None, string.Empty);
		}

		/// <summary>
		/// Executes a script
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="script">The script to run</param>
		/// <param name="sourceContext">A cookie identifying the script that can be used
		/// by script contexts that have debugging enabled</param>
		/// <param name="sourceName">The location the script came from</param>
		/// <returns>The result of the script, if any</returns>
		public static EdgeJsValue RunScript(string script, JsSourceContext sourceContext, string sourceName)
		{
			EdgeJsValue result;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsRunScript(script, sourceContext, sourceName,
				out result));

			return result;
		}

		/// <summary>
		/// Runs a serialized script
		/// </summary>
		/// <remarks>
		/// <para>Requires an active script context.</para>
		/// <para>The runtime will hold on to the buffer until all instances of any functions created from
		/// the buffer are garbage collected.</para>
		/// </remarks>
		/// <param name="script">The source code of the serialized script</param>
		/// <param name="buffer">The serialized script</param>
		/// <returns>The result of the script, if any</returns>
		public static EdgeJsValue RunSerializedScript(string script, byte[] buffer)
		{
			return RunSerializedScript(script, buffer, JsSourceContext.None, string.Empty);
		}

		/// <summary>
		/// Runs a serialized script
		/// </summary>
		/// <remarks>
		/// <para>Requires an active script context.</para>
		/// <para>The runtime will hold on to the buffer until all instances of any functions created from
		/// the buffer are garbage collected.</para>
		/// </remarks>
		/// <param name="script">The source code of the serialized script</param>
		/// <param name="buffer">The serialized script</param>
		/// <param name="sourceContext">A cookie identifying the script that can be used
		/// by script contexts that have debugging enabled</param>
		/// <param name="sourceName">The location the script came from</param>
		/// <returns>The result of the script, if any</returns>
		public static EdgeJsValue RunSerializedScript(string script, byte[] buffer, JsSourceContext sourceContext,
			string sourceName)
		{
			EdgeJsValue result;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsRunSerializedScript(script, buffer, sourceContext,
				sourceName, out result));

			return result;
		}

		/// <summary>
		/// Serializes a parsed script to a buffer than can be reused
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>SerializeScript</c> parses a script and then stores the parsed form of the script in a
		/// runtime-independent format. The serialized script then can be deserialized in any
		/// runtime without requiring the script to be re-parsed.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <param name="script">The script to serialize</param>
		/// <returns>The buffer to put the serialized script into</returns>
		public static byte[] SerializeScript(string script)
		{
			byte[] buffer = null;
			uint bufferSize = 0;

			JsErrorCode errorCode = EdgeNativeMethods.JsSerializeScript(script, buffer, ref bufferSize);
			EdgeJsErrorHelpers.ThrowIfError(errorCode);

			buffer = new byte[(int)bufferSize];

			errorCode = EdgeNativeMethods.JsSerializeScript(script, buffer, ref bufferSize);
			EdgeJsErrorHelpers.ThrowIfError(errorCode);

			return buffer;
		}

		/// <summary>
		/// Returns a exception that caused the runtime of the current context to be in the
		/// exception state and resets the exception state for that runtime
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the runtime of the current context is not in an exception state, this API will throw
		/// <c>JsErrorInvalidArgument</c>. If the runtime is disabled, this will return an exception
		/// indicating that the script was terminated, but it will not clear the exception (the
		/// exception will be cleared if the runtime is re-enabled using
		/// <c>EnableRuntimeExecution</c>).
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <returns>The exception for the runtime of the current context</returns>
		public static EdgeJsValue GetAndClearException()
		{
			EdgeJsValue reference;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsGetAndClearException(out reference));

			return reference;
		}

		/// <summary>
		/// Sets a runtime of the current context to an exception state
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the runtime of the current context is already in an exception state, this API will
		/// throw <c>JsErrorInExceptionState</c>.
		/// </para>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <param name="exception">The JavaScript exception to set for the runtime of the current context</param>
		public static void SetException(EdgeJsValue exception)
		{
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsSetException(exception));
		}

		/// <summary>
		/// Starts debugging in the context
		/// </summary>
		public static void StartDebugging()
		{
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsStartDebugging());
		}

		/// <summary>
		/// Adds a reference to a script context
		/// </summary>
		/// <remarks>
		/// Calling AddRef ensures that the context will not be freed until Release is called.
		/// </remarks>
		/// <returns>The object's new reference count</returns>
		public uint AddRef()
		{
			uint count;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsContextAddRef(this, out count));

			return count;
		}

		/// <summary>
		/// Releases a reference to a script context
		/// </summary>
		/// <remarks>
		/// Removes a reference to a context that was created by AddRef.
		/// </remarks>
		/// <returns>The object's new reference count</returns>
		public uint Release()
		{
			uint count;
			EdgeJsErrorHelpers.ThrowIfError(EdgeNativeMethods.JsContextRelease(this, out count));

			return count;
		}
	}
}
namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” error helpers
	/// </summary>
	internal static class EdgeJsErrorHelpers
	{
		/// <summary>
		/// Throws if a native method returns an error code
		/// </summary>
		/// <param name="error">The error</param>
		public static void ThrowIfError(JsErrorCode error)
		{
			if (error != JsErrorCode.NoError)
			{
				switch (error)
				{
					#region Usage

					case JsErrorCode.InvalidArgument:
						throw new JsUsageException(error, "Invalid argument.");

					case JsErrorCode.NullArgument:
						throw new JsUsageException(error, "Null argument.");

					case JsErrorCode.NoCurrentContext:
						throw new JsUsageException(error, "No current context.");

					case JsErrorCode.InExceptionState:
						throw new JsUsageException(error, "Runtime is in exception state.");

					case JsErrorCode.NotImplemented:
						throw new JsUsageException(error, "Method is not implemented.");

					case JsErrorCode.WrongThread:
						throw new JsUsageException(error, "Runtime is active on another thread.");

					case JsErrorCode.RuntimeInUse:
						throw new JsUsageException(error, "Runtime is in use.");

					case JsErrorCode.BadSerializedScript:
						throw new JsUsageException(error, "Bad serialized script.");

					case JsErrorCode.InDisabledState:
						throw new JsUsageException(error, "Runtime is disabled.");

					case JsErrorCode.CannotDisableExecution:
						throw new JsUsageException(error, "Cannot disable execution.");

					case JsErrorCode.HeapEnumInProgress:
						throw new JsUsageException(error, "Heap enumeration is in progress.");

					case JsErrorCode.ArgumentNotObject:
						throw new JsUsageException(error, "Argument is not an object.");

					case JsErrorCode.InProfileCallback:
						throw new JsUsageException(error, "In a profile callback.");

					case JsErrorCode.InThreadServiceCallback:
						throw new JsUsageException(error, "In a thread service callback.");

					case JsErrorCode.CannotSerializeDebugScript:
						throw new JsUsageException(error, "Cannot serialize a debug script.");

					case JsErrorCode.AlreadyDebuggingContext:
						throw new JsUsageException(error, "Context is already in debug mode.");

					case JsErrorCode.AlreadyProfilingContext:
						throw new JsUsageException(error, "Already profiling this context.");

					case JsErrorCode.IdleNotEnabled:
						throw new JsUsageException(error, "Idle is not enabled.");

					#endregion

					#region Engine

					case JsErrorCode.OutOfMemory:
						throw new JsEngineException(error, "Out of memory.");

					#endregion

					#region Script

					case JsErrorCode.ScriptException:
						{
							EdgeJsValue errorObject;
							JsErrorCode innerError = EdgeNativeMethods.JsGetAndClearException(out errorObject);

							if (innerError != JsErrorCode.NoError)
							{
								throw new JsFatalException(innerError);
							}

							throw new EdgeJsScriptException(error, errorObject, "Script threw an exception.");
						}

					case JsErrorCode.ScriptCompile:
						{
							EdgeJsValue errorObject;
							JsErrorCode innerError = EdgeNativeMethods.JsGetAndClearException(out errorObject);

							if (innerError != JsErrorCode.NoError)
							{
								throw new JsFatalException(innerError);
							}

							throw new EdgeJsScriptException(error, errorObject, "Compile error.");
						}

					case JsErrorCode.ScriptTerminated:
						throw new EdgeJsScriptException(error, EdgeJsValue.Invalid, "Script was terminated.");

					case JsErrorCode.ScriptEvalDisabled:
						throw new EdgeJsScriptException(error, EdgeJsValue.Invalid, "Eval of strings is disabled in this runtime.");

					#endregion

					#region Fatal

					case JsErrorCode.Fatal:
						throw new JsFatalException(error);

					#endregion

					default:
						throw new JsFatalException(error);
				}
			}
		}


		/// <summary>
		/// Creates a new JavaScript error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Creates a new JavaScript RangeError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateRangeError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateRangeError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Creates a new JavaScript ReferenceError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateReferenceError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateReferenceError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Creates a new JavaScript SyntaxError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateSyntaxError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateSyntaxError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Creates a new JavaScript TypeError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateTypeError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateTypeError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Creates a new JavaScript URIError error object
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="message">The message that describes the error</param>
		/// <returns>The new error object</returns>
		public static EdgeJsValue CreateUriError(string message)
		{
			EdgeJsValue messageValue = EdgeJsValue.FromString(message);
			EdgeJsValue errorValue = EdgeJsValue.CreateUriError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Sets a exception
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="exception">The error object</param>
		public static void SetException(EdgeJsValue exception)
		{
			JsErrorCode innerError = EdgeNativeMethods.JsSetException(exception);
			if (innerError != JsErrorCode.NoError)
			{
				throw new JsFatalException(innerError);
			}
		}
	}
}
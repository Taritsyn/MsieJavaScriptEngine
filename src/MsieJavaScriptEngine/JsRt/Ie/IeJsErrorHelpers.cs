namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” error helpers
	/// </summary>
	internal static class IeJsErrorHelpers
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

					case JsErrorCode.AlreadyDebuggingContext:
						throw new JsUsageException(error, "Context is already in debug mode.");

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

					case JsErrorCode.AlreadyProfilingContext:
						throw new JsUsageException(error, "Already profiling this context.");

					case JsErrorCode.IdleNotEnabled:
						throw new JsUsageException(error, "Idle is not enabled.");

					case JsErrorCode.OutOfMemory:
						throw new JsEngineException(error, "Out of memory.");

					case JsErrorCode.ScriptException:
						{
							IeJsValue errorObject;
							JsErrorCode innerError = IeNativeMethods.JsGetAndClearException(out errorObject);

							if (innerError != JsErrorCode.NoError)
							{
								throw new JsFatalException(innerError);
							}

							throw new IeJsScriptException(error, errorObject, "Script threw an exception.");
						}

					case JsErrorCode.ScriptCompile:
						{
							IeJsValue errorObject;
							JsErrorCode innerError = IeNativeMethods.JsGetAndClearException(out errorObject);

							if (innerError != JsErrorCode.NoError)
							{
								throw new JsFatalException(innerError);
							}

							throw new IeJsScriptException(error, errorObject, "Compile error.");
						}

					case JsErrorCode.ScriptTerminated:
						throw new IeJsScriptException(error, IeJsValue.Invalid, "Script was terminated.");

					case JsErrorCode.ScriptEvalDisabled:
						throw new IeJsScriptException(error, IeJsValue.Invalid, "Eval of strings is disabled in this runtime.");

					case JsErrorCode.Fatal:
						throw new JsFatalException(error);

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
		public static IeJsValue CreateError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateError(messageValue);

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
		public static IeJsValue CreateRangeError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateRangeError(messageValue);

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
		public static IeJsValue CreateReferenceError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateReferenceError(messageValue);

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
		public static IeJsValue CreateSyntaxError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateSyntaxError(messageValue);

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
		public static IeJsValue CreateTypeError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateTypeError(messageValue);

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
		public static IeJsValue CreateUriError(string message)
		{
			IeJsValue messageValue = IeJsValue.FromString(message);
			IeJsValue errorValue = IeJsValue.CreateUriError(messageValue);

			return errorValue;
		}

		/// <summary>
		/// Sets a exception
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="exception">The error object</param>
		public static void SetException(IeJsValue exception)
		{
			JsErrorCode innerError = IeNativeMethods.JsSetException(exception);
			if (innerError != JsErrorCode.NoError)
			{
				throw new JsFatalException(innerError);
			}
		}
	}
}
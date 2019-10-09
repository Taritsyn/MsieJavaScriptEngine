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
		/// <param name="errorCode">The error code</param>
		public static void ThrowIfError(JsErrorCode errorCode)
		{
			if (errorCode != JsErrorCode.NoError)
			{
				switch (errorCode)
				{
					#region Usage

					case JsErrorCode.InvalidArgument:
						throw new JsUsageException(errorCode, "Invalid argument.");

					case JsErrorCode.NullArgument:
						throw new JsUsageException(errorCode, "Null argument.");

					case JsErrorCode.NoCurrentContext:
						throw new JsUsageException(errorCode, "No current context.");

					case JsErrorCode.InExceptionState:
						throw new JsUsageException(errorCode, "Runtime is in exception state.");

					case JsErrorCode.NotImplemented:
						throw new JsUsageException(errorCode, "Method is not implemented.");

					case JsErrorCode.WrongThread:
						throw new JsUsageException(errorCode, "Runtime is active on another thread.");

					case JsErrorCode.RuntimeInUse:
						throw new JsUsageException(errorCode, "Runtime is in use.");

					case JsErrorCode.BadSerializedScript:
						throw new JsUsageException(errorCode, "Bad serialized script.");

					case JsErrorCode.InDisabledState:
						throw new JsUsageException(errorCode, "Runtime is disabled.");

					case JsErrorCode.CannotDisableExecution:
						throw new JsUsageException(errorCode, "Cannot disable execution.");

					case JsErrorCode.HeapEnumInProgress:
						throw new JsUsageException(errorCode, "Heap enumeration is in progress.");

					case JsErrorCode.ArgumentNotObject:
						throw new JsUsageException(errorCode, "Argument is not an object.");

					case JsErrorCode.InProfileCallback:
						throw new JsUsageException(errorCode, "In a profile callback.");

					case JsErrorCode.InThreadServiceCallback:
						throw new JsUsageException(errorCode, "In a thread service callback.");

					case JsErrorCode.CannotSerializeDebugScript:
						throw new JsUsageException(errorCode, "Cannot serialize a debug script.");

					case JsErrorCode.AlreadyDebuggingContext:
						throw new JsUsageException(errorCode, "Context is already in debug mode.");

					case JsErrorCode.AlreadyProfilingContext:
						throw new JsUsageException(errorCode, "Already profiling this context.");

					case JsErrorCode.IdleNotEnabled:
						throw new JsUsageException(errorCode, "Idle is not enabled.");

					#endregion

					#region Engine

					case JsErrorCode.OutOfMemory:
						throw new JsEngineException(errorCode, "Out of memory.");

					#endregion

					#region Script

					case JsErrorCode.ScriptException:
					case JsErrorCode.ScriptCompile:
						{
							IeJsValue errorObject;
							JsErrorCode innerErrorCode = IeNativeMethods.JsGetAndClearException(out errorObject);

							if (innerErrorCode != JsErrorCode.NoError)
							{
								throw new JsFatalException(innerErrorCode);
							}

							string message = errorCode == JsErrorCode.ScriptCompile ?
								"Compile error." : "Script threw an exception.";

							throw new IeJsScriptException(errorCode, errorObject, message);
						}

					case JsErrorCode.ScriptTerminated:
						throw new IeJsScriptException(errorCode, IeJsValue.Invalid, "Script was terminated.");

					case JsErrorCode.ScriptEvalDisabled:
						throw new IeJsScriptException(errorCode, IeJsValue.Invalid, "Eval of strings is disabled in this runtime.");

					#endregion

					#region Fatal

					case JsErrorCode.Fatal:
						throw new JsFatalException(errorCode);

					#endregion

					default:
						throw new JsFatalException(errorCode);
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
	}
}
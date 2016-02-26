namespace MsieJavaScriptEngine
{
	using System;
	using System.Reflection;
	using System.Text;

	using ActiveScript;
	using Helpers;
	using JsRt.Edge;
	using JsRt.Ie;
	using Resources;
	using Utilities;

	/// <summary>
	/// .NET-wrapper for working with the Internet Explorer's JavaScript engines
	/// </summary>
	public sealed class MsieJsEngine : IDisposable
	{
		/// <summary>
		/// JavaScript engine
		/// </summary>
		private IInnerJsEngine _jsEngine;

		/// <summary>
		/// Current JavaScript engine mode
		/// </summary>
		private static JsEngineMode _currentMode;

		/// <summary>
		/// Synchronizer of JavaScript engines creation
		/// </summary>
		private static readonly object _creationSynchronizer = new object();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// Gets a name of JavaScript engine mode
		/// </summary>
		public string Mode
		{
			get { return _jsEngine.Mode; }
		}


		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <exception cref="MsieJavaScriptEngine.JsEngineLoadException">Failed to load a JavaScript engine.</exception>
		/// <exception cref="System.NotSupportedException">Selected mode of JavaScript engine is not supported.</exception>
		public MsieJsEngine()
			: this(new JsEngineSettings())
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="settings">JavaScript engine settings</param>
		/// <exception cref="MsieJavaScriptEngine.JsEngineLoadException">Failed to load a JavaScript engine.</exception>
		/// <exception cref="System.NotSupportedException">Selected mode of JavaScript engine is not supported.</exception>
		public MsieJsEngine(JsEngineSettings settings)
		{
			JsEngineMode engineMode = settings.EngineMode;
			JsEngineMode processedEngineMode = engineMode;

			if (engineMode == JsEngineMode.Auto)
			{
				if (ChakraEdgeJsRtJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraEdgeJsRt;
				}
				else if (ChakraIeJsRtJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraIeJsRt;
				}
				else if (ChakraActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraActiveScript;
				}
				else if (ClassicActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.Classic;
				}
				else
				{
					throw new JsEngineLoadException(Strings.Runtime_JsEnginesNotFound);
				}
			}

			lock (_creationSynchronizer)
			{
				JsEngineMode previousMode = _currentMode;

				switch (processedEngineMode)
				{
					case JsEngineMode.ChakraEdgeJsRt:
						if (previousMode != JsEngineMode.ChakraIeJsRt
							&& previousMode != JsEngineMode.ChakraActiveScript)
						{
							_jsEngine = new ChakraEdgeJsRtJsEngine(settings.EnableDebugging);
						}
						else if (previousMode == JsEngineMode.ChakraIeJsRt)
						{
							throw new JsEngineLoadException(
								string.Format(
									Strings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}
						else if (previousMode == JsEngineMode.ChakraActiveScript)
						{
							throw new JsEngineLoadException(
								string.Format(
									Strings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraIeJsRt:
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{
							_jsEngine = new ChakraIeJsRtJsEngine(settings.EnableDebugging);
						}
						else
						{
							throw new JsEngineLoadException(
								string.Format(
									Strings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraActiveScript:
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{
							_jsEngine = new ChakraActiveScriptJsEngine();
						}
						else
						{
							throw new JsEngineLoadException(
								string.Format(
									Strings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.Classic:
						_jsEngine = new ClassicActiveScriptJsEngine(settings.UseEcmaScript5Polyfill,
							settings.UseJson2Library);
						break;
					default:
						throw new NotSupportedException(
							string.Format(Strings.Runtime_JsEngineModeNotSupported, processedEngineMode));
				}

				_currentMode = processedEngineMode;
			}
		}


		private void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(ToString());
			}
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JavaScript expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public object Evaluate(string expression)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			return _jsEngine.Evaluate(expression);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JavaScript expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public T Evaluate<T>(string expression)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
				string.Format(Strings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			object result = _jsEngine.Evaluate(expression);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JavaScript code</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void Execute(string code)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "code"), "code");
			}

			_jsEngine.Execute(code);
		}

		/// <summary>
		/// Executes a code from JS-file
		/// </summary>
		/// <param name="path">Path to the JS-file</param>
		/// <param name="encoding">Text encoding</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.IO.FileNotFoundException">Specified JS-file not found.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void ExecuteFile(string path, Encoding encoding = null)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "path"), "path");
			}

			string code = Utils.GetFileTextContent(path, encoding);
			Execute(code);
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">JS-resource name</param>
		/// <param name="type">Type from assembly that containing an embedded resource</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.ArgumentNullException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void ExecuteResource(string resourceName, Type type)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			if (type == null)
			{
				throw new ArgumentNullException(
					"type", string.Format(Strings.Common_ArgumentIsNull, "type"));
			}

			string code = Utils.GetResourceAsString(resourceName, type);
			Execute(code);
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">JS-resource name</param>
		/// <param name="assembly">Assembly that containing an embedded resource</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.ArgumentNullException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void ExecuteResource(string resourceName, Assembly assembly)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					"assembly", string.Format(Strings.Common_ArgumentIsNull, "assembly"));
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			Execute(code);
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The function name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of one function
		/// parameter is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public object CallFunction(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidFunctionNameFormat, functionName));
			}

			int argumentCount = args.Length;
			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object argument = args[argumentIndex];

					if (argument != null)
					{
						Type argType = argument.GetType();

						if (!ValidationHelpers.IsSupportedType(argType))
						{
							throw new NotSupportedTypeException(
								string.Format(Strings.Runtime_FunctionParameterTypeNotSupported,
									functionName, argType.FullName));
						}
					}
				}
			}

			object result = _jsEngine.CallFunction(functionName, args);

			return result;
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <typeparam name="T">Type of function result</typeparam>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The function name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value or
		/// one function parameter is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public T CallFunction<T>(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
				string.Format(Strings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidFunctionNameFormat, functionName));
			}

			int argumentCount = args.Length;
			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object argument = args[argumentIndex];

					if (argument != null)
					{
						Type argType = argument.GetType();

						if (!ValidationHelpers.IsSupportedType(argType))
						{
							throw new NotSupportedTypeException(
								string.Format(Strings.Runtime_FunctionParameterTypeNotSupported,
									functionName, argType.FullName));
						}
					}
				}
			}

			object result = _jsEngine.CallFunction(functionName, args);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Сhecks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Result of check (true - exists; false - not exists</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public bool HasVariable(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			return _jsEngine.HasVariable(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public object GetVariableValue(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			return _jsEngine.GetVariableValue(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public T GetVariableValue<T>(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
					string.Format(Strings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			object result = _jsEngine.GetVariableValue(variableName);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Sets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="value">Value of variable</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of variable value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void SetVariableValue(string variableName, object value)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (value != null)
			{
				Type variableType = value.GetType();

				if (!ValidationHelpers.IsSupportedType(variableType))
				{
					throw new NotSupportedTypeException(
						string.Format(Strings.Runtime_VariableTypeNotSupported,
							variableName, variableType.FullName));
				}
			}

			_jsEngine.SetVariableValue(variableName, value);
		}

		/// <summary>
		/// Removes a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JavaScript engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JavaScript runtime error.</exception>
		public void RemoveVariable(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			_jsEngine.RemoveVariable(variableName);
		}

		/// <summary>
		/// Embeds a host object to script code
		/// </summary>
		/// <param name="itemName">The name for the new global variable or function that will represent the object</param>
		/// <param name="value">The object to expose</param>
		/// <remarks>Allows to embed instances of simple classes (or structures) and delegates.</remarks>
		public void EmbedHostObject(string itemName, object value)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(itemName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "itemName"), "itemName");
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidScriptItemNameFormat, itemName));
			}

			if (value != null)
			{
				Type itemType = value.GetType();

				if (ValidationHelpers.IsPrimitiveType(itemType)
					|| itemType == typeof (Undefined))
				{
					throw new NotSupportedTypeException(
						string.Format(Strings.Runtime_EmbeddedHostObjectTypeNotSupported, itemName, itemType.FullName));
				}
			}
			else
			{
				throw new ArgumentNullException("value", string.Format(Strings.Common_ArgumentIsNull, "value"));
			}

			_jsEngine.EmbedHostObject(itemName, value);
		}

		/// <summary>
		/// Embeds a host type to script code
		/// </summary>
		/// <param name="itemName">The name for the new global variable that will represent the type</param>
		/// <param name="type">The type to expose</param>
		/// <remarks>
		/// Host types are exposed to script code in the form of objects whose properties and
		/// methods are bound to the type's static members.
		/// </remarks>
		public void EmbedHostType(string itemName, Type type)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(itemName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "itemName"), "itemName");
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidScriptItemNameFormat, itemName));
			}

			if (type != null)
			{
				if (ValidationHelpers.IsPrimitiveType(type)
					|| type == typeof(Undefined))
				{
					throw new NotSupportedTypeException(
						string.Format(Strings.Runtime_EmbeddedHostTypeNotSupported, type.FullName));
				}
			}
			else
			{
				throw new ArgumentNullException("type", string.Format(Strings.Common_ArgumentIsNull, "type"));
			}

			_jsEngine.EmbedHostType(itemName, type);
		}

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				if (_jsEngine != null)
				{
					_jsEngine.Dispose();
					_jsEngine = null;
				}
			}
		}

		#endregion
	}
}
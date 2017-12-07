using System;
using System.Reflection;
using System.Text;

#if !NETSTANDARD
using MsieJavaScriptEngine.ActiveScript;
#endif
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.JsRt.Edge;
using MsieJavaScriptEngine.JsRt.Ie;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// .NET-wrapper for working with the Internet Explorer's JS engines
	/// </summary>
	public sealed class MsieJsEngine : IDisposable
	{
		/// <summary>
		/// JS engine
		/// </summary>
		private IInnerJsEngine _jsEngine;

		/// <summary>
		/// Current JS engine mode
		/// </summary>
		private static JsEngineMode _currentMode;

		/// <summary>
		/// Synchronizer of JS engines creation
		/// </summary>
		private static readonly object _creationSynchronizer = new object();

		/// <summary>
		/// Unique document name manager
		/// </summary>
		private readonly UniqueDocumentNameManager _documentNameManager =
			new UniqueDocumentNameManager("Script Document");

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private InterlockedStatedFlag _disposedFlag = new InterlockedStatedFlag();

		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		public string Mode
		{
			get { return _jsEngine.Mode; }
		}


		/// <summary>
		/// Constructs an instance of MSIE JS engine
		/// </summary>
		/// <exception cref="MsieJavaScriptEngine.JsEngineLoadException">Failed to load a JS engine.</exception>
		/// <exception cref="System.NotSupportedException">Selected mode of JS engine is not supported.</exception>
		public MsieJsEngine()
			: this(new JsEngineSettings())
		{ }

		/// <summary>
		/// Constructs an instance of MSIE JS engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		/// <exception cref="MsieJavaScriptEngine.JsEngineLoadException">Failed to load a JS engine.</exception>
		/// <exception cref="System.NotSupportedException">Selected mode of JS engine is not supported.</exception>
		public MsieJsEngine(JsEngineSettings settings)
		{
			JsEngineMode engineMode = settings.EngineMode;
			JsEngineMode processedEngineMode = engineMode;
			JsEngineSettings processedSettings = settings;

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
#if !NETSTANDARD
				else if (ChakraActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraActiveScript;
				}
				else if (ClassicActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.Classic;
				}
#endif
				else
				{
					throw new JsEngineLoadException(
#if NETSTANDARD
						NetCoreStrings.Runtime_JsEnginesNotFound
#else
						NetFrameworkStrings.Runtime_JsEnginesNotFound
#endif
					);
				}
			}

			if (processedEngineMode != engineMode)
			{
				processedSettings = settings.Clone();
				processedSettings.EngineMode = processedEngineMode;
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
							_jsEngine = new ChakraEdgeJsRtJsEngine(processedSettings);
						}
						else if (previousMode == JsEngineMode.ChakraIeJsRt)
						{
							throw new JsEngineLoadException(
								string.Format(
									CommonStrings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}
						else if (previousMode == JsEngineMode.ChakraActiveScript)
						{
							throw new JsEngineLoadException(
								string.Format(
									CommonStrings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraIeJsRt:
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{
							_jsEngine = new ChakraIeJsRtJsEngine(processedSettings);
						}
						else
						{
							throw new JsEngineLoadException(
								string.Format(
									CommonStrings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraActiveScript:
#if !NETSTANDARD
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{

							_jsEngine = new ChakraActiveScriptJsEngine(processedSettings);
						}
						else
						{
							throw new JsEngineLoadException(
								string.Format(
									CommonStrings.Runtime_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
#else
						throw new NotSupportedException(
								string.Format(NetCoreStrings.Runtime_JsEngineModeNotCompatibleWithNetCore, processedEngineMode));
#endif
					case JsEngineMode.Classic:
#if !NETSTANDARD
						_jsEngine = new ClassicActiveScriptJsEngine(processedSettings);

						break;
#else
						throw new NotSupportedException(
								string.Format(NetCoreStrings.Runtime_JsEngineModeNotCompatibleWithNetCore, processedEngineMode));
#endif
					default:
						throw new NotSupportedException(
							string.Format(CommonStrings.Runtime_JsEngineModeNotSupported, processedEngineMode));
				}

				_currentMode = processedEngineMode;
			}
		}


		private void VerifyNotDisposed()
		{
			if (_disposedFlag.IsSet())
			{
				throw new ObjectDisposedException(ToString());
			}
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public object Evaluate(string expression)
		{
			return Evaluate(expression, string.Empty);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS expression</param>
		/// <param name="documentName">Document name</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public object Evaluate(string expression, string documentName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);

			return _jsEngine.Evaluate(expression, uniqueDocumentName);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JS expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public T Evaluate<T>(string expression)
		{
			return Evaluate<T>(expression, string.Empty);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JS expression</param>
		/// <param name="documentName">Document name</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public T Evaluate<T>(string expression, string documentName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
				string.Format(CommonStrings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);
			object result = _jsEngine.Evaluate(expression, uniqueDocumentName);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void Execute(string code)
		{
			Execute(code, string.Empty);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <param name="documentName">Document name</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void Execute(string code, string documentName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "code"), "code");
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);
			_jsEngine.Execute(code, uniqueDocumentName);
		}

		/// <summary>
		/// Executes a code from JS-file
		/// </summary>
		/// <param name="path">Path to the JS-file</param>
		/// <param name="encoding">Text encoding</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.IO.FileNotFoundException">Specified JS-file not found.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void ExecuteFile(string path, Encoding encoding = null)
		{
			VerifyNotDisposed();

			if (path == null)
			{
				throw new ArgumentNullException(
					"path", string.Format(CommonStrings.Common_ArgumentIsNull, "path"));
			}

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "path"), "path");
			}

			string code = Utils.GetFileTextContent(path, encoding);
			Execute(code, path);
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name without the namespace of the specified type</param>
		/// <param name="type">The type, that determines the assembly and whose namespace is used to scope
		/// the resource name</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.ArgumentNullException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void ExecuteResource(string resourceName, Type type)
		{
			VerifyNotDisposed();

			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(CommonStrings.Common_ArgumentIsNull, "resourceName"));
			}

			if (type == null)
			{
				throw new ArgumentNullException(
					"type", string.Format(CommonStrings.Common_ArgumentIsNull, "type"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			Assembly assembly = type.GetTypeInfo().Assembly;
			string nameSpace = type.Namespace;
			string resourceFullName = nameSpace != null ? nameSpace + "." + resourceName : resourceName;

			string code = Utils.GetResourceAsString(resourceFullName, assembly);
			Execute(code, resourceName);
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.ArgumentNullException" />
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void ExecuteResource(string resourceName, Assembly assembly)
		{
			VerifyNotDisposed();

			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(CommonStrings.Common_ArgumentIsNull, "resourceName"));
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					"assembly", string.Format(CommonStrings.Common_ArgumentIsNull, "assembly"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			Execute(code, resourceName);
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The function name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of one function
		/// parameter is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public object CallFunction(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidFunctionNameFormat, functionName));
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
								string.Format(CommonStrings.Runtime_FunctionParameterTypeNotSupported,
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
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The function name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value or
		/// one function parameter is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public T CallFunction<T>(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
				string.Format(CommonStrings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidFunctionNameFormat, functionName));
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
								string.Format(CommonStrings.Runtime_FunctionParameterTypeNotSupported,
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
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public bool HasVariable(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidVariableNameFormat, variableName));
			}

			return _jsEngine.HasVariable(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		/// <exception cref="System.ObjectDisposedException">Operation is performed on a disposed MSIE
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public object GetVariableValue(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidVariableNameFormat, variableName));
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
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of return value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public T GetVariableValue<T>(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new NotSupportedTypeException(
					string.Format(CommonStrings.Runtime_ReturnValueTypeNotSupported, returnValueType.FullName));
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidVariableNameFormat, variableName));
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
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.NotSupportedTypeException">The type of variable value
		/// is not supported.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void SetVariableValue(string variableName, object value)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (value != null)
			{
				Type variableType = value.GetType();

				if (!ValidationHelpers.IsSupportedType(variableType))
				{
					throw new NotSupportedTypeException(
						string.Format(CommonStrings.Runtime_VariableTypeNotSupported,
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
		/// JS engine.</exception>
		/// <exception cref="System.ArgumentException" />
		/// <exception cref="System.FormatException">The variable name has incorrect format.</exception>
		/// <exception cref="MsieJavaScriptEngine.JsRuntimeException">JS runtime error.</exception>
		public void RemoveVariable(string variableName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidVariableNameFormat, variableName));
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
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "itemName"), "itemName");
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidScriptItemNameFormat, itemName));
			}

			if (value != null)
			{
				Type itemType = value.GetType();

				if (ValidationHelpers.IsPrimitiveType(itemType)
					|| itemType == typeof (Undefined))
				{
					throw new NotSupportedTypeException(
						string.Format(CommonStrings.Runtime_EmbeddedHostObjectTypeNotSupported, itemName, itemType.FullName));
				}
			}
			else
			{
				throw new ArgumentNullException("value", string.Format(CommonStrings.Common_ArgumentIsNull, "value"));
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
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "itemName"), "itemName");
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new FormatException(
					string.Format(CommonStrings.Runtime_InvalidScriptItemNameFormat, itemName));
			}

			if (type != null)
			{
				if (ValidationHelpers.IsPrimitiveType(type)
					|| type == typeof(Undefined))
				{
					throw new NotSupportedTypeException(
						string.Format(CommonStrings.Runtime_EmbeddedHostTypeNotSupported, type.FullName));
				}
			}
			else
			{
				throw new ArgumentNullException("type", string.Format(CommonStrings.Common_ArgumentIsNull, "type"));
			}

			_jsEngine.EmbedHostType(itemName, type);
		}

		/// <summary>
		/// Interrupts script execution and causes the JS engine to throw an exception
		/// </summary>
		public void Interrupt()
		{
			VerifyNotDisposed();

			_jsEngine.Interrupt();
		}

		/// <summary>
		/// Performs a full garbage collection
		/// </summary>
		public void CollectGarbage()
		{
			VerifyNotDisposed();

			_jsEngine.CollectGarbage();
		}

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			if (_disposedFlag.Set())
			{
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
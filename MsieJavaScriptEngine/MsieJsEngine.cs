namespace MsieJavaScriptEngine
{
	using System;
	using System.Reflection;
	using System.Text;

	using ActiveScript;
	using Helpers;
	using JsRt;
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
		public MsieJsEngine()
			: this(JsEngineMode.Auto)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="engineMode">JavaScript engine mode</param>
		public MsieJsEngine(JsEngineMode engineMode)
			: this(engineMode, false)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		public MsieJsEngine(JsEngineMode engineMode, bool useEcmaScript5Polyfill)
			: this(engineMode, useEcmaScript5Polyfill, false)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		public MsieJsEngine(bool useEcmaScript5Polyfill)
			: this(useEcmaScript5Polyfill, false)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public MsieJsEngine(bool useEcmaScript5Polyfill, bool useJson2Library)
			: this(JsEngineMode.Auto, useEcmaScript5Polyfill, useJson2Library)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JavaScript engine
		/// </summary>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public MsieJsEngine(JsEngineMode engineMode, bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			JsEngineMode processedEngineMode = engineMode;

			if (engineMode == JsEngineMode.Auto)
			{
				if (ChakraJsRtJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraJsRt;
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

			switch (processedEngineMode)
			{
				case JsEngineMode.ChakraJsRt:
					_jsEngine = new ChakraJsRtJsEngine();
					break;
				case JsEngineMode.ChakraActiveScript:
					_jsEngine = new ChakraActiveScriptJsEngine(useEcmaScript5Polyfill, useJson2Library);
					break;
				case JsEngineMode.Classic:
					_jsEngine = new ClassicActiveScriptJsEngine(useEcmaScript5Polyfill, useJson2Library);
					break;
				default:
					throw new NotSupportedException(
						string.Format(Strings.Runtime_JsEngineModeNotSupported, processedEngineMode));
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

			return JsTypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JavaScript code</param>
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

			object result = _jsEngine.CallFunction(functionName, args);

			return JsTypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Сhecks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Result of check (true - exists; false - not exists</returns>
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

			return JsTypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Sets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="value">Value of variable</param>
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
		/// Сhecks for the existence of a property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Name of property</param>
		/// <returns>Result of check (true - exists; false - not exists)</returns>
		[Obsolete("This method is obsolete.", false)]
		public bool HasProperty(string variableName, string propertyName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "propertyName"), "propertyName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (!ValidationHelpers.CheckPropertyNameFormat(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidPropertyNameFormat, propertyName));
			}

			return _jsEngine.HasProperty(variableName, propertyName);
		}

		/// <summary>
		/// Gets a value of property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Name of property</param>
		/// <returns>Value of property</returns>
		[Obsolete("This method is obsolete.", false)]
		public object GetPropertyValue(string variableName, string propertyName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "propertyName"), "propertyName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (!ValidationHelpers.CheckPropertyNameFormat(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidPropertyNameFormat, propertyName));
			}

			return _jsEngine.GetPropertyValue(variableName, propertyName);
		}

		/// <summary>
		/// Gets a value of property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Name of property</param>
		/// <returns>Value of property</returns>
		[Obsolete("This method is obsolete.", false)]
		public T GetPropertyValue<T>(string variableName, string propertyName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "propertyName"), "propertyName");
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

			if (!ValidationHelpers.CheckPropertyNameFormat(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidPropertyNameFormat, propertyName));
			}

			object result = _jsEngine.GetPropertyValue(variableName, propertyName);

			return JsTypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Sets a value to property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Name of property</param>
		/// <param name="value">Value of property</param>
		[Obsolete("This method is obsolete.", false)]
		public void SetPropertyValue(string variableName, string propertyName, object value)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "propertyName"), "propertyName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (!ValidationHelpers.CheckPropertyNameFormat(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidPropertyNameFormat, propertyName));
			}

			if (value != null)
			{
				Type propertyType = value.GetType();

				if (!ValidationHelpers.IsSupportedType(propertyType))
				{
					throw new NotSupportedTypeException(
						string.Format(Strings.Runtime_PropertyTypeNotSupported,
							variableName, propertyName, propertyType.FullName));
				}
			}

			_jsEngine.SetPropertyValue(variableName, propertyName, value);
		}

		/// <summary>
		/// Removes a property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Name of property</param>
		[Obsolete("This method is obsolete.", false)]
		public void RemoveProperty(string variableName, string propertyName)
		{
			VerifyNotDisposed();

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "propertyName"), "propertyName");
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			_jsEngine.RemoveProperty(variableName, propertyName);
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
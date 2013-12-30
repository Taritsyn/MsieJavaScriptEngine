namespace MsieJavaScriptEngine
{
	using System;
	using System.Reflection;
	using System.Text;
	using System.Web.Script.Serialization;

	using ActiveScript;
	using Helpers;
	using Resources;
	using Utilities;

	/// <summary>
	/// .NET-wrapper for working with the Internet Explorer's JS engines 
	/// (Chakra and classic JS engine)
	/// </summary>
	public sealed class MsieJsEngine : IDisposable
	{
		/// <summary>
		/// Name of resource, which contains a ECMAScript 5 Polyfill
		/// </summary>
		const string ES5_POLYFILL_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.ES5.min.js";

		/// <summary>
		/// Name of resource, which contains a JSON2 library
		/// </summary>
		const string JSON2_LIBRARY_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.json2.min.js";

		/// <summary>
		/// Name of resource, which contains a MsieJavaScript library
		/// </summary>
		const string MSIE_JAVASCRIPT_LIBRARY_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.msieJavaScriptEngine.min.js";

		/// <summary>
		/// Instance of site for the Windows Script engine
		/// </summary>
		private readonly ActiveScriptSite _activeScriptSite;

		/// <summary>
		/// Synchronizer of code execution
		/// </summary>
		private readonly object _executionSynchronizer = new object();

		/// <summary>
		/// JS-serializer
		/// </summary>
		private readonly JavaScriptSerializer _jsSerializer;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Constructs instance of MSIE JS-engine
		/// </summary>
		public MsieJsEngine() : this(false)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JS-engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		public MsieJsEngine(bool useEcmaScript5Polyfill) 
			: this(useEcmaScript5Polyfill, false)
		{ }

		/// <summary>
		/// Constructs instance of MSIE JS-engine
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public MsieJsEngine(bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			_activeScriptSite = new ActiveScriptSite();
			_jsSerializer = new JavaScriptSerializer();

			Type type = GetType();

			if (useEcmaScript5Polyfill)
			{
				ExecuteResource(ES5_POLYFILL_RESOURCE_NAME, type);
			}

			if (useJson2Library)
			{
				ExecuteResource(JSON2_LIBRARY_RESOURCE_NAME, type);
			}

			ExecuteResource(MSIE_JAVASCRIPT_LIBRARY_RESOURCE_NAME, type);
		}

		/// <summary>
		/// Destructs instance of MSIE JS engine
		/// </summary>
		~MsieJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Converts a value to JSON string
		/// </summary>
		/// <param name="value">The value to serialize</param>
		/// <returns>The serialized JSON string</returns>
		private string Serialize(object value)
		{
			if (value is Undefined)
			{
				return "undefined";
			}

			return _jsSerializer.Serialize(value);
		}

		/// <summary>
		/// Converts a given value to the specified type
		/// </summary>
		/// <typeparam name="T">The type to which value will be converted</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The value that has been converted to the target type</returns>
		public T ConvertToType<T>(object value)
		{
			return (T)ConvertToType(value, typeof(T));
		}

		/// <summary>
		/// Converts the specified value to the specified type
		/// </summary>
		/// <param name="value">The value to convert</param>
		/// <param name="targetType">The type to convert the value to</param>
		/// <returns>The value that has been converted to the target type</returns>
		private object ConvertToType(object value, Type targetType)
		{
			object result;
			
			if (ValidationHelpers.IsSupportedType(targetType))
			{
				result = _jsSerializer.ConvertToType(value, targetType);
			}
			else
			{
				throw new ArgumentException(
					string.Format(Strings.Runtime_TypeUnsupported, targetType), "value");
			}

			return result;
		}

		/// <summary>
		/// Executes a mapping from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToScriptType(object value)
		{
			if (value == null)
			{
				return DBNull.Value;
			}

			if (value is Undefined)
			{
				return null;
			}

			return value;
		}

		/// <summary>
		/// Executes a mapping from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToHostType(object value)
		{
			if (value == null)
			{
				return Undefined.Value;
			}

			if (value is DBNull)
			{
				return null;
			}

			return value;
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS-expression</param>
		/// <returns>Result of the expression</returns>
		public object Evaluate(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			return InnerEvaluate(expression);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JS-expression</param>
		/// <returns>Result of the expression</returns>
		public T Evaluate<T>(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "expression"), "expression");
			}

			object result = InnerEvaluate(expression);

			return ConvertToType<T>(result);
		}

		private object InnerEvaluate(string expression)
		{
			object result;

			lock (_executionSynchronizer)
			{
				result = _activeScriptSite.ExecuteScriptText(expression, true);
			}

			result = MapToHostType(result);

			return result;
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">Code</param>
		public void Execute(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "code"), "code");
			}

			lock (_executionSynchronizer)
			{
				_activeScriptSite.ExecuteScriptText(code, false);
			}
		}

		/// <summary>
		/// Executes a code from JS-file
		/// </summary>
		/// <param name="path">Path to the JS-file</param>
		/// <param name="encoding">Text encoding</param>
		public void ExecuteFile(string path, Encoding encoding = null)
		{
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
		/// Сhecks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Variable name</param>
		/// <returns>Result of check (true - exists; false - not exists</returns>
		public bool HasVariable(string variableName)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			ValidateVariableName(variableName);

			return InnerHasVariable(variableName);
		}

		private bool InnerHasVariable(string variableName)
		{
			string expression = string.Format("(typeof {0} !== 'undefined');", variableName);
			var result = Evaluate<bool>(expression);

			return result;
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Variable name</param>
		/// <returns>Value of variable</returns>
		public object GetVariableValue(string variableName)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			ValidateVariableName(variableName);

			return InnerGetVariableValue(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="variableName">Variable name</param>
		/// <returns>Value of variable</returns>
		public T GetVariableValue<T>(string variableName)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			ValidateVariableName(variableName);

			object result = InnerGetVariableValue(variableName);

			return ConvertToType<T>(result);
		}

		private object InnerGetVariableValue(string variableName)
		{
			string expression = string.Format("{0};", variableName);
			object result = Evaluate(expression);

			return result;
		}

		/// <summary>
		/// Sets a value of variable
		/// </summary>
		/// <param name="variableName">Variable name</param>
		/// <param name="value">Value of variable</param>
		public void SetVariableValue(string variableName, object value)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			ValidateVariableName(variableName);

			string serializeValue = Serialize(value);
			string code = string.Format(@"if (typeof {0} !== 'undefined') {{
	{0} = {1};
}}
else {{
	var {0} = {1};
}}", variableName, serializeValue);

			Execute(code);
		}

		/// <summary>
		/// Removes a variable
		/// </summary>
		/// <param name="variableName">Variable name</param>
		public void RemoveVariable(string variableName)
		{
			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "variableName"), "variableName");
			}

			ValidateVariableName(variableName);

			string code = string.Format(@"if (typeof {0} !== 'undefined') {{
	{0} = undefined;
}}", variableName);

			Execute(code);
		}

		/// <summary>
		/// Starts a validation of variable name
		/// </summary>
		/// <param name="variableName">Variable name</param>
		private void ValidateVariableName(string variableName)
		{
			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidVariableNameFormat, variableName));
			}

			if (!ValidationHelpers.CheckNameAllowability(variableName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_VariableNameIsForbidden, variableName));
			}
		}

		/// <summary>
		/// Сhecks for the existence of a property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Property name</param>
		/// <returns>Result of check (true - exists; false - not exists)</returns>
		public bool HasProperty(string variableName, string propertyName)
		{
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

			ValidateVariableName(variableName);
			ValidatePropertyName(propertyName);

			if (!InnerHasVariable(variableName))
			{
				throw new UndefinedValueException(
					string.Format(Strings.Runtime_VariableNotExist, variableName));
			}

			string expression = string.Format("(typeof {0}.{1} !== 'undefined');", variableName, propertyName);
			var result = Evaluate<bool>(expression);

			return result;
		}

		/// <summary>
		/// Gets a value of property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Property name</param>
		/// <returns>Value of property</returns>
		public object GetPropertyValue(string variableName, string propertyName)
		{
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

			ValidateVariableName(variableName);
			ValidatePropertyName(propertyName);

			if (!InnerHasVariable(variableName))
			{
				throw new UndefinedValueException(
					string.Format(Strings.Runtime_VariableNotExist, variableName));
			}

			return InnerGetPropertyValue(variableName, propertyName);
		}

		/// <summary>
		/// Gets a value of property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Property name</param>
		/// <returns>Value of property</returns>
		public T GetPropertyValue<T>(string variableName, string propertyName)
		{
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

			ValidateVariableName(variableName);
			ValidatePropertyName(propertyName);

			if (!InnerHasVariable(variableName))
			{
				throw new UndefinedValueException(
					string.Format(Strings.Runtime_VariableNotExist, variableName));
			}

			object result = InnerGetPropertyValue(variableName, propertyName);

			return ConvertToType<T>(result);
		}

		private object InnerGetPropertyValue(string variableName, string propertyName)
		{
			string expression = string.Format("{0}.{1};", variableName, propertyName);
			object result = Evaluate(expression);

			return result;
		}

		/// <summary>
		/// Sets a value of property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="value">Value of property</param>
		public void SetPropertyValue(string variableName, string propertyName, object value)
		{
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

			ValidateVariableName(variableName);
			ValidatePropertyName(propertyName);

			string serializeValue = Serialize(value);
			string code = string.Format(@"if (typeof {0} === 'undefined') {{
	var {0} = {{}};	
}}

msieJavaScript.setPropertyValue({0}, ""{1}"", {2})", variableName, propertyName, serializeValue);

			Execute(code);
		}

		/// <summary>
		/// Removes a property
		/// </summary>
		/// <param name="variableName">Name of variable that contains the object</param>
		/// <param name="propertyName">Property name</param>
		public void RemoveProperty(string variableName, string propertyName)
		{
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

			ValidateVariableName(variableName);
			ValidatePropertyName(propertyName);

			string code = string.Format(@"if (typeof {0}.{1} !== 'undefined') {{
	delete {0}.{1};
}}", variableName, propertyName);

			Execute(code);
		}

		/// <summary>
		/// Starts a validation of property name
		/// </summary>
		/// <param name="propertyName">Property name</param>
		private void ValidatePropertyName(string propertyName)
		{
			if (!ValidationHelpers.CheckPropertyNameFormat(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidPropertyNameFormat, propertyName));
			}

			if (!ValidationHelpers.CheckPropertyNameAllowability(propertyName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_PropertyNameIsForbidden, propertyName));
			}
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		public object CallFunction(string functionName, params object[] args)
		{
			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			ValidateFunctionName(functionName);

			object result = InnerCallFunction(functionName, args);

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
			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "functionName"), "functionName");
			}

			ValidateFunctionName(functionName);

			object result = InnerCallFunction(functionName, args);

			return ConvertToType<T>(result);
		}

		private object InnerCallFunction(string functionName, params object[] args)
		{
			object result;

			int argumentCount = args.Length;
			var processedArgs = new object[argumentCount];

			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					processedArgs[argumentIndex] = MapToScriptType(args[argumentIndex]);
				}
			}

			lock (_executionSynchronizer)
			{
				result = _activeScriptSite.CallFunction(functionName, processedArgs);
			}

			result = MapToHostType(result);

			return result;
		}

		/// <summary>
		/// Starts a validation of function name
		/// </summary>
		/// <param name="functionName">Function name</param>
		private void ValidateFunctionName(string functionName)
		{
			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_InvalidFunctionNameFormat, functionName));
			}

			if (!ValidationHelpers.CheckNameAllowability(functionName))
			{
				throw new FormatException(
					string.Format(Strings.Runtime_FunctionNameIsForbidden, functionName));
			}
		}
		
		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		public void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (_activeScriptSite != null)
				{
					_activeScriptSite.Dispose();
				}
			}
		}
	}
}
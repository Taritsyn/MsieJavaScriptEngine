namespace MsieJavaScriptEngine.ActiveScript
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Windows.Threading;

	using Helpers;
	using Resources;
	using Utilities;

	/// <summary>
	/// Base class of the ActiveScript JavaScript engine
	/// </summary>
	internal abstract class ActiveScriptJsEngineBase : IInnerJsEngine
	{
		/// <summary>
		/// Name of resource, which contains a ECMAScript 5 Polyfill
		/// </summary>
		private const string ES5_POLYFILL_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.ES5.min.js";

		/// <summary>
		/// Name of resource, which contains a JSON2 library
		/// </summary>
		private const string JSON2_LIBRARY_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.json2.min.js";

		/// <summary>
		/// Pointer to an instance of native JavaScript engine
		/// </summary>
		private IntPtr _pActiveScript = IntPtr.Zero;

		/// <summary>
		/// Instance of native JavaScript engine
		/// </summary>
		private IActiveScript _activeScript;

		/// <summary>
		/// Instance of site for the ActiveScript engine
		/// </summary>
		private ActiveScriptSiteWrapper _activeScriptSite;

		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		private readonly string _engineModeName;

		/// <summary>
		/// Lowest supported version of Internet Explorer
		/// </summary>
		private readonly string _lowerIeVersion;

		/// <summary>
		/// <see cref="System.Windows.Threading.Dispatcher"/> for the thread currently executing
		/// </summary>
		private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Constructs instance of the ActiveScript JavaScript engine
		/// </summary>
		/// <param name="clsid">CLSID of JavaScript engine</param>
		/// <param name="engineModeName">Name of JavaScript engine mode</param>
		/// <param name="lowerIeVersion">Lowest supported version of Internet Explorer</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		protected ActiveScriptJsEngineBase(string clsid, string engineModeName, string lowerIeVersion,
			bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			_engineModeName = engineModeName;
			_lowerIeVersion = lowerIeVersion;

			try
			{
				_pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
				_activeScript = (IActiveScript)Marshal.GetObjectForIUnknown(_pActiveScript);
			}
			catch (Exception e)
			{
				throw new JsEngineLoadException(
					string.Format(Strings.Runtime_JsEngineNotLoaded,
						_engineModeName, _lowerIeVersion, e.Message), _engineModeName);
			}

			_activeScriptSite = new ActiveScriptSiteWrapper(_pActiveScript, _activeScript);

			LoadResources(useEcmaScript5Polyfill, useJson2Library);
		}

		/// <summary>
		/// Destructs instance of ActiveScript JavaScript engine
		/// </summary>
		~ActiveScriptJsEngineBase()
		{
			Dispose(false);
		}


		/// <summary>
		/// Checks a support of the JavaScript engine on the machine
		/// </summary>
		/// <param name="clsid">CLSID of JavaScript engine</param>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		protected static bool IsSupported(string clsid)
		{
			bool isSupported;
			IntPtr pActiveScript = IntPtr.Zero;

			try
			{
				pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
				isSupported = true;
			}
			catch
			{
				isSupported = false;
			}
			finally
			{
				ComHelpers.ReleaseAndEmpty(ref pActiveScript);
			}

			return isSupported;
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

		private JsRuntimeException ConvertActiveScriptExceptionToJsRuntimeException(
			ActiveScriptException activeScriptException)
		{
			var jsEngineException = new JsRuntimeException(activeScriptException.Message, _engineModeName)
			{
				ErrorCode = activeScriptException.ErrorCode.ToString(CultureInfo.InvariantCulture),
				Category = activeScriptException.Subcategory,
				LineNumber = (int)activeScriptException.LineNumber,
				ColumnNumber = activeScriptException.ColumnNumber,
				SourceFragment = activeScriptException.SourceError,
				Source = activeScriptException.Source,
				HelpLink = activeScriptException.HelpLink
			};

			return jsEngineException;
		}

		private void InvokeScript(Action action)
		{
			try
			{
				_dispatcher.Invoke(DispatcherPriority.Input, action);
			}
			catch (ActiveScriptException e)
			{
				throw ConvertActiveScriptExceptionToJsRuntimeException(e);
			}
		}

		private T InvokeScript<T>(Func<T> func)
		{
			try
			{
				return (T)_dispatcher.Invoke(DispatcherPriority.Input, func);
			}
			catch (ActiveScriptException e)
			{
				throw ConvertActiveScriptExceptionToJsRuntimeException(e);
			}
		}

		/// <summary>
		/// Loads a resources
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		private void LoadResources(bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			Type type = GetType();

			if (useEcmaScript5Polyfill)
			{
				ExecuteResource(ES5_POLYFILL_RESOURCE_NAME, type);
			}

			if (useJson2Library)
			{
				ExecuteResource(JSON2_LIBRARY_RESOURCE_NAME, type);
			}
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">JS-resource name</param>
		/// <param name="type">Type from assembly that containing an embedded resource</param>
		private void ExecuteResource(string resourceName, Type type)
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
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			_dispatcher.Invoke(DispatcherPriority.Input, (Action)InnerDispose);
		}

		private void InnerDispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				if (_activeScriptSite != null)
				{
					_activeScriptSite.Dispose();
					_activeScriptSite = null;
				}

				if (_activeScript != null)
				{
					_activeScript.Close();
					_activeScript = null;
				}

				ComHelpers.ReleaseAndEmpty(ref _pActiveScript);
			}
		}

		#region IInnerJsEngine implementation

		public string Mode
		{
			get { return _engineModeName; }
		}

		public object Evaluate(string expression)
		{
			object result = InvokeScript(() => _activeScriptSite.ExecuteScriptText(expression, true));
			result = MapToHostType(result);

			return result;
		}

		public void Execute(string code)
		{
			InvokeScript(() =>
			{
				_activeScriptSite.ExecuteScriptText(code, false);
			});
		}

		public object CallFunction(string functionName, params object[] args)
		{
			int argumentCount = args.Length;
			var processedArgs = new object[argumentCount];

			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					processedArgs[argumentIndex] = MapToScriptType(args[argumentIndex]);
				}
			}

			object result = InvokeScript(() =>
			{
				try
				{
					return _activeScriptSite.CallFunction(functionName, processedArgs);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_FunctionNotExist, functionName));
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public bool HasVariable(string variableName)
		{
			var variableExist = InvokeScript(() => _activeScriptSite.HasProperty(variableName));

			return variableExist;
		}

		public object GetVariableValue(string variableName)
		{
			object variableValue = InvokeScript(() =>
			{
				try
				{
					return _activeScriptSite.GetProperty(variableName);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(Strings.Runtime_VariableNotExist, variableName));
				}
			});

			object result = MapToHostType(variableValue);

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			object processedValue = MapToScriptType(value);

			InvokeScript(() => _activeScriptSite.SetProperty(variableName, processedValue));
		}

		public void RemoveVariable(string variableName)
		{
			InvokeScript(() => _activeScriptSite.DeleteProperty(variableName));
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
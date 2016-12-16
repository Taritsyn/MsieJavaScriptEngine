#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Base class of the ActiveScript JavaScript engine
	/// </summary>
	internal abstract class ActiveScriptJsEngineBase : IInnerJsEngine, IActiveScriptSite
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
		private IntPtr _pActiveScript;

		/// <summary>
		/// Pointer to an instance of garbage collector
		/// </summary>
		private IntPtr _pActiveScriptGarbageCollector;

		/// <summary>
		/// Instance of native JavaScript engine
		/// </summary>
		private IActiveScript _activeScript;

		/// <summary>
		/// Instance of <see cref="IActiveScriptParseWrapper"/>
		/// </summary>
		private IActiveScriptParseWrapper _activeScriptParse;

		/// <summary>
		/// Instance of <see cref="IActiveScriptGarbageCollector"/>
		/// </summary>
		private IActiveScriptGarbageCollector _activeScriptGarbageCollector;

		/// <summary>
		/// Instance of script dispatch
		/// </summary>
		private IExpando _dispatch;

		/// <summary>
		/// List of host items
		/// </summary>
		private readonly Dictionary<string, object> _hostItems = new Dictionary<string, object>();

		/// <summary>
		/// Host-defined document version string
		/// </summary>
		private readonly string _documentVersion;

		/// <summary>
		/// Last ActiveScript exception
		/// </summary>
		private ActiveScriptException _lastException;

		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		private readonly JsEngineMode _engineMode;

		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		private readonly string _engineModeName;

		/// <summary>
		/// Script dispatcher
		/// </summary>
		private static readonly ScriptDispatcher _dispatcher = new ScriptDispatcher();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private StatedFlag _disposedFlag = new StatedFlag();


		/// <summary>
		/// Constructs an instance of the ActiveScript JavaScript engine
		/// </summary>
		/// <param name="clsid">CLSID of JavaScript engine</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="lowerIeVersion">Lowest supported version of Internet Explorer</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		protected ActiveScriptJsEngineBase(string clsid, JsEngineMode engineMode, string lowerIeVersion,
			ScriptLanguageVersion languageVersion, bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			_engineMode = engineMode;
			_engineModeName = JsEngineModeHelpers.GetModeName(engineMode);
			_documentVersion = DateTime.UtcNow.ToString("o");

			_dispatcher.Invoke(() =>
			{
				_pActiveScript = IntPtr.Zero;

				try
				{
					_pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
					_activeScript = (IActiveScript)Marshal.GetObjectForIUnknown(_pActiveScript);
				}
				catch (Exception e)
				{
					throw new JsEngineLoadException(
						string.Format(CommonStrings.Runtime_IeJsEngineNotLoaded,
							_engineModeName, lowerIeVersion, e.Message), _engineModeName);
				}

				if (languageVersion != ScriptLanguageVersion.None)
				{
					var activeScriptProperty = _activeScript as IActiveScriptProperty;
					if (activeScriptProperty != null)
					{
						object scriptLanguageVersion = (int)languageVersion;
						uint result = activeScriptProperty.SetProperty((uint)ScriptProperty.InvokeVersioning,
							IntPtr.Zero, ref scriptLanguageVersion);
						if (result != (uint)ScriptHResult.Ok)
						{
							throw new JsEngineLoadException(
								string.Format(NetFrameworkStrings.Runtime_ActiveScriptLanguageVersionSelectionFailed, languageVersion));
						}
					}
				}

				_activeScriptParse = new ActiveScriptParseWrapper(_pActiveScript, _activeScript);
				_activeScriptParse.InitNew();

				_pActiveScriptGarbageCollector = ComHelpers.QueryInterfaceNoThrow<IActiveScriptGarbageCollector>(_pActiveScript);
				_activeScriptGarbageCollector = _activeScript as IActiveScriptGarbageCollector;

				_activeScript.SetScriptSite(this);
				_activeScript.SetScriptState(ScriptState.Started);

				InitScriptDispatch();
			});

			LoadResources(useEcmaScript5Polyfill, useJson2Library);
		}

		/// <summary>
		/// Destructs an instance of ActiveScript JavaScript engine
		/// </summary>
		~ActiveScriptJsEngineBase()
		{
			Dispose(false);
		}


		/// <summary>
		/// Checks a support of the JavaScript engine on the machine
		/// </summary>
		/// <param name="clsid">CLSID of JavaScript engine</param>
		/// <param name="isSupported">Flag indicating whether this JavaScript engine is supported</param>
		/// <param name="supportSynchronizer">Support synchronizer</param>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		protected static bool IsSupported(string clsid, ref bool? isSupported, ref object supportSynchronizer)
		{
			if (isSupported.HasValue)
			{
				return isSupported.Value;
			}

			lock (supportSynchronizer)
			{
				if (isSupported.HasValue)
				{
					return isSupported.Value;
				}

				IntPtr pActiveScript = IntPtr.Zero;

				try
				{
					pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
					isSupported = true;
				}
				catch (COMException e)
				{
					if (e.ErrorCode == ComErrorCode.ClassNotRegistered)
					{
						isSupported = false;
					}
					else
					{
						isSupported = null;
					}
				}
				catch
				{
					isSupported = null;
				}
				finally
				{
					ComHelpers.ReleaseAndEmpty(ref pActiveScript);
				}

				return isSupported.HasValue && isSupported.Value;
			}
		}

		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private object MapToScriptType(object value)
		{
			return TypeMappingHelpers.MapToScriptType(value, _engineMode);
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToScriptType(object[] args)
		{
			return TypeMappingHelpers.MapToScriptType(args, _engineMode);
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private object MapToHostType(object value)
		{
			return TypeMappingHelpers.MapToHostType(value);
		}

		/// <summary>
		/// Makes a mapping of array items from the script type to a host type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToHostType(object[] args)
		{
			return TypeMappingHelpers.MapToHostType(args);
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
				HelpLink = activeScriptException.HelpLink
			};

			return jsEngineException;
		}

		/// <summary>
		/// Initializes a script dispatch
		/// </summary>
		private void InitScriptDispatch()
		{
			IExpando dispatch = null;
			object obj;

			_activeScript.GetScriptDispatch(null, out obj);

			if (obj != null && obj.GetType().IsCOMObject)
			{
				dispatch = obj as IExpando;
			}

			if (dispatch == null)
			{
				throw new InvalidOperationException(NetFrameworkStrings.Runtime_ActiveScriptDispatcherNotInitialized);
			}

			_dispatch = dispatch;
		}

		/// <summary>
		/// Gets and resets a last exception. Returns null for none.
		/// </summary>
		private ActiveScriptException GetAndResetLastException()
		{
			ActiveScriptException temp = _lastException;
			_lastException = null;

			return temp;
		}

		private void ThrowError()
		{
			ActiveScriptException last = GetAndResetLastException();
			if (last != null)
			{
				throw last;
			}
		}

		private void InvokeScript(Action action)
		{
			_dispatcher.Invoke(() =>
			{
				try
				{
					action();
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
				catch (TargetInvocationException e)
				{
					var activeScriptException = e.InnerException as ActiveScriptException;
					if (activeScriptException != null)
					{
						throw ConvertActiveScriptExceptionToJsRuntimeException(activeScriptException);
					}

					throw;
				}
			});
		}

		private T InvokeScript<T>(Func<T> func)
		{
			return _dispatcher.Invoke(() =>
			{
				try
				{
					return func();
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
				catch (TargetInvocationException e)
				{
					var activeScriptException = e.InnerException as ActiveScriptException;
					if (activeScriptException != null)
					{
						throw ConvertActiveScriptExceptionToJsRuntimeException(activeScriptException);
					}

					throw;
				}
			});
		}

		/// <summary>
		/// Executes a script text
		/// </summary>
		/// <param name="code">Script text</param>
		/// <param name="isExpression">Flag that script text needs to run as an expression</param>
		/// <returns>Result of the execution</returns>
		private object InnerExecute(string code, bool isExpression)
		{
			object result;

			try
			{
				result = _activeScriptParse.ParseScriptText(code, null, null, null, IntPtr.Zero,
					0, isExpression ? ScriptTextFlags.IsExpression : ScriptTextFlags.IsVisible);
			}
			catch
			{
				ThrowError();
				throw;
			}

			// Check for parse error
			ThrowError();

			return result;
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		private object InnerCallFunction(string functionName, params object[] args)
		{
			object result;

			try
			{
				result = _dispatch.InvokeMember(functionName, BindingFlags.InvokeMethod,
					null, _dispatch, args, null, CultureInfo.InvariantCulture, null);
			}
			catch
			{
				ThrowError();
				throw;
			}

			return result;
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		private object InnerGetVariableValue(string variableName)
		{
			object variableValue;

			try
			{
				variableValue = _dispatch.InvokeMember(variableName, BindingFlags.GetProperty,
					null, _dispatch, new object[0], null,
					CultureInfo.InvariantCulture, null);
			}
			catch
			{
				ThrowError();
				throw;
			}

			return variableValue;
		}

		/// <summary>
		/// Sets a value to variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="value">Value of variable</param>
		private void InnerSetVariableValue(string variableName, object value)
		{
			object[] args = { value };

			try
			{
				_dispatch.InvokeMember(variableName, BindingFlags.SetProperty, null, _dispatch,
					args, null, CultureInfo.InvariantCulture, null);
			}
			catch (MissingMemberException)
			{
				_dispatch.AddProperty(variableName);
				_dispatch.InvokeMember(variableName, BindingFlags.SetProperty, null, _dispatch,
					args, null, CultureInfo.InvariantCulture, null);
			}
			catch
			{
				ThrowError();
				throw;
			}
		}

		private void EmbedHostItem(string itemName, object value)
		{
			InvokeScript(() =>
			{
				object oldValue = null;
				if (_hostItems.ContainsKey(itemName))
				{
					oldValue = _hostItems[itemName];
				}
				_hostItems[itemName] = value;

				try
				{
					_activeScript.AddNamedItem(itemName, ScriptItemFlags.IsVisible | ScriptItemFlags.GlobalMembers);
				}
				catch (Exception)
				{
					if (oldValue != null)
					{
						_hostItems[itemName] = oldValue;
					}
					else
					{
						_hostItems.Remove(itemName);
					}

					throw;
				}
			});
		}

		/// <summary>
		/// Starts a garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		private void InnerCollectGarbage(ScriptGCType type)
		{
			if (_activeScriptGarbageCollector != null)
			{
				_activeScriptGarbageCollector.CollectGarbage(type);
			}
		}

		/// <summary>
		/// Loads a resources
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		private void LoadResources(bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			Assembly assembly = GetType().GetTypeInfo().Assembly;

			if (useEcmaScript5Polyfill)
			{
				ExecuteResource(ES5_POLYFILL_RESOURCE_NAME, assembly);
			}

			if (useJson2Library)
			{
				ExecuteResource(JSON2_LIBRARY_RESOURCE_NAME, assembly);
			}
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		private void ExecuteResource(string resourceName, Assembly assembly)
		{
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
			Execute(code);
		}

		#region IActiveScriptSite implementation

		/// <summary>
		/// Retrieves the locale identifier associated with the host's user interface. The scripting
		/// engine uses the identifier to ensure that error strings and other user-interface elements
		/// generated by the engine appear in the appropriate language.
		/// </summary>
		/// <param name="lcid">A variable that receives the locale identifier for user-interface
		/// elements displayed by the scripting engine</param>
		void IActiveScriptSite.GetLcid(out int lcid)
		{
			lcid = CultureInfo.CurrentCulture.LCID;
		}

		/// <summary>
		/// Allows the scripting engine to obtain information about an item added with the
		/// IActiveScript.AddNamedItem method
		/// </summary>
		/// <param name="name">The name associated with the item, as specified in the
		/// IActiveScript.AddNamedItem method</param>
		/// <param name="mask">A bit mask specifying what information about the item should be
		/// returned. The scripting engine should request the minimum amount of information possible
		/// because some of the return parameters (for example, ITypeInfo) can take considerable
		/// time to load or generate</param>
		/// <param name="pUnkItem">A variable that receives a pointer to the IUnknown interface associated
		/// with the given item. The scripting engine can use the IUnknown.QueryInterface method to
		/// obtain the IDispatch interface for the item. This parameter receives null if mask
		/// does not include the ScriptInfo.IUnknown value. Also, it receives null if there is no
		/// object associated with the item name; this mechanism is used to create a simple class when
		/// the named item was added with the ScriptItem.CodeOnly flag set in the
		/// IActiveScript.AddNamedItem method.</param>
		/// <param name="pTypeInfo">A variable that receives a pointer to the ITypeInfo interface
		/// associated with the item. This parameter receives null if mask does not include the
		/// ScriptInfo.ITypeInfo value, or if type information is not available for this item. If type
		/// information is not available, the object cannot source events, and name binding must be
		/// realized with the IDispatch.GetIDsOfNames method. Note that the ITypeInfo interface
		/// retrieved describes the item's coclass (TKIND_COCLASS) because the object may support
		/// multiple interfaces and event interfaces. If the item supports the IProvideMultipleTypeInfo
		/// interface, the ITypeInfo interface retrieved is the same as the index zero ITypeInfo that
		/// would be obtained using the IProvideMultipleTypeInfo.GetInfoOfIndex method.</param>
		void IActiveScriptSite.GetItemInfo(string name, ScriptInfoFlags mask, ref IntPtr pUnkItem, ref IntPtr pTypeInfo)
		{
			object item = _hostItems[name];
			if (item == null)
			{
				throw new COMException(
					string.Format(NetFrameworkStrings.Runtime_ItemNotFound, name), ComErrorCode.ElementNotFound);
			}

			if (mask.HasFlag(ScriptInfoFlags.IUnknown))
			{
				pUnkItem = Marshal.GetIDispatchForObject(item);
			}

			if (mask.HasFlag(ScriptInfoFlags.ITypeInfo))
			{
				pTypeInfo = Marshal.GetITypeInfoForType(item.GetType());
			}
		}

		/// <summary>
		/// Retrieves a host-defined string that uniquely identifies the current document version. If
		/// the related document has changed outside the scope of Windows Script (as in the case of an
		/// HTML page being edited with Notepad), the scripting engine can save this along with its
		/// persisted state, forcing a recompile the next time the script is loaded.
		/// </summary>
		/// <param name="version">The host-defined document version string</param>
		void IActiveScriptSite.GetDocVersionString(out string version)
		{
			version = _documentVersion;
		}

		/// <summary>
		/// Informs the host that the script has completed execution
		/// </summary>
		/// <param name="result">A variable that contains the script result, or null if the script
		/// produced no result</param>
		/// <param name="exceptionInfo">Contains exception information generated when the script
		/// terminated, or null if no exception was generated</param>
		void IActiveScriptSite.OnScriptTerminate(object result, EXCEPINFO exceptionInfo)
		{ }

		/// <summary>
		/// Informs the host that the scripting engine has changed states
		/// </summary>
		/// <param name="scriptState">Indicates the new script state</param>
		void IActiveScriptSite.OnStateChange(ScriptState scriptState)
		{ }

		/// <summary>
		/// Informs the host that an execution error occurred while the engine was running the script.
		/// </summary>
		/// <param name="scriptError">A host can use this interface to obtain information about the
		/// execution error</param>
		void IActiveScriptSite.OnScriptError(IActiveScriptError scriptError)
		{
			_lastException = ActiveScriptException.Create(scriptError);
		}

		/// <summary>
		/// Informs the host that the scripting engine has begun executing the script code
		/// </summary>
		void IActiveScriptSite.OnEnterScript()
		{ }

		/// <summary>
		/// Informs the host that the scripting engine has returned from executing script code
		/// </summary>
		void IActiveScriptSite.OnLeaveScript()
		{ }

		#endregion

		#region IInnerJsEngine implementation

		public string Mode
		{
			get { return _engineModeName; }
		}

		public object Evaluate(string expression)
		{
			object result = InvokeScript(() => InnerExecute(expression, true));
			result = MapToHostType(result);

			return result;
		}

		public void Execute(string code)
		{
			InvokeScript(() =>
			{
				InnerExecute(code, false);
			});
		}

		public object CallFunction(string functionName, params object[] args)
		{
			object[] processedArgs = MapToScriptType(args);

			object result = InvokeScript(() =>
			{
				try
				{
					return InnerCallFunction(functionName, processedArgs);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(CommonStrings.Runtime_FunctionNotExist, functionName));
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public bool HasVariable(string variableName)
		{
			bool result = InvokeScript(() =>
			{
				bool variableExist;

				try
				{
					object variableValue = InnerGetVariableValue(variableName);
					variableExist = variableValue != null;
				}
				catch (MissingMemberException)
				{
					variableExist = false;
				}

				return variableExist;
			});

			return result;
		}

		public object GetVariableValue(string variableName)
		{
			object result = InvokeScript(() =>
			{
				try
				{
					return InnerGetVariableValue(variableName);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(NetFrameworkStrings.Runtime_VariableNotExist, variableName));
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public void SetVariableValue(string variableName, object value)
		{
			object processedValue = MapToScriptType(value);
			InvokeScript(() => InnerSetVariableValue(variableName, processedValue));
		}

		public void RemoveVariable(string variableName)
		{
			InvokeScript(() =>
			{
				InnerSetVariableValue(variableName, null);

				if (_hostItems.ContainsKey(variableName))
				{
					_hostItems.Remove(variableName);
				}
			});
		}

		public void EmbedHostObject(string itemName, object value)
		{
			object processedValue = MapToScriptType(value);
			EmbedHostItem(itemName, processedValue);
		}

		public void EmbedHostType(string itemName, Type type)
		{
			var typeValue = new HostType(type, _engineMode);
			EmbedHostItem(itemName, typeValue);
		}

		public void CollectGarbage()
		{
			_dispatcher.Invoke(() => InnerCollectGarbage(ScriptGCType.Exhaustive));
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

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (_disposedFlag.Set())
			{
				_dispatcher.Invoke(() =>
				{
					if (_dispatch != null)
					{
						ComHelpers.ReleaseComObject(ref _dispatch, !disposing);
						_dispatch = null;
					}

					_activeScriptGarbageCollector = null;
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptGarbageCollector);

					if (_activeScriptParse != null)
					{
						_activeScriptParse.Dispose();
						_activeScriptParse = null;
					}

					if (_activeScript != null)
					{
						_activeScript.Close();
						Marshal.FinalReleaseComObject(_activeScript);
						_activeScript = null;
					}

					ComHelpers.ReleaseAndEmpty(ref _pActiveScript);

					if (disposing)
					{
						if (_hostItems != null)
						{
							_hostItems.Clear();
						}

						_lastException = null;
					}
				});
			}
		}

		#endregion
	}
}
#endif
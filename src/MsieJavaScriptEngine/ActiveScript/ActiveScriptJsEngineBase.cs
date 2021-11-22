#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of JS engine
	/// </summary>
	internal abstract partial class ActiveScriptJsEngineBase : InnerJsEngineBase
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
		/// Instance of Active Script wrapper
		/// </summary>
		protected IActiveScriptWrapper _activeScriptWrapper;

		/// <summary>
		/// Instance of script dispatch
		/// </summary>
		private IExpando _dispatch;

		/// <summary>
		/// List of host items
		/// </summary>
		protected Dictionary<string, object> _hostItems = new Dictionary<string, object>();

		/// <summary>
		/// Last Active Script exception
		/// </summary>
		private ActiveScriptException _lastException;

		/// <summary>
		/// Instance of process debug manager wrapper
		/// </summary>
		private ProcessDebugManagerWrapper _processDebugManagerWrapper;

		/// <summary>
		/// Instance of debug application wrapper
		/// </summary>
		private DebugApplicationWrapper _debugApplicationWrapper;

		/// <summary>
		/// The cookie of the debug application
		/// </summary>
		private uint _debugApplicationCookie;

		/// <summary>
		/// Flag indicating whether debugging started
		/// </summary>
		private bool _debuggingStarted;

		/// <summary>
		/// List of document names
		/// </summary>
		private Dictionary<UIntPtr, string> _documentNames = new Dictionary<UIntPtr, string>();

		/// <summary>
		/// List of debug documents
		/// </summary>
		private Dictionary<UIntPtr, DebugDocument> _debugDocuments = new Dictionary<UIntPtr, DebugDocument>();

		/// <summary>
		/// Next source context
		/// </summary>
		private uint _nextSourceContext = 1;

		/// <summary>
		/// Lowest supported version of Internet Explorer
		/// </summary>
		private readonly string _lowerIeVersion;

		/// <summary>
		/// Prefix of error category name
		/// </summary>
		private readonly string _errorCategoryNamePrefix;

		/// <summary>
		/// Flag that indicates if the script interruption is requested
		/// </summary>
		protected bool _interruptRequested;

		/// <summary>
		/// Instance of script dispatcher
		/// </summary>
		private static ScriptDispatcher _dispatcher;

		/// <summary>
		/// Synchronizer of script dispatcher initialization
		/// </summary>
		private static readonly object _dispatcherSynchronizer = new object();


		/// <summary>
		/// Constructs an instance of the Active Script engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="lowerIeVersion">Lowest supported version of Internet Explorer</param>
		/// <param name="errorCategoryNamePrefix">Prefix of error category name</param>
		protected ActiveScriptJsEngineBase(JsEngineSettings settings, string clsid,
			ScriptLanguageVersion languageVersion, string lowerIeVersion, string errorCategoryNamePrefix)
			: base(settings)
		{
			InitScriptDispatcher(_settings.MaxStackSize);

			_lowerIeVersion = lowerIeVersion;
			_errorCategoryNamePrefix = errorCategoryNamePrefix;

			try
			{
				_dispatcher.Invoke(() =>
				{
					_activeScriptWrapper = CreateActiveScriptWrapper(clsid, languageVersion);

					if (_settings.EnableDebugging)
					{
						StartDebugging();
					}

					_activeScriptWrapper.SetScriptSite(CreateScriptSite());
					_activeScriptWrapper.InitNew();
					_activeScriptWrapper.SetScriptState(ScriptState.Started);
					LoadPolyfills();

					_dispatch = WrapScriptDispatch(_activeScriptWrapper.GetScriptDispatch());
				});
			}
			catch (COMException e)
			{
				throw WrapCOMException(e);
			}
			catch (ActiveScriptException e)
			{
				string description = e.Description;
				string message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName, true);

				var wrapperEngineLoadException = new JsEngineLoadException(message, _engineModeName, e)
				{
					Description = description
				};

				throw wrapperEngineLoadException;
			}
			catch (InvalidOperationException e)
			{
				throw JsErrorHelpers.WrapEngineLoadException(e, _engineModeName);
			}
			catch (Exception e)
			{
				throw JsErrorHelpers.WrapEngineLoadException(e, _engineModeName, true);
			}
			finally
			{
				if (_dispatch == null)
				{
					Dispose();
				}
			}
		}


		/// <summary>
		/// Initializes a script dispatcher
		/// </summary>
		/// <param name="maxStackSize">The maximum stack size, in bytes, to be used by the thread,
		/// or 0 to use the default maximum stack size specified in the header for the executable.</param>
		private static void InitScriptDispatcher(int maxStackSize)
		{
			if (_dispatcher != null)
			{
				return;
			}

			lock (_dispatcherSynchronizer)
			{
				if (_dispatcher != null)
				{
					return;
				}

				_dispatcher = new ScriptDispatcher(maxStackSize);
			}
		}

		/// <summary>
		/// Checks a support of the JS engine on the machine
		/// </summary>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="isSupported">Flag indicating whether this JS engine is supported</param>
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
					if (e.ErrorCode == ComErrorCode.E_CLASS_NOT_REGISTERED)
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
		/// Creates a instance of the Active Script wrapper
		/// </summary>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <returns>Instance of the Active Script wrapper</returns>
		private IActiveScriptWrapper CreateActiveScriptWrapper(string clsid, ScriptLanguageVersion languageVersion)
		{
			IActiveScriptWrapper activeScriptWrapper;

			if (Utils.Is64BitProcess())
			{
				activeScriptWrapper = new ActiveScriptWrapper64(clsid, languageVersion, _settings.EnableDebugging);
			}
			else
			{
				activeScriptWrapper = new ActiveScriptWrapper32(clsid, languageVersion, _settings.EnableDebugging);
			}

			return activeScriptWrapper;
		}

		/// <summary>
		/// Starts debugging
		/// </summary>
		private void StartDebugging()
		{
			if (ProcessDebugManagerWrapper.TryCreate(out _processDebugManagerWrapper))
			{
				_processDebugManagerWrapper.CreateApplication(out _debugApplicationWrapper);

				if (_processDebugManagerWrapper.TryAddApplication(_debugApplicationWrapper, out _debugApplicationCookie))
				{
					_debuggingStarted = true;
				}
				else
				{
					_debugApplicationWrapper.Close();
					_debugApplicationWrapper = null;

					_processDebugManagerWrapper = null;
				}
			}
		}

		/// <summary>
		/// Creates a instance of the Active Script site
		/// </summary>
		/// <returns>Instance of the Active Script site</returns>
		protected abstract ScriptSiteBase CreateScriptSite();

		/// <summary>
		/// Initializes a script context
		/// </summary>
		protected virtual void InitScriptContext()
		{
			// Do nothing
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

		/// <summary>
		/// Gets a error type by number
		/// </summary>
		/// <param name="errorNumber">Error number</param>
		/// <returns>Error type</returns>
		protected abstract string GetErrorTypeByNumber(int errorNumber);

		/// <summary>
		/// Loads a JS polyfills
		/// </summary>
		private void LoadPolyfills()
		{
			Assembly assembly = GetType()
#if !NET40
				.GetTypeInfo()
#endif
				.Assembly
				;

			if (_settings.UseEcmaScript5Polyfill)
			{
				InnerExecuteResource(ES5_POLYFILL_RESOURCE_NAME, assembly);
			}

			if (_settings.UseJson2Library)
			{
				InnerExecuteResource(JSON2_LIBRARY_RESOURCE_NAME, assembly);
			}
		}

		/// <summary>
		/// Executes a script text
		/// </summary>
		/// <param name="code">Script text</param>
		/// <param name="documentName">Document name</param>
		/// <param name="isExpression">Flag that script text needs to run as an expression</param>
		/// <returns>Result of the execution</returns>
		private object InnerExecute(string code, string documentName, bool isExpression)
		{
			object result;
			DebugDocument debugDocument;
			UIntPtr sourceContext;
			ScriptTextFlags flags = isExpression ? ScriptTextFlags.IsExpression : ScriptTextFlags.IsVisible;

			if (TryCreateDebugDocument(documentName, code, out sourceContext, out debugDocument))
			{
				flags |= ScriptTextFlags.HostManagesSource;
			}

			try
			{
				result = _activeScriptWrapper.ParseScriptText(code, null, null, null, sourceContext, 0, flags);
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
		/// Executes a code from embedded JS resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		private void InnerExecuteResource(string resourceName, Assembly assembly)
		{
			string code = Utils.GetResourceAsString(resourceName, assembly);
			InnerExecute(code, resourceName, false);
		}

		/// <summary>
		/// Try create a debug document
		/// </summary>
		/// <param name="name">Document name</param>
		/// <param name="code">Script text</param>
		/// <param name="sourceContext">Application specific source context</param>
		/// <param name="document">Debug document</param>
		/// <returns>Result of creating a debug document (true - is created; false - is not created)</returns>
		private bool TryCreateDebugDocument(string name, string code, out UIntPtr sourceContext,
			out DebugDocument document)
		{
			bool result;
			sourceContext = new UIntPtr(_nextSourceContext++);
			document = null;

			if (_debuggingStarted)
			{
				document = new DebugDocument(_activeScriptWrapper, _debugApplicationWrapper, sourceContext,
					name, code);
				_debugDocuments[sourceContext] = document;

				result = true;
			}
			else
			{
				result = false;
			}

			_documentNames[sourceContext] = name;

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
		protected void InnerSetVariableValue(string variableName, object value)
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

		/// <summary>
		/// Removes a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		protected abstract void InnerRemoveVariable(string variableName);

		private void InnerEmbedHostItem(string itemName, object value)
		{
			object oldValue = null;
			if (_hostItems.ContainsKey(itemName))
			{
				oldValue = _hostItems[itemName];
			}
			_hostItems[itemName] = value;

			try
			{
				_activeScriptWrapper.AddNamedItem(itemName, ScriptItemFlags.IsVisible | ScriptItemFlags.GlobalMembers);
			}
			catch
			{
				if (oldValue != null)
				{
					_hostItems[itemName] = oldValue;
				}
				else
				{
					_hostItems.Remove(itemName);
				}

				ThrowError();
				throw;
			}
		}

		/// <summary>
		/// Starts a garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		private void InnerCollectGarbage(ScriptGCType type)
		{
			_activeScriptWrapper.CollectGarbage(type);
		}

		#region Mapping

		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private object MapToScriptType(object value)
		{
			return TypeMappingHelpers.MapToScriptType(value, _settings.EngineMode);
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		private object[] MapToScriptType(object[] args)
		{
			return TypeMappingHelpers.MapToScriptType(args, _settings.EngineMode);
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

		private static IExpando WrapScriptDispatch(object dispatch)
		{
			IExpando wrappedDispatch = null;
			if (dispatch != null && dispatch.GetType().IsCOMObject)
			{
				wrappedDispatch = dispatch as IExpando;
			}

			if (wrappedDispatch == null)
			{
				throw new InvalidOperationException(
					NetFrameworkStrings.Engine_ActiveScriptDispatcherNotInitialized);
			}

			return wrappedDispatch;
		}

		private JsException WrapActiveScriptException(ActiveScriptException originalException)
		{
			JsException wrapperException;
			string message = originalException.Message;
			string category = originalException.Category;

			switch (category)
			{
				case JsErrorCategory.Compilation:
					wrapperException = new JsCompilationException(message, _engineModeName, originalException);
					break;

				case JsErrorCategory.Runtime:
					wrapperException = new JsRuntimeException(message, _engineModeName, originalException)
					{
						CallStack = originalException.CallStack
					};
					break;

				case JsErrorCategory.Interrupted:
					wrapperException = new JsInterruptedException(message, _engineModeName, originalException);
					break;

				default:
					wrapperException = new JsException(message, _engineModeName, originalException);
					break;
			}

			wrapperException.Description = originalException.Description;

			var wrapperScriptException = wrapperException as JsScriptException;
			if (wrapperScriptException != null)
			{
				wrapperScriptException.Type = originalException.Type;
				wrapperScriptException.DocumentName = originalException.DocumentName;
				wrapperScriptException.LineNumber = (int)originalException.LineNumber;
				wrapperScriptException.ColumnNumber = originalException.ColumnNumber;
				wrapperScriptException.SourceFragment = originalException.SourceFragment;
			}

			return wrapperException;
		}

		private JsEngineLoadException WrapCOMException(COMException originalComException)
		{
			string description;
			string message;

			if (originalComException.ErrorCode == ComErrorCode.E_CLASS_NOT_REGISTERED)
			{
				description = string.Format(CommonStrings.Engine_AssemblyNotRegistered,
						_settings.EngineMode == JsEngineMode.Classic ? DllName.JScript : DllName.JScript9) +
					" " +
					string.Format(CommonStrings.Engine_IeInstallationRequired, _lowerIeVersion)
					;
				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName);
			}
			else
			{
				description = originalComException.Message;
				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName, true);
			}

			var wrapperEngineLoadException = new JsEngineLoadException(message, _engineModeName,
				originalComException)
			{
				Description = description
			};

			return wrapperEngineLoadException;
		}

		/// <summary>
		/// Shortens a name of error category
		/// </summary>
		/// <param name="categoryName">Name of error category</param>
		/// <returns>Short name of error category</returns>
		private string ShortenErrorCategoryName(string categoryName)
		{
			return ActiveScriptJsErrorHelpers.ShortenErrorItemName(categoryName, _errorCategoryNamePrefix);
		}

		#endregion

		#region InnerJsEngineBase overrides

		#region IInnerJsEngine implementation

		public override PrecompiledScript Precompile(string code, string documentName)
		{
			throw new NotSupportedException();
		}

		public override object Evaluate(string expression, string documentName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					return InnerExecute(expression, documentName, true);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public override void Execute(string code, string documentName)
		{
			_dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					InnerExecute(code, documentName, false);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});
		}

		public override void Execute(PrecompiledScript precompiledScript)
		{
			throw new NotSupportedException();
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object[] processedArgs = MapToScriptType(args);

			object result = _dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					return InnerCallFunction(functionName, processedArgs);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(CommonStrings.Runtime_FunctionNotExist, functionName),
						_engineModeName
					);
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public override bool HasVariable(string variableName)
		{
			bool result = _dispatcher.Invoke(() =>
			{
				InitScriptContext();

				bool variableExist;

				try
				{
					object variableValue = InnerGetVariableValue(variableName);
					variableExist = variableValue != null;
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
				catch (MissingMemberException)
				{
					variableExist = false;
				}

				return variableExist;
			});

			return result;
		}

		public override object GetVariableValue(string variableName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					return InnerGetVariableValue(variableName);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
				catch (MissingMemberException)
				{
					throw new JsRuntimeException(
						string.Format(NetFrameworkStrings.Runtime_VariableNotExist, variableName),
						_engineModeName
					);
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			object processedValue = MapToScriptType(value);

			_dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					InnerSetVariableValue(variableName, processedValue);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});
		}

		public override void RemoveVariable(string variableName)
		{
			_dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					InnerRemoveVariable(variableName);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});
		}

		public override void EmbedHostObject(string itemName, object value)
		{
			object processedValue = MapToScriptType(value);

			_dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					InnerEmbedHostItem(itemName, processedValue);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			var typeValue = new HostType(type, _settings.EngineMode);

			_dispatcher.Invoke(() =>
			{
				InitScriptContext();

				try
				{
					InnerEmbedHostItem(itemName, typeValue);
				}
				catch (ActiveScriptException e)
				{
					throw WrapActiveScriptException(e);
				}
			});
		}

		public override void CollectGarbage()
		{
			_dispatcher.Invoke(() => InnerCollectGarbage(ScriptGCType.Exhaustive));
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public override void Dispose()
		{
			if (_disposedFlag.Set())
			{
				if (_debuggingStarted && _debugDocuments != null)
				{
					foreach (UIntPtr debugDocumentKey in _debugDocuments.Keys)
					{
						var debugDocumentValue = _debugDocuments[debugDocumentKey];
						debugDocumentValue.Close();
					}

					_debugDocuments.Clear();
					_debugDocuments = null;
				}

				if (_processDebugManagerWrapper != null)
				{
					_processDebugManagerWrapper.RemoveApplication(_debugApplicationCookie);

					if (_debugApplicationWrapper != null)
					{
						_debugApplicationWrapper.Close();
						_debugApplicationWrapper = null;
					}

					_processDebugManagerWrapper = null;
				}

				if (_documentNames != null)
				{
					_documentNames.Clear();
					_documentNames = null;
				}

				_dispatcher.Invoke(() =>
				{
					if (_dispatch != null)
					{
						Marshal.ReleaseComObject(_dispatch);
						_dispatch = null;
					}

					if (_activeScriptWrapper != null)
					{
						_activeScriptWrapper.Dispose();
						_activeScriptWrapper = null;
					}
				});

				if (_hostItems != null)
				{
					_hostItems.Clear();
					_hostItems = null;
				}

				_lastException = null;
			}
		}

		#endregion

		#endregion
	}
}
#endif
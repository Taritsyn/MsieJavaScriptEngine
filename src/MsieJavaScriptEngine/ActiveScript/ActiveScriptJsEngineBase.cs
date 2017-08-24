#if !NETSTANDARD1_3
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
		private IActiveScriptWrapper _activeScriptWrapper;

		/// <summary>
		/// Instance of script dispatch
		/// </summary>
		private IExpando _dispatch;

		/// <summary>
		/// List of host items
		/// </summary>
		private readonly Dictionary<string, object> _hostItems = new Dictionary<string, object>();

		/// <summary>
		/// Last Active Script exception
		/// </summary>
		private ActiveScriptException _lastException;

		/// <summary>
		/// Instance of script dispatcher
		/// </summary>
		private static readonly ScriptDispatcher _dispatcher = new ScriptDispatcher();

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
		/// List of debug documents
		/// </summary>
		private readonly Dictionary<UIntPtr, DebugDocument> _debugDocuments =
			new Dictionary<UIntPtr, DebugDocument>();

		/// <summary>
		/// Next source context
		/// </summary>
		private uint _nextSourceContext = 1;

		/// <summary>
		/// Prefix of error category name
		/// </summary>
		private readonly string _errorCategoryNamePrefix;


		/// <summary>
		/// Constructs an instance of the Active Script engine
		/// </summary>
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		protected ActiveScriptJsEngineBase(JsEngineMode engineMode, bool enableDebugging,
			bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(engineMode)
		{
			string lowerIeVersion;

			if (_engineMode == JsEngineMode.ChakraActiveScript)
			{
				lowerIeVersion = "9";
				_errorCategoryNamePrefix = "JavaScript ";
			}
			else if (_engineMode == JsEngineMode.Classic)
			{
				lowerIeVersion = "6";
				_errorCategoryNamePrefix = "Microsoft JScript ";
			}
			else
			{
				throw new NotSupportedException();
			}

			_dispatcher.Invoke(() =>
			{
				try
				{
					_activeScriptWrapper = Utils.Is64BitProcess() ?
						(IActiveScriptWrapper)new ActiveScriptWrapper64(engineMode, enableDebugging)
						:
						new ActiveScriptWrapper32(engineMode, enableDebugging)
						;
				}
				catch (Exception e)
				{
					throw new JsEngineLoadException(
						string.Format(CommonStrings.Runtime_IeJsEngineNotLoaded,
							_engineModeName, lowerIeVersion, e.Message), _engineModeName);
				}

				if (enableDebugging)
				{
					StartDebugging();
				}

				_activeScriptWrapper.SetScriptSite(new ScriptSite(this));
				_activeScriptWrapper.InitNew();
				_activeScriptWrapper.SetScriptState(ScriptState.Started);

				InitScriptDispatch();
			});

			LoadResources(useEcmaScript5Polyfill, useJson2Library);
		}

		/// <summary>
		/// Destructs an instance of the Active Script engine
		/// </summary>
		~ActiveScriptJsEngineBase()
		{
			Dispose(false);
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
			var jsEngineException = new JsRuntimeException(activeScriptException.Message, _engineModeName,
				activeScriptException)
			{
				ErrorCode = activeScriptException.ErrorCode.ToString(CultureInfo.InvariantCulture),
				Category = ShortenErrorCategoryName(activeScriptException.Category),
				LineNumber = (int)activeScriptException.LineNumber,
				ColumnNumber = activeScriptException.ColumnNumber,
				SourceFragment = activeScriptException.SourceFragment,
				HelpLink = activeScriptException.HelpLink
			};

			return jsEngineException;
		}

		/// <summary>
		/// Shortens a name of error category
		/// </summary>
		/// <param name="categoryName">Name of error category</param>
		/// <returns>Short name of error category</returns>
		private string ShortenErrorCategoryName(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}

			string shortCategoryName = categoryName;
			if (categoryName.StartsWith(_errorCategoryNamePrefix, StringComparison.Ordinal))
			{
				shortCategoryName = categoryName.Substring(_errorCategoryNamePrefix.Length);
				if (shortCategoryName.Length > 0)
				{
					char[] chars = shortCategoryName.ToCharArray();
					chars[0] = char.ToUpperInvariant(chars[0]);
					shortCategoryName = new string(chars);
				}
			}

			return shortCategoryName;
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
		/// Initializes a script dispatch
		/// </summary>
		private void InitScriptDispatch()
		{
			IExpando dispatch = null;
			object obj;

			_activeScriptWrapper.GetScriptDispatch(null, out obj);

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
			UIntPtr sourceContext = CreateDebugDocument(documentName, code, out debugDocument);
			ScriptTextFlags flags = isExpression ? ScriptTextFlags.IsExpression : ScriptTextFlags.IsVisible;
			if (sourceContext != UIntPtr.Zero)
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
		/// Creates a debug document
		/// </summary>
		/// <param name="name">Document name</param>
		/// <param name="code">Script text</param>
		/// <param name="document">Debug document</param>
		/// <returns>Source context</returns>
		private UIntPtr CreateDebugDocument(string name, string code, out DebugDocument document)
		{
			UIntPtr sourceContext;
			if (!_debuggingStarted)
			{
				sourceContext = UIntPtr.Zero;
				document = null;
			}
			else
			{
				sourceContext = new UIntPtr(_nextSourceContext++);
				document = new DebugDocument(_activeScriptWrapper, _debugApplicationWrapper, sourceContext,
					name, code);

				_debugDocuments[sourceContext] = document;
			}

			return sourceContext;
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
			Execute(code, resourceName);
		}

		#region IInnerJsEngine implementation

		public override string Mode
		{
			get { return _engineModeName; }
		}

		public override object Evaluate(string expression, string documentName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				try
				{
					return InnerExecute(expression, documentName, true);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
			});

			result = MapToHostType(result);

			return result;
		}

		public override void Execute(string code, string documentName)
		{
			_dispatcher.Invoke(() =>
			{
				try
				{
					InnerExecute(code, documentName, false);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
			});
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object[] processedArgs = MapToScriptType(args);

			object result = _dispatcher.Invoke(() =>
			{
				try
				{
					return InnerCallFunction(functionName, processedArgs);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
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

		public override bool HasVariable(string variableName)
		{
			bool result = _dispatcher.Invoke(() =>
			{
				bool variableExist;

				try
				{
					object variableValue = InnerGetVariableValue(variableName);
					variableExist = variableValue != null;
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
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
				try
				{
					return InnerGetVariableValue(variableName);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
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

		public override void SetVariableValue(string variableName, object value)
		{
			object processedValue = MapToScriptType(value);

			_dispatcher.Invoke(() =>
			{
				try
				{
					InnerSetVariableValue(variableName, processedValue);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
			});
		}

		public override void RemoveVariable(string variableName)
		{
			_dispatcher.Invoke(() =>
			{
				try
				{
					InnerSetVariableValue(variableName, null);

					if (_hostItems.ContainsKey(variableName))
					{
						_hostItems.Remove(variableName);
					}
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
			});
		}

		public override void EmbedHostObject(string itemName, object value)
		{
			object processedValue = MapToScriptType(value);

			_dispatcher.Invoke(() =>
			{
				try
				{
					InnerEmbedHostItem(itemName, processedValue);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
				}
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			var typeValue = new HostType(type, _engineMode);

			_dispatcher.Invoke(() =>
			{
				try
				{
					InnerEmbedHostItem(itemName, typeValue);
				}
				catch (ActiveScriptException e)
				{
					throw ConvertActiveScriptExceptionToJsRuntimeException(e);
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

					if (_activeScriptWrapper != null)
					{
						_activeScriptWrapper.Dispose();
						_activeScriptWrapper = null;
					}
				});

				if (disposing)
				{
					if (_debuggingStarted && _debugDocuments != null)
					{
						foreach (UIntPtr debugDocumentKey in _debugDocuments.Keys)
						{
							var debugDocumentValue = _debugDocuments[debugDocumentKey];
							debugDocumentValue.Close();
						}

						_debugDocuments.Clear();
					}

					if (_processDebugManagerWrapper != null)
					{
						_processDebugManagerWrapper.RemoveApplication(_debugApplicationCookie);
						_debugApplicationWrapper.Close();
					}

					if (_hostItems != null)
					{
						_hostItems.Clear();
					}

					_lastException = null;
				}
			}
		}

		#endregion
	}
}
#endif
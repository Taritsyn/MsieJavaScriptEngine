namespace MsieJavaScriptEngine.ActiveScript
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.Expando;

	using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

	using Helpers;
	using Resources;

	internal class ActiveScriptSiteWrapper : IActiveScriptSite, IDisposable
	{
		private const int TYPE_ERROR_ELEMENT_NOT_FOUND = unchecked((int)(0x8002802B));

		/// <summary>
		/// Instance of native JavaScript engine
		/// </summary>
		private IActiveScript _activeScript;

		/// <summary>
		/// Instance of ActiveScriptParseWrapper
		/// </summary>
		private IActiveScriptParseWrapper _activeScriptParse;

		/// <summary>
		/// Instance of script dispatch
		/// </summary>
		private IExpando _dispatch;

		/// <summary>
		/// List of site items
		/// </summary>
		private Dictionary<string, object> _siteItems = new Dictionary<string, object>();

		/// <summary>
		/// Last ActiveScript exception
		/// </summary>
		private ActiveScriptException _lastException;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// Gets or sets a host-defined document version string
		/// </summary>
		public string DocumentVersion
		{
			get;
			protected set;
		}


		/// <summary>
		/// Constructs instance of <see cref="ActiveScriptSiteWrapper"/>
		/// </summary>
		/// <param name="pActiveScript">Pointer to an instance of native JavaScript engine</param>
		/// <param name="activeScript">Instance of native JavaScript engine</param>
		public ActiveScriptSiteWrapper(IntPtr pActiveScript, IActiveScript activeScript)
			: this(pActiveScript, activeScript, DateTime.UtcNow.ToString("o")) 
		{ }
		
		/// <summary>
		/// Constructs instance of <see cref="ActiveScriptSiteWrapper"/>
		/// </summary>
		/// <param name="pActiveScript">Pointer to an instance of native JavaScript engine</param>
		/// <param name="activeScript">Instance of native JavaScript engine</param>
		/// <param name="documentVersion">Host-defined document version string</param>
		public ActiveScriptSiteWrapper(IntPtr pActiveScript, IActiveScript activeScript, 
			string documentVersion)
		{
			_activeScript = activeScript;

			_activeScriptParse = new ActiveScriptParseWrapper(pActiveScript, _activeScript);
			_activeScriptParse.InitNew();

			_activeScript.SetScriptSite(this);
			_activeScript.SetScriptState(ScriptState.Started);

			InitScriptDispatch();

			DocumentVersion = documentVersion;
		}
		
		/// <summary>
		/// Destructs instance of <see cref="ActiveScriptSiteWrapper"/>
		/// </summary>
		~ActiveScriptSiteWrapper()
		{
			Dispose(false);
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
				throw new InvalidOperationException(Strings.Runtime_ActiveScriptDispatcherNotInitialized);
			}

			_dispatch = dispatch;
		}

		/// <summary>
		/// Allows the scripting engine to obtain information about an item added with the
		/// IActiveScript.AddNamedItem method
		/// </summary>
		/// <param name="name">The name associated with the item, as specified in the
		/// IActiveScript.AddNamedItem method</param>
		private object GetItem(string name)
		{
			lock (_siteItems)
			{
				object result;

				return _siteItems.TryGetValue(name, out result) ? result : null;
			}
		}

		/// <summary>
		/// Allows the scripting engine to obtain information about an item added with the
		/// IActiveScript.AddNamedItem method. Gets the COM ITypeInfo
		/// </summary>
		/// <param name="name">The name associated with the item, as specified in the
		/// IActiveScript.AddNamedItem method</param>
		private IntPtr GetTypeInfo(string name)
		{
			lock (_siteItems)
			{
				if (!_siteItems.ContainsKey(name))
				{
					return IntPtr.Zero;
				}

				return Marshal.GetITypeInfoForType(_siteItems[name].GetType());
			}
		}

		/// <summary>
		/// Gets and resets a last exception. Returns null for none.
		/// </summary>
		private ActiveScriptException GetAndResetLastException()
		{
			var temp = _lastException;
			_lastException = null;

			return temp;
		}

		private void ThrowError()
		{
			var last = GetAndResetLastException();
			if (last != null)
			{
				throw last;
			}
		}

		/// <summary>
		/// Executes a script text
		/// </summary>
		/// <param name="code">Script text</param>
		/// <param name="isExpression">Flag that script text needs to run as an expression</param>
		/// <returns>Result of the execution</returns>
		public object ExecuteScriptText(string code, bool isExpression)
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
		public object CallFunction(string functionName, params object[] args)
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
		/// Сhecks for the existence of a global object property
		/// </summary>
		/// <param name="propertyName">Name of property</param>
		/// <returns>Result of check (true - exists; false - not exists)</returns>
		public bool HasProperty(string propertyName)
		{
			bool propertyExist;

			try
			{
				object propertyValue = GetProperty(propertyName);
				propertyExist = (propertyValue != null);
			}
			catch (MissingMemberException)
			{
				propertyExist = false;
			}
			catch
			{
				ThrowError();
				throw;
			}

			return propertyExist;
		}

		/// <summary>
		/// Gets a value of global object property
		/// </summary>
		/// <param name="propertyName">Name of property</param>
		/// <returns>Value of property</returns>
		public object GetProperty(string propertyName)
		{
			object propertyValue;

			try
			{
				propertyValue = _dispatch.InvokeMember(propertyName, BindingFlags.GetProperty,
					null, _dispatch, new object[0], null,
					CultureInfo.InvariantCulture, null);
			}
			catch
			{
				ThrowError();
				throw;
			}

			return propertyValue;
		}

		/// <summary>
		/// Sets a value to global object property
		/// </summary>
		/// <param name="propertyName">Name of property</param>
		/// <param name="value">Value of property</param>
		public void SetProperty(string propertyName, object value)
		{
			var marshaledArgs = new[] { value };
			try
			{
				_dispatch.InvokeMember(propertyName, BindingFlags.SetProperty, null, _dispatch,
					marshaledArgs, null, CultureInfo.InvariantCulture, null);
			}
			catch (MissingMemberException)
			{
				_dispatch.AddProperty(propertyName);
				_dispatch.InvokeMember(propertyName, BindingFlags.SetProperty, null, _dispatch,
					marshaledArgs, null, CultureInfo.InvariantCulture, null);
			}
			catch
			{
				ThrowError();
				throw;
			}
		}

		/// <summary>
		/// Removes a global object property
		/// </summary>
		/// <param name="propertyName">Name of property</param>
		public void DeleteProperty(string propertyName)
		{
			try
			{
				SetProperty(propertyName, null);
			}
			catch
			{
				ThrowError();
				throw;
			}
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				_lastException = null;

				if (_siteItems != null)
				{
					_siteItems.Clear();
					_siteItems = null;
				}

				if (_dispatch != null)
				{
					ComHelpers.ReleaseComObject(ref _dispatch, !disposing);
					_dispatch = null;
				}

				if (_activeScriptParse != null)
				{
					_activeScriptParse.Dispose();
					_activeScriptParse = null;
				}

				_activeScript = null;
			}
		}

		#region IActiveScriptSite implementation

		/// <summary>
		/// Retrieves the locale identifier associated with the host's user interface. The scripting
		/// engine uses the identifier to ensure that error strings and other user-interface elements
		/// generated by the engine appear in the appropriate language.
		/// </summary>
		/// <param name="lcid">A variable that receives the locale identifier for user-interface
		/// elements displayed by the scripting engine</param>
		public void GetLcid(out int lcid)
		{
			lcid = CultureInfo.CurrentCulture.LCID;
		}

		/// <summary>
		/// Allows the scripting engine to obtain information about an item added with the
		/// IActiveScript.AddNamedItem method
		/// </summary>
		/// <param name="name">The name associated with the item, as specified in the
		/// IActiveScript.AddNamedItem method</param>
		/// <param name="returnMask">A bit mask specifying what information about the item should be
		/// returned. The scripting engine should request the minimum amount of information possible
		/// because some of the return parameters (for example, ITypeInfo) can take considerable
		/// time to load or generate</param>
		/// <param name="item">A variable that receives a pointer to the IUnknown interface associated
		/// with the given item. The scripting engine can use the IUnknown.QueryInterface method to
		/// obtain the IDispatch interface for the item. This parameter receives null if returnMask
		/// does not include the ScriptInfo.IUnknown value. Also, it receives null if there is no
		/// object associated with the item name; this mechanism is used to create a simple class when
		/// the named item was added with the ScriptItem.CodeOnly flag set in the
		/// IActiveScript.AddNamedItem method.</param>
		/// <param name="pTypeInfo">A variable that receives a pointer to the ITypeInfo interface
		/// associated with the item. This parameter receives null if returnMask does not include the
		/// ScriptInfo.ITypeInfo value, or if type information is not available for this item. If type
		/// information is not available, the object cannot source events, and name binding must be
		/// realized with the IDispatch.GetIDsOfNames method. Note that the ITypeInfo interface
		/// retrieved describes the item's coclass (TKIND_COCLASS) because the object may support
		/// multiple interfaces and event interfaces. If the item supports the IProvideMultipleTypeInfo
		/// interface, the ITypeInfo interface retrieved is the same as the index zero ITypeInfo that
		/// would be obtained using the IProvideMultipleTypeInfo.GetInfoOfIndex method.</param>
		public void GetItemInfo(string name, ScriptInfoFlags returnMask, out object item, out IntPtr pTypeInfo)
		{
			if ((returnMask & ScriptInfoFlags.IUnknown) > 0)
			{
				item = GetItem(name);
				if (item == null)
				{
					throw new COMException(string.Format(Strings.Runtime_ItemNotFound, name), TYPE_ERROR_ELEMENT_NOT_FOUND);
				}
			}
			else
			{
				item = null;
			}

			if ((returnMask & ScriptInfoFlags.ITypeInfo) > 0)
			{
				pTypeInfo = GetTypeInfo(name);
			}
			else
			{
				pTypeInfo = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Retrieves a host-defined string that uniquely identifies the current document version. If
		/// the related document has changed outside the scope of Windows Script (as in the case of an
		/// HTML page being edited with Notepad), the scripting engine can save this along with its
		/// persisted state, forcing a recompile the next time the script is loaded.
		/// </summary>
		/// <param name="version">The host-defined document version string</param>
		public void GetDocVersionString(out string version)
		{
			version = DocumentVersion;
		}

		/// <summary>
		/// Informs the host that the script has completed execution
		/// </summary>
		/// <param name="result">A variable that contains the script result, or null if the script
		/// produced no result</param>
		/// <param name="exceptionInfo">Contains exception information generated when the script
		/// terminated, or null if no exception was generated</param>
		public virtual void OnScriptTerminate(object result, EXCEPINFO exceptionInfo)
		{ }

		/// <summary>
		/// Informs the host that the scripting engine has changed states
		/// </summary>
		/// <param name="scriptState">Indicates the new script state</param>
		public virtual void OnStateChange(ScriptState scriptState)
		{ }

		/// <summary>
		/// Informs the host that an execution error occurred while the engine was running the script.
		/// </summary>
		/// <param name="scriptError">A host can use this interface to obtain information about the
		/// execution error</param>
		public void OnScriptError(IActiveScriptError scriptError)
		{
			_lastException = ActiveScriptException.Create(scriptError);
			OnScriptError(_lastException);
		}

		/// <summary>
		/// Informs the host that an execution error occurred while the engine was running the script
		/// </summary>
		/// <param name="exception">The exception</param>
		protected virtual void OnScriptError(ActiveScriptException exception)
		{ }

		/// <summary>
		/// Informs the host that the scripting engine has begun executing the script code
		/// </summary>
		public virtual void OnEnterScript()
		{ }

		/// <summary>
		/// Informs the host that the scripting engine has returned from executing script code
		/// </summary>
		public virtual void OnLeaveScript()
		{ }

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
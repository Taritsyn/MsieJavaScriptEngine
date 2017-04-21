#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script wrapper
	/// </summary>
	internal sealed class ActiveScriptWrapper
	{
		/// <summary>
		/// Flag that the current process is a 64-bit process
		/// </summary>
		private readonly bool _is64Bit;

		/// <summary>
		/// Pointer to an instance of Active Script engine
		/// </summary>
		private IntPtr _pActiveScript;

		/// <summary>
		/// Pointer to an instance of 32-bit Active Script parser
		/// </summary>
		private IntPtr _pActiveScriptParse32;

		/// <summary>
		/// Pointer to an instance of 64-bit Active Script parser
		/// </summary>
		private IntPtr _pActiveScriptParse64;

		/// <summary>
		/// Pointer to an instance of 32-bit Active Script debugger
		/// </summary>
		private IntPtr _pActiveScriptDebug32;

		/// <summary>
		/// Pointer to an instance of 64-bit Active Script debugger
		/// </summary>
		private IntPtr _pActiveScriptDebug64;

		/// <summary>
		/// Pointer to an instance of Active Script garbage collector
		/// </summary>
		private IntPtr _pActiveScriptGarbageCollector;

		/// <summary>
		/// Instance of Active Script engine
		/// </summary>
		private IActiveScript _activeScript;

		/// <summary>
		/// Instance of 32-bit Active Script parser
		/// </summary>
		private IActiveScriptParse32 _activeScriptParse32;

		/// <summary>
		/// Instance of 64-bit Active Script parser
		/// </summary>
		private IActiveScriptParse64 _activeScriptParse64;

		/// <summary>
		/// Instance of Active Script garbage collector
		/// </summary>
		private IActiveScriptGarbageCollector _activeScriptGarbageCollector;

		/// <summary>
		/// Last COM exception
		/// </summary>
		private EXCEPINFO _lastException;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private StatedFlag _disposedFlag = new StatedFlag();

		/// <summary>
		/// Gets a last COM exception
		/// </summary>
		public EXCEPINFO LastException
		{
			get { return _lastException; }
		}


		/// <summary>
		/// Constructs an instance of the Active Script wrapper
		/// </summary>
		/// <param name="clsid">CLSID of script engine</param>
		/// <param name="languageVersion">Version of script language</param>
		public ActiveScriptWrapper(string clsid, ScriptLanguageVersion languageVersion)
		{
			_is64Bit = Utils.Is64BitProcess();

			_pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
			if (_is64Bit)
			{
				_pActiveScriptParse64 = ComHelpers.QueryInterface<IActiveScriptParse64>(_pActiveScript);
				_pActiveScriptDebug64 = ComHelpers.QueryInterface<IActiveScriptDebug64>(_pActiveScript);
			}
			else
			{
				_pActiveScriptParse32 = ComHelpers.QueryInterface<IActiveScriptParse32>(_pActiveScript);
				_pActiveScriptDebug32 = ComHelpers.QueryInterface<IActiveScriptDebug32>(_pActiveScript);
			}
			_pActiveScriptGarbageCollector = ComHelpers.QueryInterfaceNoThrow<IActiveScriptGarbageCollector>(_pActiveScript);

			_activeScript = (IActiveScript)Marshal.GetObjectForIUnknown(_pActiveScript);
			if (_is64Bit)
			{
				_activeScriptParse64 = (IActiveScriptParse64)_activeScript;
			}
			else
			{
				_activeScriptParse32 = (IActiveScriptParse32)_activeScript;
			}
			_activeScriptGarbageCollector = _activeScript as IActiveScriptGarbageCollector;

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
		}


		/// <summary>
		/// Informs the scripting engine of the <see cref="IActiveScriptSite"/> interface site
		/// provided by the host. Call this method before any other <see cref="IActiveScript"/>
		/// interface methods is used.
		/// </summary>
		/// <param name="site">The host-supplied script site to be associated with this instance
		/// of the scripting engine. The site must be uniquely assigned to this scripting engine
		/// instance; it cannot be shared with other scripting engines.</param>
		public void SetScriptSite(IActiveScriptSite site)
		{
			_activeScript.SetScriptSite(site);
		}

		/// <summary>
		/// Puts the scripting engine into the given state. This method can be called from non-base
		/// threads without resulting in a non-base callout to host objects or to the
		/// <see cref="IActiveScriptSite"/> interface.
		/// </summary>
		/// <param name="state">Sets the scripting engine to the given state</param>
		public void SetScriptState(ScriptState state)
		{
			_activeScript.SetScriptState(state);
		}

		/// <summary>
		/// Adds the name of a root-level item to the scripting engine's name space. A root-level item
		/// is an object with properties and methods, an event source, or all three.
		/// </summary>
		/// <param name="name">The name of the item as viewed from the script. The name must be unique
		/// and persistable</param>
		/// <param name="flags">Flags associated with an item</param>
		public void AddNamedItem(string name, ScriptItemFlags flags)
		{
			_activeScript.AddNamedItem(name, flags);
		}

		/// <summary>
		/// Retrieves the IDispatch interface for the methods and properties associated
		/// with the currently running script
		/// </summary>
		/// <param name="itemName">The name of the item for which the caller needs the associated
		/// dispatch object. If this parameter is null, the dispatch object contains as its members
		/// all of the global methods and properties defined by the script. Through the
		/// IDispatch interface and the associated <see cref="ITypeInfo"/> interface, the host can
		/// invoke script methods or view and modify script variables.</param>
		/// <param name="dispatch">The object associated with the script's global methods and
		/// properties. If the scripting engine does not support such an object, null is returned.</param>
		public void GetScriptDispatch(string itemName, out object dispatch)
		{
			_activeScript.GetScriptDispatch(itemName, out dispatch);
		}

		/// <summary>
		/// Initializes the scripting engine
		/// </summary>
		public void InitNew()
		{
			if (_is64Bit)
			{
				_activeScriptParse64.InitNew();
			}
			else
			{
				_activeScriptParse32.InitNew();
			}
		}

		/// <summary>
		/// Parses the given code scriptlet, adding declarations into the namespace and
		/// evaluating code as appropriate
		/// </summary>
		/// <param name="code">The scriptlet text to evaluate. The interpretation of this
		/// string depends on the scripting language</param>
		/// <param name="itemName">The item name that gives the context in which the
		/// scriptlet is to be evaluated. If this parameter is null, the code is evaluated
		/// in the scripting engine's global context</param>
		/// <param name="context">The context object. This object is reserved for use in a
		/// debugging environment, where such a context may be provided by the debugger to
		/// represent an active run-time context. If this parameter is null, the engine
		/// uses <paramref name="itemName"/> to identify the context.</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When <paramref name="code"/>
		/// is parsed from a stream of text, the host typically uses a delimiter, such as two
		/// single quotation marks (''), to detect the end of the scriptlet. This parameter
		/// specifies the delimiter that the host used, allowing the scripting engine to provide
		/// some conditional primitive preprocessing (for example, replacing a single quotation
		/// mark ['] with two single quotation marks for use as a delimiter). Exactly how
		/// (and if) the scripting engine makes use of this information depends on the
		/// scripting engine. Set this parameter to null if the host did not use a delimiter
		/// to mark the end of the scriptlet.</param>
		/// <param name="sourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <returns>The results of scriptlet processing, or null if the caller expects no
		/// result (that is, the <see cref="ScriptTextFlags.IsExpression"/> value is not set)</returns>
		public object ParseScriptText(string code, string itemName, object context, string delimiter,
			UIntPtr sourceContextCookie, uint startingLineNumber, ScriptTextFlags flags)
		{
			object result;

			if (_is64Bit)
			{
				_activeScriptParse64.ParseScriptText(
					code,
					itemName,
					context,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out result,
					out _lastException);
			}
			else
			{
				_activeScriptParse32.ParseScriptText(
					code,
					itemName,
					context,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out result,
					out _lastException);
			}

			return result;
		}

		/// <summary>
		/// Used by a smart host to delegate the <see cref="IDebugDocumentContext.EnumCodeContexts"/> method
		/// </summary>
		/// <param name="sourceContext">The source context as provided to
		/// <see cref="IActiveScriptParse32.ParseScriptText"/> or
		/// <see cref="IActiveScriptParse32.AddScriptlet"/></param>
		/// <param name="offset">Character offset relative to start of script text</param>
		/// <param name="length">Number of characters in this context</param>
		/// <param name="enumContexts">An enumerator of the code contexts in the specified range</param>
		public void EnumCodeContextsOfPosition(UIntPtr sourceContext, uint offset, uint length,
			out IEnumDebugCodeContexts enumContexts)
		{
			uint result;

			if (_is64Bit)
			{
				var del = ComHelpers.GetMethodDelegate<RawEnumCodeContextsOfPosition64>(_pActiveScriptDebug64, 5);
				result = del(_pActiveScriptDebug64, sourceContext.ToUInt64(), offset, length, out enumContexts);
			}
			else
			{
				var del = ComHelpers.GetMethodDelegate<RawEnumCodeContextsOfPosition32>(_pActiveScriptDebug32, 5);
				result = del(_pActiveScriptDebug32, sourceContext.ToUInt32(), offset, length, out enumContexts);
			}

			ComHelpers.HResult.Check(result);
		}

		/// <summary>
		/// The Active Script host calls this method to start garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		public void CollectGarbage(ScriptGCType type)
		{
			if (_activeScriptGarbageCollector != null)
			{
				_activeScriptGarbageCollector.CollectGarbage(type);
			}
		}

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			if (_disposedFlag.Set())
			{
				_activeScriptGarbageCollector = null;
				if (_is64Bit)
				{
					_activeScriptParse64 = null;
				}
				else
				{
					_activeScriptParse32 = null;
				}

				ComHelpers.ReleaseAndEmpty(ref _pActiveScriptGarbageCollector);
				if (_is64Bit)
				{
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptDebug64);
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse64);
				}
				else
				{
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptDebug32);
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse32);
				}
				ComHelpers.ReleaseAndEmpty(ref _pActiveScript);

				if (_activeScript != null)
				{
					_activeScript.Close();
					Marshal.FinalReleaseComObject(_activeScript);
					_activeScript = null;
				}
			}
		}

		#endregion
	}
}
#endif
#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Base class of the Active Script wrapper
	/// </summary>
	internal abstract class ActiveScriptWrapperBase : IActiveScriptWrapper
	{
		/// <summary>
		/// JS engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

		/// <summary>
		/// Flag for whether to enable script debugging features
		/// </summary>
		protected readonly bool _enableDebugging;

		/// <summary>
		/// Pointer to an instance of Active Script engine
		/// </summary>
		protected IntPtr _pActiveScript;

		/// <summary>
		/// Pointer to an instance of Active Script garbage collector
		/// </summary>
		private IntPtr _pActiveScriptGarbageCollector;

		/// <summary>
		/// Instance of Active Script engine
		/// </summary>
		protected IActiveScript _activeScript;

		/// <summary>
		/// Instance of Active Script garbage collector
		/// </summary>
		private IActiveScriptGarbageCollector _activeScriptGarbageCollector;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		protected StatedFlag _disposedFlag = new StatedFlag();


		/// <summary>
		/// Constructs an instance of the Active Script wrapper
		/// </summary>
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		protected ActiveScriptWrapperBase(JsEngineMode engineMode, bool enableDebugging)
		{
			_engineMode = engineMode;
			_enableDebugging = enableDebugging;

			string clsid;
			ScriptLanguageVersion languageVersion;

			if (engineMode == JsEngineMode.ChakraActiveScript)
			{
				clsid = ClassId.Chakra;
				languageVersion = ScriptLanguageVersion.EcmaScript5;
			}
			else
			{
				clsid = ClassId.Classic;
				languageVersion = ScriptLanguageVersion.None;
			}

			_pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
			_pActiveScriptGarbageCollector = ComHelpers.QueryInterfaceNoThrow<IActiveScriptGarbageCollector>(
				_pActiveScript);

			_activeScript = (IActiveScript)Marshal.GetObjectForIUnknown(_pActiveScript);
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


		protected abstract uint InnerEnumCodeContextsOfPosition(UIntPtr sourceContext, uint offset,
			uint length, out IEnumDebugCodeContexts enumContexts);

		#region IActiveScriptWrapper implementation

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
		public abstract void InitNew();

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
		public abstract object ParseScriptText(string code, string itemName, object context, string delimiter,
			UIntPtr sourceContextCookie, uint startingLineNumber, ScriptTextFlags flags);

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
			if (_enableDebugging)
			{
				uint result = InnerEnumCodeContextsOfPosition(sourceContext, offset, length, out enumContexts);
				ComHelpers.HResult.Check(result);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		public abstract void EnumStackFrames(out IEnumDebugStackFrames enumFrames);

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

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public virtual void Dispose()
		{
			_activeScriptGarbageCollector = null;

			ComHelpers.ReleaseAndEmpty(ref _pActiveScriptGarbageCollector);
			ComHelpers.ReleaseAndEmpty(ref _pActiveScript);

			if (_activeScript != null)
			{
				_activeScript.Close();
				Marshal.FinalReleaseComObject(_activeScript);
				_activeScript = null;
			}
		}

		#endregion
	}
}
#endif
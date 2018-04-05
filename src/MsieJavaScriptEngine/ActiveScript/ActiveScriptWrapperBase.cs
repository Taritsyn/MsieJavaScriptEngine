#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Base class of the Active Script wrapper
	/// </summary>
	internal abstract class ActiveScriptWrapperBase : IActiveScriptWrapper
	{
		/// <summary>
		/// Flag for whether to enable script debugging features
		/// </summary>
		protected readonly bool _enableDebugging;

		/// <summary>
		/// Pointer to an instance of Active Script engine
		/// </summary>
		private IntPtr _pActiveScript;

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
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		protected ActiveScriptWrapperBase(string clsid, ScriptLanguageVersion languageVersion,
			bool enableDebugging)
		{
			_enableDebugging = enableDebugging;

			_pActiveScript = ComHelpers.CreateInstanceByClsid<IActiveScript>(clsid);
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
					if (result != ComErrorCode.S_OK)
					{
						throw new InvalidOperationException(
							string.Format(NetFrameworkStrings.Engine_ActiveScriptLanguageVersionSelectionFailed,
								languageVersion)
						);
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
		/// Gets a script dispatch
		/// </summary>
		/// <returns>The object associated with the script's global methods and properties</returns>
		public object GetScriptDispatch()
		{
			object dispatch;
			_activeScript.GetScriptDispatch(null, out dispatch);

			return dispatch;
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
		/// Interrupts the execution of a running script thread (an event sink, an immediate execution,
		/// or a macro invocation). This method can be used to terminate a script that is stuck (for
		/// example, in an infinite loop). It can be called from non-base threads without resulting in
		/// a non-base callout to host objects or to the <see cref="IActiveScriptSite"/> method.
		/// </summary>
		/// <param name="scriptThreadId">Identifier of the thread to interrupt</param>
		/// <param name="exceptionInfo">The error information that should be reported to the aborted script</param>
		/// <param name="flags">Option flags associated with the interruption</param>
		public void InterruptScriptThread(uint scriptThreadId, ref EXCEPINFO exceptionInfo,
			ScriptInterruptFlags flags)
		{
			_activeScript.InterruptScriptThread(scriptThreadId, ref exceptionInfo, flags);
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

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public virtual void Dispose()
		{
			_activeScriptGarbageCollector = null;

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
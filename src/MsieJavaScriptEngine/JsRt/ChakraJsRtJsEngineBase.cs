using System;
#if NETSTANDARD1_3
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// JsRT version of Chakra JS engine
	/// </summary>
	internal abstract class ChakraJsRtJsEngineBase : InnerJsEngineBase
	{
		/// <summary>
		/// JS source context
		/// </summary>
		protected JsSourceContext _jsSourceContext = JsSourceContext.FromIntPtr(IntPtr.Zero);

		/// <summary>
		/// Flag for whether to enable script debugging features
		/// </summary>
		protected readonly bool _enableDebugging;

		/// <summary>
		/// Flag indicating whether debugging started
		/// </summary>
		private StatedFlag _debuggingStartedFlag;
#if NETSTANDARD1_3

		/// <summary>
		/// Set of external objects
		/// </summary>
		protected readonly HashSet<object> _externalObjects = new HashSet<object>();

		/// <summary>
		/// Callback for finalization of external object
		/// </summary>
		protected JsObjectFinalizeCallback _externalObjectFinalizeCallback;
#endif

		/// <summary>
		/// Script dispatcher
		/// </summary>
		protected readonly ScriptDispatcher _dispatcher = new ScriptDispatcher();


		/// <summary>
		/// Constructs an instance of the Chakra JsRT engine
		/// </summary>
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		protected ChakraJsRtJsEngineBase(JsEngineMode engineMode, bool enableDebugging)
			: base(engineMode)
		{
			_enableDebugging = enableDebugging;
#if NETSTANDARD1_3
			_externalObjectFinalizeCallback = ExternalObjectFinalizeCallback;
#endif
		}


		/// <summary>
		/// Starts debugging
		/// </summary>
		protected void StartDebugging()
		{
			if (_debuggingStartedFlag.Set())
			{
				InnerStartDebugging();
			}
		}

		protected abstract void InnerStartDebugging();
#if NETSTANDARD1_3

		private void ExternalObjectFinalizeCallback(IntPtr data)
		{
			if (data == IntPtr.Zero)
			{
				return;
			}

			GCHandle handle = GCHandle.FromIntPtr(data);
			object obj = handle.Target;

			if (obj == null)
			{
				return;
			}

			if (_externalObjects != null)
			{
				_externalObjects.Remove(obj);
			}
		}
#endif

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of
		/// managed objects contained in fields of class</param>
		protected virtual void Dispose(bool disposing)
		{
#if NETSTANDARD1_3
			if (disposing)
			{
				if (_externalObjects != null)
				{
					_externalObjects.Clear();
				}

				_externalObjectFinalizeCallback = null;
			}
#endif
		}

		#endregion
	}
}
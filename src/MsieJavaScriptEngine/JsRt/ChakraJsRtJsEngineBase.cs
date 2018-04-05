using System;
#if NETSTANDARD
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif

using MsieJavaScriptEngine.Utilities;

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
		/// Flag indicating whether debugging started
		/// </summary>
		private StatedFlag _debuggingStartedFlag;
#if NETSTANDARD

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
		/// <param name="settings">JS engine settings</param>
		protected ChakraJsRtJsEngineBase(JsEngineSettings settings)
			: base(settings)
		{
#if NETSTANDARD
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
#if NETSTANDARD

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
#if NETSTANDARD
			if (disposing)
			{
				_externalObjects?.Clear();

				_externalObjectFinalizeCallback = null;
			}
#endif
		}

		#endregion
	}
}
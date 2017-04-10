using System;
#if NETSTANDARD1_3
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif

using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// Base class of the Chakra JsRT JavaScript engine
	/// </summary>
	internal abstract class ChakraJsRtJsEngineBase : IInnerJsEngine
	{
		/// <summary>
		/// JS source context
		/// </summary>
		protected JsSourceContext _jsSourceContext = JsSourceContext.FromIntPtr(IntPtr.Zero);

		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		protected readonly string _engineModeName;

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
		/// Flag that object is destroyed
		/// </summary>
		protected StatedFlag _disposedFlag = new StatedFlag();


		/// <summary>
		/// Constructs an instance of the Chakra JsRT JavaScript engine
		/// </summary>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		protected ChakraJsRtJsEngineBase(JsEngineMode engineMode, bool enableDebugging)
		{
			_engineMode = engineMode;
			_engineModeName = JsEngineModeHelpers.GetModeName(engineMode);
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

		#region IInnerJsEngine implementation

		public abstract string Mode { get; }


		public object Evaluate(string expression)
		{
			return Evaluate(expression, string.Empty);
		}

		public abstract object Evaluate(string expression, string documentName);

		public void Execute(string code)
		{
			Execute(code, string.Empty);
		}

		public abstract void Execute(string code, string documentName);

		public abstract object CallFunction(string functionName, params object[] args);

		public abstract bool HasVariable(string variableName);

		public abstract object GetVariableValue(string variableName);

		public abstract void SetVariableValue(string variableName, object value);

		public abstract void RemoveVariable(string variableName);

		public abstract void EmbedHostObject(string itemName, object value);

		public abstract void EmbedHostType(string itemName, Type type);

		public abstract void CollectGarbage();

		#endregion

		#region IDisposable implementation

		public abstract void Dispose();

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
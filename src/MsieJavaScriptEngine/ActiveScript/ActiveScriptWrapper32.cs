#if !NETSTANDARD1_3
using System;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// 32-bit Active Script wrapper
	/// </summary>
	internal sealed class ActiveScriptWrapper32 : ActiveScriptWrapperBase
	{
		/// <summary>
		/// Pointer to an instance of 32-bit Active Script parser
		/// </summary>
		private IntPtr _pActiveScriptParse32;

		/// <summary>
		/// Pointer to an instance of 32-bit Active Script debugger
		/// </summary>
		private IntPtr _pActiveScriptDebug32;

		/// <summary>
		/// Pointer to an instance of 32-bit debug stack frame sniffer
		/// </summary>
		private IntPtr _pDebugStackFrameSniffer32;

		/// <summary>
		/// Instance of 32-bit Active Script parser
		/// </summary>
		private IActiveScriptParse32 _activeScriptParse32;

		/// <summary>
		/// Instance of 32-bit Active Script debugger
		/// </summary>
		private IActiveScriptDebug32 _activeScriptDebug32;

		/// <summary>
		/// Instance of 32-bit debug stack frame sniffer
		/// </summary>
		private IDebugStackFrameSnifferEx32 _debugStackFrameSniffer32;


		/// <summary>
		/// Constructs an instance of the 32-bit Active Script wrapper
		/// </summary>
		/// <param name="engineMode">JS engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ActiveScriptWrapper32(JsEngineMode engineMode, bool enableDebugging)
			: base(engineMode, enableDebugging)
		{
			_pActiveScriptParse32 = ComHelpers.QueryInterface<IActiveScriptParse32>(_pActiveScript);
			_activeScriptParse32 = (IActiveScriptParse32)_activeScript;

			if (_enableDebugging)
			{
				_pActiveScriptDebug32 = ComHelpers.QueryInterface<IActiveScriptDebug32>(_pActiveScript);
				_activeScriptDebug32 = (IActiveScriptDebug32)_activeScript;

				if (engineMode == JsEngineMode.Classic)
				{
					_pDebugStackFrameSniffer32 = ComHelpers.QueryInterfaceNoThrow<IDebugStackFrameSnifferEx32>(
						_pActiveScript);
					_debugStackFrameSniffer32 = _activeScript as IDebugStackFrameSnifferEx32;
				}
			}
		}

		/// <summary>
		/// Destructs an instance of the 32-bit Active Script wrapper
		/// </summary>
		~ActiveScriptWrapper32()
		{
			Dispose(false);
		}


		#region ActiveScriptWrapperBase overrides

		protected override uint InnerEnumCodeContextsOfPosition(UIntPtr sourceContext, uint offset,
			uint length, out IEnumDebugCodeContexts enumContexts)
		{
			uint result = _activeScriptDebug32.EnumCodeContextsOfPosition(sourceContext.ToUInt32(),
				offset, length, out enumContexts);

			return result;
		}

		#region IActiveScriptWrapper implementation

		public override void InitNew()
		{
			_activeScriptParse32.InitNew();
		}

		public override object ParseScriptText(string code, string itemName, object context,
			string delimiter, UIntPtr sourceContextCookie, uint startingLineNumber, ScriptTextFlags flags)
		{
			object result;
			EXCEPINFO exceptionInfo;

			_activeScriptParse32.ParseScriptText(
				code,
				itemName,
				context,
				delimiter,
				sourceContextCookie,
				startingLineNumber,
				flags,
				out result,
				out exceptionInfo);

			return result;
		}

		public override void EnumStackFrames(out IEnumDebugStackFrames enumFrames)
		{
			if (_debugStackFrameSniffer32 != null)
			{
				_debugStackFrameSniffer32.EnumStackFrames(out enumFrames);
			}
			else
			{
				enumFrames = new NullEnumDebugStackFrames();
			}
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
		protected override void Dispose(bool disposing)
		{
			if (_disposedFlag.Set())
			{
				if (disposing)
				{
					_debugStackFrameSniffer32 = null;
					_activeScriptDebug32 = null;
					_activeScriptParse32 = null;
				}

				ComHelpers.ReleaseAndEmpty(ref _pDebugStackFrameSniffer32);
				ComHelpers.ReleaseAndEmpty(ref _pActiveScriptDebug32);
				ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse32);

				base.Dispose(disposing);
			}
		}

		#endregion

		#endregion
	}
}
#endif
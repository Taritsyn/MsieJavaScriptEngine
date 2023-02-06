#if NETFRAMEWORK
using System;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// 64-bit Active Script wrapper
	/// </summary>
	internal sealed class ActiveScriptWrapper64 : ActiveScriptWrapperBase
	{
		/// <summary>
		/// Pointer to an instance of 64-bit Active Script parser
		/// </summary>
		private IntPtr _pActiveScriptParse64;

		/// <summary>
		/// Pointer to an instance of 64-bit Active Script debugger
		/// </summary>
		private IntPtr _pActiveScriptDebug64;

		/// <summary>
		/// Pointer to an instance of 64-bit debug stack frame sniffer
		/// </summary>
		private IntPtr _pDebugStackFrameSniffer64;

		/// <summary>
		/// Instance of 64-bit Active Script parser
		/// </summary>
		private IActiveScriptParse64 _activeScriptParse64;

		/// <summary>
		/// Instance of 64-bit Active Script debugger
		/// </summary>
		private IActiveScriptDebug64 _activeScriptDebug64;

		/// <summary>
		/// Instance of 64-bit debug stack frame sniffer
		/// </summary>
		private IDebugStackFrameSnifferEx64 _debugStackFrameSniffer64;


		/// <summary>
		/// Constructs an instance of the 64-bit Active Script wrapper
		/// </summary>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ActiveScriptWrapper64(string clsid, ScriptLanguageVersion languageVersion, bool enableDebugging)
			: base(clsid, languageVersion, enableDebugging)
		{
			_pActiveScriptParse64 = ComHelpers.QueryInterface<IActiveScriptParse64>(_pActiveScript);
			_activeScriptParse64 = (IActiveScriptParse64)_activeScript;

			if (_enableDebugging)
			{
				_pActiveScriptDebug64 = ComHelpers.QueryInterface<IActiveScriptDebug64>(_pActiveScript);
				_activeScriptDebug64 = (IActiveScriptDebug64)_activeScript;

				_pDebugStackFrameSniffer64 = ComHelpers.QueryInterfaceNoThrow<IDebugStackFrameSnifferEx64>(
					_pActiveScript);
				_debugStackFrameSniffer64 = _activeScript as IDebugStackFrameSnifferEx64;
			}
		}

		/// <summary>
		/// Destructs an instance of the 64-bit Active Script wrapper
		/// </summary>
		~ActiveScriptWrapper64()
		{
			Dispose(false);
		}


		#region ActiveScriptWrapperBase overrides

		protected override uint InnerEnumCodeContextsOfPosition(UIntPtr sourceContext, uint offset,
			uint length, out IEnumDebugCodeContexts enumContexts)
		{
			uint result = _activeScriptDebug64.EnumCodeContextsOfPosition(sourceContext.ToUInt64(),
				offset, length, out enumContexts);

			return result;
		}

		#region IActiveScriptWrapper implementation

		public override void InitNew()
		{
			_activeScriptParse64.InitNew();
		}

		public override object ParseScriptText(string code, string itemName, object context,
			string delimiter, UIntPtr sourceContextCookie, uint startingLineNumber, ScriptTextFlags flags)
		{
			object result;
			EXCEPINFO exceptionInfo;

			_activeScriptParse64.ParseScriptText(
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
			if (_debugStackFrameSniffer64 != null)
			{
				_debugStackFrameSniffer64.EnumStackFrames(out enumFrames);
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
		/// <param name="disposing">Flag, allowing destruction of managed objects contained in fields of class</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_disposedFlag.Set())
				{
					_debugStackFrameSniffer64 = null;
					_activeScriptDebug64 = null;
					_activeScriptParse64 = null;

					DisposeUnmanagedResources();
					base.Dispose(true);
				}
			}
			else
			{
				DisposeUnmanagedResources();
				base.Dispose(false);
			}
		}

		private void DisposeUnmanagedResources()
		{
			ComHelpers.ReleaseAndEmpty(ref _pDebugStackFrameSniffer64);
			ComHelpers.ReleaseAndEmpty(ref _pActiveScriptDebug64);
			ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse64);
		}

		#endregion

		#endregion
	}
}
#endif
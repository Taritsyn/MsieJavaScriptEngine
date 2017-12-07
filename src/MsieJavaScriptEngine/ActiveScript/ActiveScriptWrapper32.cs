#if !NETSTANDARD
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
		/// Instance of 32-bit debug stack frame sniffer
		/// </summary>
		private IDebugStackFrameSnifferEx32 _debugStackFrameSniffer32;


		/// <summary>
		/// Constructs an instance of the 32-bit Active Script wrapper
		/// </summary>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ActiveScriptWrapper32(string clsid, ScriptLanguageVersion languageVersion,
			bool enableDebugging)
			: base(clsid, languageVersion, enableDebugging)
		{
			_pActiveScriptParse32 = ComHelpers.QueryInterface<IActiveScriptParse32>(_pActiveScript);
			_activeScriptParse32 = (IActiveScriptParse32)_activeScript;

			if (_enableDebugging)
			{
				_pActiveScriptDebug32 = ComHelpers.QueryInterface<IActiveScriptDebug32>(_pActiveScript);
				_pDebugStackFrameSniffer32 = ComHelpers.QueryInterfaceNoThrow<IDebugStackFrameSnifferEx32>(
					_pActiveScript);
				if (_pDebugStackFrameSniffer32 != IntPtr.Zero)
				{
					_debugStackFrameSniffer32 = _activeScript as IDebugStackFrameSnifferEx32;
				}
			}
		}


		#region ActiveScriptWrapperBase overrides

		protected override uint InnerEnumCodeContextsOfPosition(UIntPtr sourceContext, uint offset,
			uint length, out IEnumDebugCodeContexts enumContexts)
		{
			var del = ComHelpers.GetMethodDelegate<RawEnumCodeContextsOfPosition32>(_pActiveScriptDebug32, 5);
			uint result = del(_pActiveScriptDebug32, sourceContext.ToUInt32(), offset, length,
				out enumContexts);

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

		public override void Dispose()
		{
			if (_disposedFlag.Set())
			{
				_debugStackFrameSniffer32 = null;
				_activeScriptParse32 = null;

				ComHelpers.ReleaseAndEmpty(ref _pDebugStackFrameSniffer32);
				ComHelpers.ReleaseAndEmpty(ref _pActiveScriptDebug32);
				ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse32);

				base.Dispose();
			}
		}

		#endregion

		#endregion
	}
}
#endif
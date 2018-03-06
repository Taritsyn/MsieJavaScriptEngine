#if !NETSTANDARD
using System;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// 64-bit Active Script wrapper
	/// </summary>
	internal sealed class ActiveScriptWrapper64 : ActiveScriptWrapperBase
	{
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
		/// <param name="engineModeName">Name of JS engine mode</param>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ActiveScriptWrapper64(string engineModeName, string clsid, ScriptLanguageVersion languageVersion,
			bool enableDebugging)
			: base(engineModeName, clsid, languageVersion, enableDebugging)
		{
			_activeScriptParse64 = (IActiveScriptParse64)_activeScript;
			if (_enableDebugging)
			{
				_activeScriptDebug64 = (IActiveScriptDebug64)_activeScript;
				_debugStackFrameSniffer64 = _activeScript as IDebugStackFrameSnifferEx64;
			}
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

		public override void Dispose()
		{
			if (_disposedFlag.Set())
			{
				_debugStackFrameSniffer64 = null;
				_activeScriptDebug64 = null;
				_activeScriptParse64 = null;

				base.Dispose();
			}
		}

		#endregion

		#endregion
	}
}
#endif
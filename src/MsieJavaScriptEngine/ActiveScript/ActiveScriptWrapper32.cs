#if !NETSTANDARD
using System;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// 32-bit Active Script wrapper
	/// </summary>
	internal sealed class ActiveScriptWrapper32 : ActiveScriptWrapperBase
	{
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
		/// <param name="engineModeName">Name of JS engine mode</param>
		/// <param name="clsid">CLSID of JS engine</param>
		/// <param name="languageVersion">Version of script language</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		public ActiveScriptWrapper32(string engineModeName, string clsid, ScriptLanguageVersion languageVersion,
			bool enableDebugging)
			: base(engineModeName, clsid, languageVersion, enableDebugging)
		{
			_activeScriptParse32 = (IActiveScriptParse32)_activeScript;
			if (_enableDebugging)
			{
				_activeScriptDebug32 = (IActiveScriptDebug32)_activeScript;
				_debugStackFrameSniffer32 = _activeScript as IDebugStackFrameSnifferEx32;
			}
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

		public override void Dispose()
		{
			if (_disposedFlag.Set())
			{
				_debugStackFrameSniffer32 = null;
				_activeScriptDebug32 = null;
				_activeScriptParse32 = null;

				base.Dispose();
			}
		}

		#endregion

		#endregion
	}
}
#endif
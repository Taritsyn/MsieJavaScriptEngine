#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	internal sealed partial class ClassicActiveScriptJsEngine
	{
		/// <summary>
		/// Classic Active Script site
		/// </summary>
		private sealed class ScriptSite : ScriptSiteBase
		{
			/// <summary>
			/// Constructs an instance of the Classic Active Script site
			/// </summary>
			/// <param name="jsEngine">Instance of the Active Script JS engine</param>
			public ScriptSite(ActiveScriptJsEngineBase jsEngine)
				: base(jsEngine)
			{ }


			#region ScriptSiteBase overrides

			/// <summary>
			/// Gets a flag that indicates if the script interruption is requested
			/// </summary>
			public override bool InterruptRequested
			{
				get { return false; }
			}


			/// <summary>
			/// Processes a Active Script error
			/// </summary>
			/// <param name="error">Instance of <see cref="IActiveScriptError"/></param>
			protected override void ProcessActiveScriptError(IActiveScriptError error)
			{
				var activeScriptException = CreateActiveScriptException(error);
				if (activeScriptException.ErrorCode == ComErrorCode.E_ABORT)
				{
					// Script execution was interrupted explicitly. At this point the script
					// engine might be in an odd state; the following call seems to get it back
					// to normal.
					ActiveScriptWrapper.SetScriptState(ScriptState.Started);
				}

				LastException = activeScriptException;
			}

			/// <summary>
			/// Gets a array of <see cref="CallStackItem"/> instances
			/// </summary>
			/// <returns>An array of <see cref="CallStackItem"/> instances</returns>
			protected override CallStackItem[] GetCallStackItems()
			{
				var callStackItems = new List<CallStackItem>();
				IEnumDebugStackFrames enumFrames;
				ActiveScriptWrapper.EnumStackFrames(out enumFrames);

				while (true)
				{
					DebugStackFrameDescriptor descriptor;
					uint countFetched;
					enumFrames.Next(1, out descriptor, out countFetched);
					if (countFetched < 1)
					{
						break;
					}

					try
					{
						IDebugStackFrame stackFrame = descriptor.Frame;

						string functionName;
						stackFrame.GetDescriptionString(true, out functionName);

						string shortFunctionName = ActiveScriptJsErrorHelpers.ShortenErrorItemName(
							functionName, "JScript ");

						IDebugCodeContext codeContext;
						stackFrame.GetCodeContext(out codeContext);

						IDebugDocumentContext documentContext;
						codeContext.GetDocumentContext(out documentContext);

						string documentName = string.Empty;
						uint lineNumber = 0;
						uint columnNumber = 0;

						if (documentContext is not null)
						{
							IDebugDocument document;
							documentContext.GetDocument(out document);

							document.GetName(DocumentNameType.Title, out documentName);

							var documentText = (IDebugDocumentText)document;

							uint position;
							uint length;
							documentText.GetPositionOfContext(documentContext, out position, out length);

							uint offsetInLine;
							documentText.GetLineOfPosition(position, out lineNumber, out offsetInLine);
							columnNumber = offsetInLine + 1;
						}

						CallStackItem callStackItem = new CallStackItem
						{
							FunctionName = shortFunctionName,
							DocumentName = documentName,
							LineNumber = (int)lineNumber,
							ColumnNumber = (int)columnNumber
						};
						callStackItems.Add(callStackItem);
					}
					finally
					{
						if (descriptor.pFinalObject != IntPtr.Zero)
						{
							Marshal.Release(descriptor.pFinalObject);
						}
					}
				}

				return callStackItems.ToArray();
			}

			#endregion
		}
	}
}
#endif
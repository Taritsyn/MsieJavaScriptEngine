#if !NETSTANDARD
using System;
using System.Runtime.InteropServices;
using System.Text;

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
			/// Writes a string representation of the script call stack to the buffer.
			/// A return value indicates whether the writing succeeded.
			/// </summary>
			/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
			/// <returns>true if the writing was successful; otherwise, false</returns>
			protected override bool TryWriteStackTrace(StringBuilder buffer)
			{
				bool result = false;

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

						string description;
						stackFrame.GetDescriptionString(true, out description);

						if (string.Equals(description, "JScript global code", StringComparison.Ordinal))
						{
							description = "Global code";
						}

						IDebugCodeContext codeContext;
						stackFrame.GetCodeContext(out codeContext);

						IDebugDocumentContext documentContext;
						codeContext.GetDocumentContext(out documentContext);

						if (documentContext == null)
						{
							JsErrorHelpers.WriteErrorLocation(buffer, description);
							buffer.AppendLine();
						}
						else
						{
							IDebugDocument document;
							documentContext.GetDocument(out document);

							string documentName;
							document.GetName(DocumentNameType.Title, out documentName);

							var documentText = (IDebugDocumentText)document;

							uint position;
							uint length;
							documentText.GetPositionOfContext(documentContext, out position, out length);

							uint lineNumber;
							uint offsetInLine;
							documentText.GetLineOfPosition(position, out lineNumber, out offsetInLine);
							uint columnNumber = offsetInLine + 1;

							buffer.AppendFormatLine("   at {0} ({1}:{2}:{3})", description, documentName,
								lineNumber, columnNumber);
						}

						result = true;
					}
					finally
					{
						if (descriptor.pFinalObject != IntPtr.Zero)
						{
							Marshal.Release(descriptor.pFinalObject);
						}
					}
				}

				if (result)
				{
					buffer.TrimEnd();
				}

				return result;
			}

			#endregion
		}
	}
}
#endif
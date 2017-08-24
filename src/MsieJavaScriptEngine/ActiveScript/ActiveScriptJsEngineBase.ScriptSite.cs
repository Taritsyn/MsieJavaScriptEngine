#if !NETSTANDARD1_3
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

using HResultHelpers = MsieJavaScriptEngine.Helpers.ComHelpers.HResult;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script site
	/// </summary>
	internal abstract partial class ActiveScriptJsEngineBase
	{
		private sealed class ScriptSite : IActiveScriptSite, IActiveScriptSiteDebug32, IActiveScriptSiteDebug64,
			IActiveScriptSiteDebugEx, ICustomQueryInterface
		{
			/// <summary>
			/// Instance of Active Script engine
			/// </summary>
			private readonly ActiveScriptJsEngineBase _jsEngine;


			/// <summary>
			/// Constructs an instance of the Active Script site
			/// </summary>
			/// <param name="jsEngine">Active Script engine</param>
			public ScriptSite(ActiveScriptJsEngineBase jsEngine)
			{
				_jsEngine = jsEngine;
			}


			/// <summary>
			/// Creates a instance of <see cref="ActiveScriptException"/>
			/// </summary>
			/// <param name="error">Instance of <see cref="IActiveScriptError"/></param>
			/// <returns>Instance of <see cref="ActiveScriptException"/></returns>
			private ActiveScriptException CreateActiveScriptException(IActiveScriptError error)
			{
				EXCEPINFO exceptionInfo;
				error.GetExceptionInfo(out exceptionInfo);

				int hResult = exceptionInfo.scode;
				int errorCode = HResultHelpers.GetCode(hResult);
				bool isSyntaxError = IsSyntaxError(hResult);
				string category = exceptionInfo.bstrSource;
				string description = exceptionInfo.bstrDescription;
				string helpLink = string.Empty;
				if (!string.IsNullOrWhiteSpace(exceptionInfo.bstrHelpFile))
				{
					helpLink = exceptionInfo.dwHelpContext != 0 ?
						string.Format("{0}: {1}", exceptionInfo.bstrHelpFile, exceptionInfo.dwHelpContext)
						:
						exceptionInfo.bstrHelpFile
						;
				}

				uint sourceContext;
				uint lineNumber;
				int columnNumber;

				error.GetSourcePosition(out sourceContext, out lineNumber, out columnNumber);
				++lineNumber;
				++columnNumber;

				string sourceFragment = string.Empty;
				if (isSyntaxError)
				{
					error.GetSourceLineText(out sourceFragment);
				}

				string message = GetErrorDetails(category, description, isSyntaxError, sourceContext,
					lineNumber, columnNumber);

				var activeScriptException = new ActiveScriptException(message)
				{
					ErrorCode = errorCode,
					Category = category,
					Description = description,
					SourceContext = sourceContext,
					LineNumber = lineNumber,
					ColumnNumber = columnNumber,
					SourceFragment = sourceFragment,
					HelpLink = helpLink
				};

				return activeScriptException;
			}

			/// <summary>
			/// Checks whether the specified HRESULT value is syntax error
			/// </summary>
			/// <param name="hResult">The HRESULT value</param>
			/// <returns>Result of check (true - is syntax error; false - is not syntax error)</returns>
			private static bool IsSyntaxError(int hResult)
			{
				bool isSyntaxError = false;

				if (HResultHelpers.GetFacility(hResult) == HResultHelpers.FACILITY_CONTROL)
				{
					int errorCode = HResultHelpers.GetCode(hResult);
					isSyntaxError = errorCode >= 1002 && errorCode <= 1035;
				}

				return isSyntaxError;
			}

			/// <summary>
			/// Gets a error details
			/// </summary>
			/// <param name="category">Category of error</param>
			/// <param name="description">Description of error</param>
			/// <param name="isSyntaxError">Value indicating whether this exception is syntax error</param>
			/// <param name="sourceContext">Application specific source context</param>
			/// <param name="lineNumber">Line number</param>
			/// <param name="columnNumber">Column number</param>
			/// <returns>Error details</returns>
			private string GetErrorDetails(string category, string description, bool isSyntaxError,
				uint sourceContext, uint lineNumber, int columnNumber)
			{
				var errorBuilder = new StringBuilder();
				errorBuilder.AppendFormatLine("{0}: {1}",
					_jsEngine.ShortenErrorCategoryName(category), description);

				if (_jsEngine._processDebugManagerWrapper != null)
				{
					bool stackTraceNotEmpty = false;
					if (!isSyntaxError)
					{
						stackTraceNotEmpty = TryWriteStackTrace(errorBuilder);
					}

					bool fullErrorLocationNotEmpty = false;
					if (!stackTraceNotEmpty)
					{
						fullErrorLocationNotEmpty = TryWriteFullErrorLocation(errorBuilder, sourceContext,
							lineNumber, columnNumber);
					}

					if (!stackTraceNotEmpty && !fullErrorLocationNotEmpty)
					{
						TryWriteErrorLocation(errorBuilder, lineNumber, columnNumber);
					}
				}
				else
				{
					TryWriteErrorLocation(errorBuilder, lineNumber, columnNumber);
				}

				string errorDetails = errorBuilder.TrimEnd().ToString();
				errorBuilder.Clear();

				return errorDetails;
			}

			/// <summary>
			/// Writes a information about error location to the buffer.
			/// A return value indicates whether the writing succeeded.
			/// </summary>
			/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
			/// <param name="lineNumber">Line number</param>
			/// <param name="columnNumber">Column number</param>
			/// <returns>true if the writing was successful; otherwise, false</returns>
			private bool TryWriteErrorLocation(StringBuilder buffer, uint lineNumber, int columnNumber)
			{
				bool result = false;

				if (lineNumber > 0)
				{
					JsErrorHelpers.WriteErrorLocation(buffer, (int)lineNumber, columnNumber);
					result = true;
				}

				return result;
			}

			/// <summary>
			/// Writes a information about full error location to the buffer.
			/// A return value indicates whether the writing succeeded.
			/// </summary>
			/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
			/// <param name="sourceContext">Application specific source context</param>
			/// <param name="lineNumber">Line number</param>
			/// <param name="columnNumber">Column number</param>
			/// <returns>true if the writing was successful; otherwise, false</returns>
			private bool TryWriteFullErrorLocation(StringBuilder buffer, uint sourceContext, uint lineNumber,
				int columnNumber)
			{
				bool result = false;
				DebugDocument document;

				if (_jsEngine._debugDocuments.TryGetValue(new UIntPtr(sourceContext), out document))
				{
					string documentName;
					document.GetName(DocumentNameType.Title, out documentName);

					JsErrorHelpers.WriteErrorLocation(buffer, documentName, (int)lineNumber, columnNumber);
					result = true;
				}

				return result;
			}

			/// <summary>
			/// Writes a string representation of the script call stack to the buffer.
			/// A return value indicates whether the writing succeeded.
			/// </summary>
			/// <param name="buffer">Instance of <see cref="StringBuilder"/></param>
			/// <returns>true if the writing was successful; otherwise, false</returns>
			private bool TryWriteStackTrace(StringBuilder buffer)
			{
				bool result = false;

				IEnumDebugStackFrames enumFrames;
				_jsEngine._activeScriptWrapper.EnumStackFrames(out enumFrames);

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

			#region IActiveScriptSite implementation

			public void GetLcid(out int lcid)
			{
				lcid = CultureInfo.CurrentCulture.LCID;
			}

			public void GetItemInfo(string name, ScriptInfoFlags mask, ref IntPtr pUnkItem, ref IntPtr pTypeInfo)
			{
				object item = _jsEngine._hostItems[name];
				if (item == null)
				{
					throw new COMException(
						string.Format(NetFrameworkStrings.Runtime_ItemNotFound, name), ComErrorCode.ElementNotFound);
				}

				if (mask.HasFlag(ScriptInfoFlags.IUnknown))
				{
					pUnkItem = Marshal.GetIDispatchForObject(item);
				}

				if (mask.HasFlag(ScriptInfoFlags.ITypeInfo))
				{
					pTypeInfo = Marshal.GetITypeInfoForType(item.GetType());
				}
			}

			public void GetDocVersionString(out string version)
			{
				throw new NotImplementedException();
			}

			public void OnScriptTerminate(object result, EXCEPINFO exceptionInfo)
			{ }

			public void OnStateChange(ScriptState state)
			{ }

			public void OnScriptError(IActiveScriptError error)
			{
				_jsEngine._lastException = CreateActiveScriptException(error);
			}

			public void OnEnterScript()
			{ }

			public void OnLeaveScript()
			{ }

			#endregion

			#region IActiveScriptSiteDebug32 and IActiveScriptSiteDebug64 implementation

			public void GetRootApplicationNode(out IDebugApplicationNode node)
			{
				_jsEngine._debugApplicationWrapper.GetRootNode(out node);
			}

			public void OnScriptErrorDebug(IActiveScriptErrorDebug errorDebug, out bool enterDebugger,
				out bool callOnScriptErrorWhenContinuing)
			{
				var error = errorDebug as IActiveScriptError;
				if (error != null)
				{
					_jsEngine._lastException = CreateActiveScriptException(error);
				}

				enterDebugger = true;
				callOnScriptErrorWhenContinuing = true;
			}

			#region IActiveScriptSiteDebug32 implementation

			public void GetApplication(out IDebugApplication32 application)
			{
				application = _jsEngine._debugApplicationWrapper.DebugApplication32;
			}

			public void GetDocumentContextFromPosition(uint sourceContext, uint offset, uint length,
				out IDebugDocumentContext documentContext)
			{
				documentContext = null;
				DebugDocument document;

				if (_jsEngine._debugDocuments.TryGetValue(new UIntPtr(sourceContext), out document))
				{
					document.GetContextOfPosition(offset, length, out documentContext);
				}
			}

			#endregion

			#region IActiveScriptSiteDebug64 implementation

			public void GetApplication(out IDebugApplication64 application)
			{
				application = _jsEngine._debugApplicationWrapper.DebugApplication64;
			}

			public void GetDocumentContextFromPosition(ulong sourceContext, uint offset, uint length,
				out IDebugDocumentContext documentContext)
			{
				documentContext = null;
				DebugDocument document;

				if (_jsEngine._debugDocuments.TryGetValue(new UIntPtr(sourceContext), out document))
				{
					document.GetContextOfPosition(offset, length, out documentContext);
				}
			}

			#endregion

			#endregion

			#region IActiveScriptSiteDebugEx implementation

			public void OnCanNotJitScriptErrorDebug(IActiveScriptErrorDebug errorDebug,
				out bool callOnScriptErrorWhenContinuing)
			{
				bool enterDebugger;

				OnScriptErrorDebug(errorDebug, out enterDebugger, out callOnScriptErrorWhenContinuing);
			}

			#endregion

			#region ICustomQueryInterface implementation

			public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr pInterface)
			{
				pInterface = IntPtr.Zero;

				if (iid == typeof(IActiveScriptSiteDebug32).GUID
					|| iid == typeof(IActiveScriptSiteDebug64).GUID
					|| iid == typeof(IActiveScriptSiteDebugEx).GUID)
				{
					return _jsEngine._processDebugManagerWrapper != null ?
						CustomQueryInterfaceResult.NotHandled : CustomQueryInterfaceResult.Failed;
				}

				return CustomQueryInterfaceResult.NotHandled;
			}

			#endregion
		}
	}
}
#endif
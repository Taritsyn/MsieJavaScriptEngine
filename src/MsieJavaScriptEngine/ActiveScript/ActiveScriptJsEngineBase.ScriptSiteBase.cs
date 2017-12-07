#if !NETSTANDARD
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

namespace MsieJavaScriptEngine.ActiveScript
{
	internal abstract partial class ActiveScriptJsEngineBase
	{
		/// <summary>
		/// Active Script site
		/// </summary>
		protected abstract class ScriptSiteBase : IActiveScriptSite,
			IActiveScriptSiteDebug32, IActiveScriptSiteDebug64,
			IActiveScriptSiteDebugEx, ICustomQueryInterface
		{
			/// <summary>
			/// Instance of the Active Script JS engine
			/// </summary>
			private readonly ActiveScriptJsEngineBase _jsEngine;

			/// <summary>
			/// Gets or sets a last Active Script exception
			/// </summary>
			public ActiveScriptException LastException
			{
				get { return _jsEngine._lastException; }
				set { _jsEngine._lastException = value; }
			}

			/// <summary>
			/// Gets a instance of Active Script wrapper
			/// </summary>
			public IActiveScriptWrapper ActiveScriptWrapper
			{
				get { return _jsEngine._activeScriptWrapper; }
			}

			/// <summary>
			/// Gets a flag that indicates if the script interruption is requested
			/// </summary>
			public virtual bool InterruptRequested
			{
				get { return _jsEngine._interruptRequested; }
			}


			/// <summary>
			/// Constructs an instance of the Active Script site
			/// </summary>
			/// <param name="jsEngine">Instance of the Active Script JS engine</param>
			protected ScriptSiteBase(ActiveScriptJsEngineBase jsEngine)
			{
				_jsEngine = jsEngine;
			}


			/// <summary>
			/// Processes a Active Script error
			/// </summary>
			/// <param name="error">Instance of <see cref="IActiveScriptError"/></param>
			protected virtual void ProcessActiveScriptError(IActiveScriptError error)
			{
				var activeScriptException = CreateActiveScriptException(error);
				LastException = activeScriptException;
			}

			/// <summary>
			/// Creates a instance of <see cref="ActiveScriptException"/>
			/// </summary>
			/// <param name="error">Instance of <see cref="IActiveScriptError"/></param>
			/// <returns>Instance of <see cref="ActiveScriptException"/></returns>
			protected ActiveScriptException CreateActiveScriptException(IActiveScriptError error)
			{
				EXCEPINFO exceptionInfo;
				error.GetExceptionInfo(out exceptionInfo);

				int hResult = exceptionInfo.scode;
				bool isSyntaxError = IsSyntaxError(hResult);
				string category;
				string description;
				if (hResult == ComErrorCode.E_ABORT)
				{
					category = string.Empty;
					description = CommonStrings.Runtime_ScriptInterrupted;
				}
				else
				{
					category = _jsEngine.ShortenErrorCategoryName(exceptionInfo.bstrSource);
					description = exceptionInfo.bstrDescription;
				}
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
					ErrorCode = hResult,
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

				if (ComHelpers.HResult.GetFacility(hResult) == ComErrorCode.FACILITY_CONTROL)
				{
					int errorNumber = ComHelpers.HResult.GetCode(hResult);
					isSyntaxError = errorNumber >= (int)JsErrorNumber.SyntaxError
						&& errorNumber <= (int)JsErrorNumber.ThrowMustBeFollowedByExpressionOnSameSourceLine;
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
				if (!string.IsNullOrWhiteSpace(category))
				{
					errorBuilder.AppendFormat("{0}: ", category);
				}
				errorBuilder.AppendLine(description);

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
			protected virtual bool TryWriteStackTrace(StringBuilder buffer)
			{
				return false;
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
						string.Format(NetFrameworkStrings.Runtime_ItemNotFound, name), ComErrorCode.E_ELEMENT_NOT_FOUND);
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
				ProcessActiveScriptError(error);
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
					ProcessActiveScriptError(error);
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
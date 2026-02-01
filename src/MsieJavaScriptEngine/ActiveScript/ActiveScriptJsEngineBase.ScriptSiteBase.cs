#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;

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
				string message = string.Empty;
				string category = string.Empty;
				string description = string.Empty;
				string type = string.Empty;
				string helpLink = string.Empty;
				string documentName = string.Empty;
				uint lineNumber = 0;
				int columnNumber = 0;
				string callStack = string.Empty;
				string sourceFragment = string.Empty;

				if (hResult == ComErrorCode.E_ABORT)
				{
					category = JsErrorCategory.Interrupted;
					description = CommonStrings.Runtime_ScriptInterrupted;
					message = description;
				}
				else
				{
					int errorNumber = ComHelpers.HResult.GetFacility(hResult) == ComErrorCode.FACILITY_CONTROL ?
						ComHelpers.HResult.GetCode(hResult) : 0;
					category = _jsEngine.ShortenErrorCategoryName(exceptionInfo.bstrSource);
					description = exceptionInfo.bstrDescription;
					type = _jsEngine.GetErrorTypeByNumber(errorNumber);

					if (!string.IsNullOrWhiteSpace(exceptionInfo.bstrHelpFile))
					{
						helpLink = exceptionInfo.dwHelpContext != 0 ?
							string.Format("{0}: {1}", exceptionInfo.bstrHelpFile, exceptionInfo.dwHelpContext)
							:
							exceptionInfo.bstrHelpFile
							;
					}

					uint sourceContext = 0;
					error.GetSourcePosition(out sourceContext, out lineNumber, out columnNumber);
					++lineNumber;
					++columnNumber;

					documentName = GetDocumentName(sourceContext);

					if (ActiveScriptJsErrorHelpers.IsCompilationError(errorNumber))
					{
						string sourceLine;
						error.GetSourceLineText(out sourceLine);

						sourceFragment = TextHelpers.GetTextFragmentFromLine(sourceLine, columnNumber);
					}
					else
					{
						callStack = JsErrorHelpers.StringifyCallStackItems(GetCallStackItems());
					}

					message = JsErrorHelpers.GenerateScriptErrorMessage(type, description, documentName,
						(int)lineNumber, columnNumber, sourceFragment, callStack);
				}

				var activeScriptException = new ActiveScriptException(message)
				{
					ErrorCode = hResult,
					Type = type,
					Category = category,
					Description = description,
					DocumentName = documentName,
					LineNumber = lineNumber,
					ColumnNumber = columnNumber,
					CallStack = callStack,
					SourceFragment = sourceFragment,
					HelpLink = helpLink
				};

				return activeScriptException;
			}

			/// <summary>
			/// Gets a document name
			/// </summary>
			/// <param name="sourceContext">Application specific source context</param>
			/// <returns>Document name</returns>
			private string GetDocumentName(uint sourceContext)
			{
				string documentName = string.Empty;
				var documentKey = new UIntPtr(sourceContext);
				DebugDocument document;

				if (_jsEngine._debugDocuments.TryGetValue(documentKey, out document))
				{
					document.GetName(DocumentNameType.Title, out documentName);
				}
				else if (!_jsEngine._documentNames.TryGetValue(documentKey, out documentName))
				{
					documentName = string.Empty;
				}

				return documentName;
			}

			/// <summary>
			/// Gets a array of <see cref="CallStackItem"/> instances
			/// </summary>
			/// <returns>An array of <see cref="CallStackItem"/> instances</returns>
			protected virtual CallStackItem[] GetCallStackItems()
			{
				return new CallStackItem[0];
			}

			#region IActiveScriptSite implementation

			public void GetLcid(out int lcid)
			{
				lcid = CultureInfo.CurrentCulture.LCID;
			}

			public void GetItemInfo(string name, ScriptInfoFlags mask, ref IntPtr pUnkItem, ref IntPtr pTypeInfo)
			{
				object item = _jsEngine._hostItems[name];
				if (item is null)
				{
					throw new COMException(
						string.Format(NetFrameworkStrings.Runtime_ItemNotFound, name),
						ComErrorCode.E_ELEMENT_NOT_FOUND
					);
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
				if (error is not null)
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
					return _jsEngine._processDebugManagerWrapper is not null ?
						CustomQueryInterfaceResult.NotHandled : CustomQueryInterfaceResult.Failed;
				}

				return CustomQueryInterfaceResult.NotHandled;
			}

			#endregion
		}
	}
}
#endif
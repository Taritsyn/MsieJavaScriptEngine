#if !NETSTANDARD1_3
using System;
using System.Globalization;
using System.Runtime.InteropServices;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

using MsieJavaScriptEngine.ActiveScript.Debugging;
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Resources;

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
				_jsEngine._lastException = ActiveScriptException.Create(error);
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
				if (errorDebug != null)
				{
					_jsEngine._lastException = ActiveScriptException.Create((IActiveScriptError)errorDebug);
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
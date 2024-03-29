﻿#if NETFRAMEWORK
using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;

namespace MsieJavaScriptEngine.ActiveScript
{
	internal sealed partial class ChakraActiveScriptJsEngine
	{
		/// <summary>
		/// Chakra Active Script site
		/// </summary>
		private sealed class ScriptSite : ScriptSiteBase, IActiveScriptSiteInterruptPoll
		{
			/// <summary>
			/// Constructs an instance of the Chakra Active Script site
			/// </summary>
			/// <param name="jsEngine">Instance of the Active Script JS engine</param>
			public ScriptSite(ChakraActiveScriptJsEngine jsEngine)
				: base(jsEngine)
			{ }


			#region IActiveScriptSiteInterruptPoll implementation

			public uint QueryContinue()
			{
				int hResult;

				if (InterruptRequested)
				{
					hResult = ComErrorCode.E_ABORT;
					string category = JsErrorCategory.Interrupted;
					string description = CommonStrings.Runtime_ScriptInterrupted;
					string message = description;

					var activeScriptException = new ActiveScriptException(message)
					{
						ErrorCode = hResult,
						Category = category,
						Description = description
					};

					LastException = activeScriptException;
				}
				else
				{
					hResult = ComErrorCode.S_OK;
				}

				return NumericHelpers.SignedAsUnsigned(hResult);
			}

			#endregion
		}
	}
}
#endif
using System;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// JsRT version of Chakra JS engine
	/// </summary>
	internal abstract class ChakraJsRtJsEngineBase : InnerJsEngineBase
	{
		/// <summary>
		/// JS source context
		/// </summary>
		protected JsSourceContext _jsSourceContext = JsSourceContext.FromIntPtr(IntPtr.Zero);

		/// <summary>
		/// Script dispatcher
		/// </summary>
		protected ScriptDispatcher _dispatcher;


		/// <summary>
		/// Constructs an instance of the Chakra JsRT engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		protected ChakraJsRtJsEngineBase(JsEngineSettings settings)
			: base(settings)
		{
#if NETSTANDARD1_3
			_dispatcher = new ScriptDispatcher();
#else
			_dispatcher = new ScriptDispatcher(settings.MaxStackSize);
#endif
		}
	}
}
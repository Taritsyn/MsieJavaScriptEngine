#if !NETSTANDARD1_3
using MsieJavaScriptEngine.Constants;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Chakra JS engine
	/// </summary>
	internal sealed partial class ChakraActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		/// <summary>
		/// Flag indicating whether this JS engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs an instance of the Chakra Active Script engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		public ChakraActiveScriptJsEngine(JsEngineSettings settings)
			: base(settings, ClassId.Chakra, ScriptLanguageVersion.EcmaScript5, "9", "JavaScript ")
		{ }


		/// <summary>
		/// Checks a support of the Chakra Active Script engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(ClassId.Chakra, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
		}

		#region ActiveScriptJsEngineBase overrides

		/// <summary>
		/// Creates a instance of the Active Script site
		/// </summary>
		/// <returns>Instance of the Active Script site</returns>
		protected override ScriptSiteBase CreateScriptSite()
		{
			return new ScriptSite(this);
		}

		/// <summary>
		/// Initializes a script context
		/// </summary>
		protected override void InitScriptContext()
		{
			_interruptRequested = false;
		}

		#region IInnerJsEngine implementation

		public override void Interrupt()
		{
			_interruptRequested = true;
		}

		#endregion

		#endregion
	}
}
#endif
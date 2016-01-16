namespace MsieJavaScriptEngine
{
	/// <summary>
	/// JavaScript engine settings
	/// </summary>
	public sealed class JsEngineSettings
	{
		/// <summary>
		/// Gets or sets a flag for whether to enable script debugging features
		/// (only works in the <code>ChakraIeJsRt</code> and <code>ChakraEdgeJsRt</code> modes)
		/// </summary>
		public bool EnableDebugging
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a JavaScript engine mode
		/// </summary>
		public JsEngineMode EngineMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to use the ECMAScript 5 Polyfill
		/// </summary>
		public bool UseEcmaScript5Polyfill
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to use the JSON2 library
		/// </summary>
		public bool UseJson2Library
		{
			get;
			set;
		}


		/// <summary>
		/// Constructs instance of JavaScript engine settings
		/// </summary>
		public JsEngineSettings()
		{
			EnableDebugging = false;
			EngineMode = JsEngineMode.Auto;
			UseEcmaScript5Polyfill = false;
			UseJson2Library = false;
		}
	}
}
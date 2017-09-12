namespace MsieJavaScriptEngine
{
	/// <summary>
	/// JS engine settings
	/// </summary>
	public sealed class JsEngineSettings
	{
		/// <summary>
		/// Gets or sets a flag for whether to enable script debugging features
		/// </summary>
		public bool EnableDebugging
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a JS engine mode
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
		/// Constructs an instance of JS engine settings
		/// </summary>
		public JsEngineSettings()
		{
			EnableDebugging = false;
			EngineMode = JsEngineMode.Auto;
			UseEcmaScript5Polyfill = false;
			UseJson2Library = false;
		}


		/// <summary>
		/// Creates a new object that is a copy of the current instance
		/// </summary>
		/// <returns>A new object that is a copy of this instance</returns>
		public JsEngineSettings Clone()
		{
			return (JsEngineSettings)MemberwiseClone();
		}
	}
}
namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Represents a pre-compiled script that can be executed by different instances of the MSIE JS engine
	/// </summary>
	public sealed class PrecompiledScript
	{
		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		public string EngineMode
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a source code of the script
		/// </summary>
		internal string Code
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a cached data for accelerated recompilation
		/// </summary>
		internal byte[] CachedBytes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a document name
		/// </summary>
		internal string DocumentName
		{
			get;
			private set;
		}


		/// <summary>
		/// Constructs an instance of pre-compiled script
		/// </summary>
		/// <param name="engineMode">Name of JS engine mode</param>
		/// <param name="code">The source code of the script</param>
		/// <param name="cachedBytes">Cached data for accelerated recompilation</param>
		/// <param name="documentName">Document name</param>
		internal PrecompiledScript(string engineMode, string code, byte[] cachedBytes, string documentName)
		{
			EngineMode = engineMode;
			Code = code;
			CachedBytes = cachedBytes;
			DocumentName = documentName;
		}
	}
}
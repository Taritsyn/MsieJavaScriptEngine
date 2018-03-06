namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Common regular expressions
	/// </summary>
	internal static class CommonRegExps
	{
		/// <summary>
		/// Pattern for working with JS names
		/// </summary>
		public const string JsNamePattern = @"[$_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}]" +
			@"[$_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\u200C\u200D\p{Mn}\p{Mc}\p{Nd}\p{Pc}]*";

		/// <summary>
		/// Pattern for working with document names
		/// </summary>
		public const string DocumentNamePattern = @"[^\s*?""<>|][^\t\n\r*?""<>|]*?";
	}
}
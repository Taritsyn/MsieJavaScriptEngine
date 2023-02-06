#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// JScript syntax error numbers
	/// </summary>
	internal static class JScriptSyntaxErrorNumber
	{
		#region Engine

		/// <summary>
		/// Out of memory
		/// </summary>
		public const int OutOfMemory = 1001;

		#endregion

		#region Compilation

		/// <summary>
		/// Syntax error
		/// </summary>
		public const int SyntaxError = 1002;

		/// <summary>
		/// Expected <c>:</c>
		/// </summary>
		public const int ExpectedColon = 1003;

		/// <summary>
		/// Expected <c>;</c>
		/// </summary>
		public const int ExpectedSemicolon = 1004;

		/// <summary>
		/// Expected <c>(</c>
		/// </summary>
		public const int ExpectedLeftParenthesis = 1005;

		/// <summary>
		/// Expected <c>)</c>
		/// </summary>
		public const int ExpectedRightParenthesis = 1006;

		/// <summary>
		/// Expected <c>]</c>
		/// </summary>
		public const int ExpectedRightSquareBracket = 1007;

		/// <summary>
		/// Expected <c>{</c>
		/// </summary>
		public const int ExpectedLeftCurlyBrace = 1008;

		/// <summary>
		/// Expected <c>}</c>
		/// </summary>
		public const int ExpectedRightCurlyBrace = 1009;

		/// <summary>
		/// Expected identifier
		/// </summary>
		public const int ExpectedIdentifier = 1010;

		/// <summary>
		/// Expected <c>=</c>
		/// </summary>
		public const int ExpectedEqualSign = 1011;

		/// <summary>
		/// Expected <c>/</c>
		/// </summary>
		public const int ExpectedForwardSlash = 1012;

		/// <summary>
		/// Invalid character
		/// </summary>
		public const int InvalidCharacter = 1014;

		/// <summary>
		/// Unterminated string constant
		/// </summary>
		public const int UnterminatedStringConstant = 1015;

		/// <summary>
		/// Unterminated comment
		/// </summary>
		public const int UnterminatedComment = 1016;

		/// <summary>
		/// <c>return</c> statement outside of function
		/// </summary>
		public const int ReturnStatementOutsideOfFunction = 1018;

		/// <summary>
		/// Can't have <c>break</c> outside of loop
		/// </summary>
		public const int CannotHaveBreakStatementOutsideOfLoop = 1019;

		/// <summary>
		/// Can't have <c>continue</c> outside of loop
		/// </summary>
		public const int CannotHaveContinueStatementOutsideOfLoop = 1020;

		/// <summary>
		/// Expected hexadecimal digit
		/// </summary>
		public const int ExpectedHexadecimalDigit = 1023;

		/// <summary>
		/// Expected <c>while</c>
		/// </summary>
		public const int ExpectedWhileStatement = 1024;

		/// <summary>
		/// Label redefined
		/// </summary>
		public const int LabelRedefined = 1025;

		/// <summary>
		/// Label not found
		/// </summary>
		public const int LabelNotFound = 1026;

		/// <summary>
		/// <c>default</c> can only appear once in a <c>switch</c> statement
		/// </summary>
		public const int DefaultStatementCanOnlyAppearOnceInSwitchStatement = 1027;

		/// <summary>
		/// Expected identifier, string or number
		/// </summary>
		public const int ExpectedIdentifierStringOrNumber = 1028;

		/// <summary>
		/// Expected <c>@end</c>
		/// </summary>
		public const int ExpectedConditionalCompilationEndStatement = 1029;

		/// <summary>
		/// Conditional compilation is turned off
		/// </summary>
		public const int ConditionalCompilationTurnedOff = 1030;

		/// <summary>
		/// Expected constant
		/// </summary>
		public const int ExpectedConstant = 1031;

		/// <summary>
		/// Expected <c>@</c>
		/// </summary>
		public const int ExpectedAtSign = 1032;

		/// <summary>
		/// Expected <c>catch</c>
		/// </summary>
		public const int ExpectedCatchStatement = 1033;

		/// <summary>
		/// Throw must be followed by an expression on the same source line
		/// </summary>
		public const int ThrowMustBeFollowedByExpressionOnSameSourceLine = 1035;

		#endregion
	}
}
#endif
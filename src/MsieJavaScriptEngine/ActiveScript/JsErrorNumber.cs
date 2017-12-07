#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// JS error numbers
	/// </summary>
	internal enum JsErrorNumber : uint
	{
		/// <summary>
		/// Success error number
		/// </summary>
		NoError = 0,

		#region Syntax

		/// <summary>
		/// Syntax error
		/// </summary>
		SyntaxError = 1002,

		/// <summary>
		/// Expected ':'
		/// </summary>
		ExpectedColon = 1003,

		/// <summary>
		/// Expected ';'
		/// </summary>
		ExpectedSemicolon = 1004,

		/// <summary>
		/// Expected '('
		/// </summary>
		ExpectedLeftParenthesis = 1005,

		/// <summary>
		/// Expected ')'
		/// </summary>
		ExpectedRightParenthesis = 1006,

		/// <summary>
		/// Expected ']'
		/// </summary>
		ExpectedRightSquareBracket = 1007,

		/// <summary>
		/// Expected '{'
		/// </summary>
		ExpectedLeftCurlyBrace = 1008,

		/// <summary>
		/// Expected '}'
		/// </summary>
		ExpectedRightCurlyBrace = 1009,

		/// <summary>
		/// Expected identifier
		/// </summary>
		ExpectedIdentifier = 1010,

		/// <summary>
		/// Expected '='
		/// </summary>
		ExpectedEqualSign = 1011,

		/// <summary>
		/// Expected '/'
		/// </summary>
		ExpectedForwardSlash = 1012,

		/// <summary>
		/// Invalid character
		/// </summary>
		InvalidCharacter = 1014,

		/// <summary>
		/// Unterminated string constant
		/// </summary>
		UnterminatedStringConstant = 1015,

		/// <summary>
		/// Unterminated comment
		/// </summary>
		UnterminatedComment = 1016,

		/// <summary>
		/// 'return' statement outside of function
		/// </summary>
		ReturnStatementOutsideOfFunction = 1018,

		/// <summary>
		/// Can't have 'break' outside of loop
		/// </summary>
		CannotHaveBreakStatementOutsideOfLoop = 1019,

		/// <summary>
		/// Can't have 'continue' outside of loop
		/// </summary>
		CannotHaveContinueStatementOutsideOfLoop = 1020,

		/// <summary>
		/// Expected hexadecimal digit
		/// </summary>
		ExpectedHexadecimalDigit = 1023,

		/// <summary>
		/// Expected 'while'
		/// </summary>
		ExpectedWhileStatement = 1024,

		/// <summary>
		/// Label redefined
		/// </summary>
		LabelRedefined = 1025,

		/// <summary>
		/// Label not found
		/// </summary>
		LabelNotFound = 1026,

		/// <summary>
		/// 'default' can only appear once in a 'switch' statement
		/// </summary>
		DefaultStatementCanOnlyAppearOnceInSwitchStatement = 1027,

		/// <summary>
		/// Expected identifier, string or number
		/// </summary>
		ExpectedIdentifierStringOrNumber = 1028,

		/// <summary>
		/// Expected '@end'
		/// </summary>
		ExpectedConditionalCompilationEndStatement = 1029,

		/// <summary>
		/// Conditional compilation is turned off
		/// </summary>
		ConditionalCompilationTurnedOff = 1030,

		/// <summary>
		/// Expected constant
		/// </summary>
		ExpectedConstant = 1031,

		/// <summary>
		/// Expected '@'
		/// </summary>
		ExpectedAtSign = 1032,

		/// <summary>
		/// Expected 'catch'
		/// </summary>
		ExpectedCatchStatement = 1033,

		/// <summary>
		/// Throw must be followed by an expression on the same source line
		/// </summary>
		ThrowMustBeFollowedByExpressionOnSameSourceLine = 1035,

		#endregion

		#region Runtime

		/// <summary>
		/// Access is denied
		/// </summary>
		AccessDenied = 5,

		/// <summary>
		/// Object doesn't support this property or method
		/// </summary>
		ObjectDoesNotSupportThisPropertyOrMethod = 438,

		/// <summary>
		/// Out of memory
		/// </summary>
		OutOfMemory = 1001,

		/// <summary>
		/// Cannot assign to 'this'
		/// </summary>
		CannotAssignToThisKeyword = 5000,

		/// <summary>
		/// Number expected
		/// </summary>
		NumberExpected = 5001,

		/// <summary>
		/// Function expected
		/// </summary>
		FunctionExpected = 5002,

		/// <summary>
		/// Cannot assign to a function result
		/// </summary>
		CannotAssignToFunctionResult = 5003,

		/// <summary>
		/// String expected
		/// </summary>
		StringExpected = 5005,

		/// <summary>
		/// Date object expected
		/// </summary>
		DateObjectExpected = 5006,

		/// <summary>
		/// Object expected
		/// </summary>
		ObjectExpected = 5007,

		/// <summary>
		/// Illegal assignment
		/// </summary>
		IllegalAssignment = 5008,

		/// <summary>
		/// Undefined identifier
		/// </summary>
		UndefinedIdentifier = 5009,

		/// <summary>
		/// Boolean expected
		/// </summary>
		BooleanExpected = 5010,

		/// <summary>
		/// Object member expected
		/// </summary>
		ObjectMemberExpected = 5012,

		/// <summary>
		/// VBArray expected
		/// </summary>
		VbArrayExpected = 5013,

		/// <summary>
		/// JavaScript object expected
		/// </summary>
		JavaScriptObjectExpected = 5014,

		/// <summary>
		/// Enumerator object expected
		/// </summary>
		EnumeratorObjectExpected = 5015,

		/// <summary>
		/// Regular Expression object expected
		/// </summary>
		RegularExpressionObjectExpected = 5016,

		/// <summary>
		/// Syntax error in regular expression
		/// </summary>
		SyntaxErrorInRegularExpression = 5017,

		/// <summary>
		/// Unexpected quantifier
		/// </summary>
		UnexpectedQuantifier = 5018,

		/// <summary>
		/// Expected ']' in regular expression
		/// </summary>
		ExpectedRightSquareBracketInRegularExpression = 5019,

		/// <summary>
		/// Expected ')' in regular expression
		/// </summary>
		ExpectedRightParenthesisInRegularExpression = 5020,

		/// <summary>
		/// Invalid range in character set
		/// </summary>
		InvalidRangeInCharacterSet = 5021,

		/// <summary>
		/// Exception thrown and not caught
		/// </summary>
		ExceptionThrownAndNotCaught = 5022,

		/// <summary>
		/// Function does not have a valid prototype object
		/// </summary>
		FunctionDoesNotHaveValidPrototypeObject = 5023,

		/// <summary>
		/// The URI to be encoded contains an invalid character
		/// </summary>
		UriToBeEncodedContainsInvalidCharacter = 5024,

		/// <summary>
		/// The URI to be decoded is not a valid encoding
		/// </summary>
		UriToBeDecodedIsNotValidEncoding = 5025,

		/// <summary>
		/// The number of fractional digits is out of range
		/// </summary>
		NumberOfFractionalDigitsIsOutOfRange = 5026,

		/// <summary>
		/// The precision is out of range
		/// </summary>
		PrecisionOutOfRange = 5027,

		/// <summary>
		/// Array or arguments object expected
		/// </summary>
		ArrayOrArgumentsObjectExpected = 5028,

		/// <summary>
		/// Array length must be a finite positive integer
		/// </summary>
		ArrayLengthMustBeFinitePositiveInteger = 5029,

		/// <summary>
		/// Array length must be assigned a finite positive number
		/// </summary>
		ArrayLengthMustBeAssignedFinitePositiveNumber = 5030,

		/// <summary>
		/// Circular reference in value argument not supported
		/// </summary>
		CircularReferenceInValueArgumentNotSupported = 5034,

		/// <summary>
		/// Invalid replacer argument
		/// </summary>
		InvalidReplacerArgument = 5035

		#endregion
	}
}
#endif
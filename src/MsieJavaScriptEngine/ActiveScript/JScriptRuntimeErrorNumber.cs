#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// JScript runtime error numbers
	/// </summary>
	internal static class JScriptRuntimeErrorNumber
	{
		#region Engine

		/// <summary>
		/// Out of memory
		/// </summary>
		public const int OutOfMemory = 7;

		#endregion

		#region Runtime

		/// <summary>
		/// Out of stack space
		/// </summary>
		public const int OutOfStackSpace = 28;

		/// <summary>
		/// Cannot assign to <c>this</c>
		/// </summary>
		public const int CannotAssignToThisKeyword = 5000;

		/// <summary>
		/// Number expected
		/// </summary>
		public const int NumberExpected = 5001;

		/// <summary>
		/// Function expected
		/// </summary>
		public const int FunctionExpected = 5002;

		/// <summary>
		/// Cannot assign to a function result
		/// </summary>
		public const int CannotAssignToFunctionResult = 5003;

		/// <summary>
		/// String expected
		/// </summary>
		public const int StringExpected = 5005;

		/// <summary>
		/// Date object expected
		/// </summary>
		public const int DateObjectExpected = 5006;

		/// <summary>
		/// Object expected
		/// </summary>
		public const int ObjectExpected = 5007;

		/// <summary>
		/// Illegal assignment
		/// </summary>
		public const int IllegalAssignment = 5008;

		/// <summary>
		/// Undefined identifier
		/// </summary>
		public const int UndefinedIdentifier = 5009;

		/// <summary>
		/// Boolean expected
		/// </summary>
		public const int BooleanExpected = 5010;

		/// <summary>
		/// Object member expected
		/// </summary>
		public const int ObjectMemberExpected = 5012;

		/// <summary>
		/// VBArray expected
		/// </summary>
		public const int VbArrayExpected = 5013;

		/// <summary>
		/// JavaScript object expected
		/// </summary>
		public const int JavaScriptObjectExpected = 5014;

		/// <summary>
		/// Enumerator object expected
		/// </summary>
		public const int EnumeratorObjectExpected = 5015;

		/// <summary>
		/// Regular Expression object expected
		/// </summary>
		public const int RegularExpressionObjectExpected = 5016;

		/// <summary>
		/// Syntax error in regular expression
		/// </summary>
		public const int SyntaxErrorInRegularExpression = 5017;

		/// <summary>
		/// Unexpected quantifier
		/// </summary>
		public const int UnexpectedQuantifier = 5018;

		/// <summary>
		/// Expected <c>]</c> in regular expression
		/// </summary>
		public const int ExpectedRightSquareBracketInRegularExpression = 5019;

		/// <summary>
		/// Expected <c>)</c> in regular expression
		/// </summary>
		public const int ExpectedRightParenthesisInRegularExpression = 5020;

		/// <summary>
		/// Invalid range in character set
		/// </summary>
		public const int InvalidRangeInCharacterSet = 5021;

		/// <summary>
		/// Exception thrown and not caught
		/// </summary>
		public const int ExceptionThrownAndNotCaught = 5022;

		/// <summary>
		/// Function does not have a valid prototype object
		/// </summary>
		public const int FunctionDoesNotHaveValidPrototypeObject = 5023;

		/// <summary>
		/// The URI to be encoded contains an invalid character
		/// </summary>
		public const int UriToBeEncodedContainsInvalidCharacter = 5024;

		/// <summary>
		/// The URI to be decoded is not a valid encoding
		/// </summary>
		public const int UriToBeDecodedIsNotValidEncoding = 5025;

		/// <summary>
		/// The number of fractional digits is out of range
		/// </summary>
		public const int NumberOfFractionalDigitsIsOutOfRange = 5026;

		/// <summary>
		/// The precision is out of range
		/// </summary>
		public const int PrecisionOutOfRange = 5027;

		/// <summary>
		/// Array or arguments object expected
		/// </summary>
		public const int ArrayOrArgumentsObjectExpected = 5028;

		/// <summary>
		/// Array length must be a finite positive integer
		/// </summary>
		public const int ArrayLengthMustBeFinitePositiveInteger = 5029;

		/// <summary>
		/// Array length must be assigned a finite positive number
		/// </summary>
		public const int ArrayLengthMustBeAssignedFinitePositiveNumber = 5030;

		/// <summary>
		/// Circular reference in value argument not supported
		/// </summary>
		public const int CircularReferenceInValueArgumentNotSupported = 5034;

		/// <summary>
		/// Invalid replacer argument
		/// </summary>
		public const int InvalidReplacerArgument = 5035;

		#endregion
	}
}
#endif
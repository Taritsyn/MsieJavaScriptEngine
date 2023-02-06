#if NETFRAMEWORK
using System.Collections.Generic;

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
		/// Mapping of error numbers and types
		/// </summary>
		private static readonly Dictionary<int, string> _runtimeErrorTypeMap = new Dictionary<int, string>
		{
			{ JScriptRuntimeErrorNumber.OutOfStackSpace, JsErrorType.Common },
			{ JScriptRuntimeErrorNumber.CannotAssignToThisKeyword, JsErrorType.Reference },
			{ JScriptRuntimeErrorNumber.NumberExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.FunctionExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.CannotAssignToFunctionResult, JsErrorType.Reference },
			{ JScriptRuntimeErrorNumber.StringExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.DateObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.ObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.IllegalAssignment, string.Empty },
			{ JScriptRuntimeErrorNumber.UndefinedIdentifier, JsErrorType.Reference },
			{ JScriptRuntimeErrorNumber.BooleanExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.ObjectMemberExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.VbArrayExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.JavaScriptObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.EnumeratorObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.RegularExpressionObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.SyntaxErrorInRegularExpression, JsErrorType.Syntax },
			{ JScriptRuntimeErrorNumber.UnexpectedQuantifier, JsErrorType.Syntax },
			{ JScriptRuntimeErrorNumber.ExpectedRightSquareBracketInRegularExpression, JsErrorType.Syntax },
			{ JScriptRuntimeErrorNumber.ExpectedRightParenthesisInRegularExpression, JsErrorType.Syntax },
			{ JScriptRuntimeErrorNumber.InvalidRangeInCharacterSet, JsErrorType.Syntax },
			{ JScriptRuntimeErrorNumber.ExceptionThrownAndNotCaught, string.Empty },
			{ JScriptRuntimeErrorNumber.FunctionDoesNotHaveValidPrototypeObject, string.Empty },
			{ JScriptRuntimeErrorNumber.UriToBeEncodedContainsInvalidCharacter, JsErrorType.URI },
			{ JScriptRuntimeErrorNumber.UriToBeDecodedIsNotValidEncoding, JsErrorType.URI },
			{ JScriptRuntimeErrorNumber.NumberOfFractionalDigitsIsOutOfRange, JsErrorType.Range },
			{ JScriptRuntimeErrorNumber.PrecisionOutOfRange, JsErrorType.Range },
			{ JScriptRuntimeErrorNumber.ArrayOrArgumentsObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.ArrayLengthMustBeFinitePositiveInteger, JsErrorType.Range },
			{ JScriptRuntimeErrorNumber.ArrayLengthMustBeAssignedFinitePositiveNumber, JsErrorType.Range },
			{ JScriptRuntimeErrorNumber.CircularReferenceInValueArgumentNotSupported, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.InvalidReplacerArgument, string.Empty }
		};


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
		/// <returns>Result of check (<c>true</c> - supports; <c>false</c> - does not support)</returns>
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

		/// <summary>
		/// Gets a error type by number
		/// </summary>
		/// <param name="errorNumber">Error number</param>
		/// <returns>Error type</returns>
		protected override string GetErrorTypeByNumber(int errorNumber)
		{
			return ActiveScriptJsErrorHelpers.GetErrorTypeByNumber(errorNumber, _runtimeErrorTypeMap);
		}

		protected override void InnerRemoveVariable(string variableName)
		{
			InnerSetVariableValue(variableName, null);

			if (_hostItems.ContainsKey(variableName))
			{
				_hostItems.Remove(variableName);
			}
		}

		#region IInnerJsEngine implementation

		public override bool SupportsScriptPrecompilation
		{
			get { return false; }
		}


		public override void Interrupt()
		{
			_interruptRequested = true;
		}

		#endregion

		#endregion
	}
}
#endif
#if !NETSTANDARD
using System.Collections.Generic;

using MsieJavaScriptEngine.Constants;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Classic JS engine
	/// </summary>
	internal sealed partial class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
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
			{ JScriptRuntimeErrorNumber.IllegalAssignment, JsErrorType.Reference },
			{ JScriptRuntimeErrorNumber.UndefinedIdentifier, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.BooleanExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.ObjectMemberExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.VbArrayExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.JavaScriptObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.EnumeratorObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.RegularExpressionObjectExpected, JsErrorType.Type },
			{ JScriptRuntimeErrorNumber.SyntaxErrorInRegularExpression, JsErrorType.RegExp },
			{ JScriptRuntimeErrorNumber.UnexpectedQuantifier, JsErrorType.RegExp },
			{ JScriptRuntimeErrorNumber.ExpectedRightSquareBracketInRegularExpression, JsErrorType.RegExp },
			{ JScriptRuntimeErrorNumber.ExpectedRightParenthesisInRegularExpression, JsErrorType.RegExp },
			{ JScriptRuntimeErrorNumber.InvalidRangeInCharacterSet, JsErrorType.RegExp },
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
		/// Constructs an instance of the Classic Active Script engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		public ClassicActiveScriptJsEngine(JsEngineSettings settings)
			: base(settings, ClassId.Classic, ScriptLanguageVersion.None, "6", "Microsoft JScript ")
		{ }


		/// <summary>
		/// Checks a support of the Classic Active Script engine on the machine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			bool isSupported = IsSupported(ClassId.Classic, ref _isSupported, ref _supportSynchronizer);

			return isSupported;
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

		#region ActiveScriptJsEngineBase overrides

		/// <summary>
		/// Creates a instance of the Active Script site
		/// </summary>
		/// <returns>Instance of the Active Script site</returns>
		protected override ScriptSiteBase CreateScriptSite()
		{
			return new ScriptSite(this);
		}

		protected override void InnerRemoveVariable(string variableName)
		{
			if (_hostItems.ContainsKey(variableName))
			{
				_hostItems.Remove(variableName);
			}
			else
			{
				InnerSetVariableValue(variableName, null);
			}
		}

		#region IInnerJsEngine implementation

		public override bool SupportsScriptPrecompilation
		{
			get { return false; }
		}


		public override void Interrupt()
		{
			var exceptionInfo = new EXCEPINFO
			{
				scode = ComErrorCode.E_ABORT
			};
			_activeScriptWrapper.InterruptScriptThread(ScriptThreadId.Base, ref exceptionInfo,
				ScriptInterruptFlags.None);
		}

		#endregion

		#endregion
	}
}
#endif
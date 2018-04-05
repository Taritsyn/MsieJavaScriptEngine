#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Reflection;

using MsieJavaScriptEngine.Constants;
#if NET40
using MsieJavaScriptEngine.Polyfills.System;
#endif
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Classic JS engine
	/// </summary>
	internal sealed partial class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
	{
		/// <summary>
		/// Name of resource, which contains a ECMAScript 5 Polyfill
		/// </summary>
		private const string ES5_POLYFILL_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.ES5.min.js";

		/// <summary>
		/// Name of resource, which contains a JSON2 library
		/// </summary>
		private const string JSON2_LIBRARY_RESOURCE_NAME = "MsieJavaScriptEngine.Resources.json2.min.js";

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
		{
			LoadResources(_settings.UseEcmaScript5Polyfill, _settings.UseJson2Library);
		}


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

		/// <summary>
		/// Loads a resources
		/// </summary>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		private void LoadResources(bool useEcmaScript5Polyfill, bool useJson2Library)
		{
			Assembly assembly = GetType().GetTypeInfo().Assembly;

			if (useEcmaScript5Polyfill)
			{
				ExecuteResource(ES5_POLYFILL_RESOURCE_NAME, assembly);
			}

			if (useJson2Library)
			{
				ExecuteResource(JSON2_LIBRARY_RESOURCE_NAME, assembly);
			}
		}

		/// <summary>
		/// Executes a code from embedded JS-resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		private void ExecuteResource(string resourceName, Assembly assembly)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException(
					nameof(resourceName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(resourceName))
				);
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					nameof(assembly),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(assembly))
				);
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(resourceName)),
					nameof(resourceName)
				);
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			Execute(code, resourceName);
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
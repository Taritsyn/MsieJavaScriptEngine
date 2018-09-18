//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated by a tool.
//
//	 Changes to this file may cause incorrect behavior and will be lost if
//	 the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace MsieJavaScriptEngine.Resources
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	internal class CommonStrings
	{
		private static Lazy<ResourceManager> _resourceManager =
			new Lazy<ResourceManager>(() => new ResourceManager(
				"MsieJavaScriptEngine.Resources.CommonStrings",
#if NET40
				typeof(CommonStrings).Assembly
#else
				typeof(CommonStrings).GetTypeInfo().Assembly
#endif
			));

		private static CultureInfo _resourceCulture;

		/// <summary>
		/// Returns a cached ResourceManager instance used by this class
		/// </summary>
		internal static ResourceManager ResourceManager
		{
			get
			{
				return _resourceManager.Value;
			}
		}

		/// <summary>
		/// Overrides a current thread's CurrentUICulture property for all
		/// resource lookups using this strongly typed resource class
		/// </summary>
		internal static CultureInfo Culture
		{
			get
			{
				return _resourceCulture;
			}
			set
			{
				_resourceCulture = value;
			}
		}

		/// <summary>
		/// Looks up a localized string similar to "The parameter '{0}' must be a non-empty string."
		/// </summary>
		internal static string Common_ArgumentIsEmpty
		{
			get { return GetString("Common_ArgumentIsEmpty"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The parameter '{0}' must be a non-nullable."
		/// </summary>
		internal static string Common_ArgumentIsNull
		{
			get { return GetString("Common_ArgumentIsNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Cannot convert object of type `{0}` to type `{1}`."
		/// </summary>
		internal static string Common_CannotConvertObjectToType
		{
			get { return GetString("Common_CannotConvertObjectToType"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "File '{0}' not exist."
		/// </summary>
		internal static string Common_FileNotExist
		{
			get { return GetString("Common_FileNotExist"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Resource with name '{0}' is null."
		/// </summary>
		internal static string Common_ResourceIsNull
		{
			get { return GetString("Common_ResourceIsNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "See the original error message: “{0}”."
		/// </summary>
		internal static string Common_SeeOriginalErrorMessage
		{
			get { return GetString("Common_SeeOriginalErrorMessage"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Cannot convert null to a value type."
		/// </summary>
		internal static string Common_ValueTypeCannotBeNull
		{
			get { return GetString("Common_ValueTypeCannotBeNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Most likely it happened, because the '{0}' assembly was not registered in your system."
		/// </summary>
		internal static string Engine_AssemblyNotRegistered
		{
			get { return GetString("Engine_AssemblyNotRegistered"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Try to install the Windows 10 with Edge browser."
		/// </summary>
		internal static string Engine_EdgeInstallationRequired
		{
			get { return GetString("Engine_EdgeInstallationRequired"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Try to install the Internet Explorer {0} or higher."
		/// </summary>
		internal static string Engine_IeInstallationRequired
		{
			get { return GetString("Engine_IeInstallationRequired"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Failed to create instance of the MsieJsEngine in {0} mode."
		/// </summary>
		internal static string Engine_JsEngineNotLoaded
		{
			get { return GetString("Engine_JsEngineNotLoaded"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Maximum stack size must be non-negative."
		/// </summary>
		internal static string Engine_MaxStackSizeMustBeNonNegative
		{
			get { return GetString("Engine_MaxStackSizeMustBeNonNegative"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Call stack"
		/// </summary>
		internal static string ErrorDetails_CallStack
		{
			get { return GetString("ErrorDetails_CallStack"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Category"
		/// </summary>
		internal static string ErrorDetails_Category
		{
			get { return GetString("ErrorDetails_Category"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Column number"
		/// </summary>
		internal static string ErrorDetails_ColumnNumber
		{
			get { return GetString("ErrorDetails_ColumnNumber"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Description"
		/// </summary>
		internal static string ErrorDetails_Description
		{
			get { return GetString("ErrorDetails_Description"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Document name"
		/// </summary>
		internal static string ErrorDetails_DocumentName
		{
			get { return GetString("ErrorDetails_DocumentName"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Engine mode"
		/// </summary>
		internal static string ErrorDetails_EngineMode
		{
			get { return GetString("ErrorDetails_EngineMode"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Line number"
		/// </summary>
		internal static string ErrorDetails_LineNumber
		{
			get { return GetString("ErrorDetails_LineNumber"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Message"
		/// </summary>
		internal static string ErrorDetails_Message
		{
			get { return GetString("ErrorDetails_Message"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Source fragment"
		/// </summary>
		internal static string ErrorDetails_SourceFragment
		{
			get { return GetString("ErrorDetails_SourceFragment"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Type"
		/// </summary>
		internal static string ErrorDetails_Type
		{
			get { return GetString("ErrorDetails_Type"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The function with the name '{0}' does not exist."
		/// </summary>
		internal static string Runtime_FunctionNotExist
		{
			get { return GetString("Runtime_FunctionNotExist"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The '{0}' line of the script call stack has an incorrect format."
		/// </summary>
		internal static string Runtime_InvalidCallStackLineFormat
		{
			get { return GetString("Runtime_InvalidCallStackLineFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Script execution was interrupted."
		/// </summary>
		internal static string Runtime_ScriptInterrupted
		{
			get { return GetString("Runtime_ScriptInterrupted"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Сannot execute a '{0}' file, because it is empty."
		/// </summary>
		internal static string Usage_CannotExecuteEmptyFile
		{
			get { return GetString("Usage_CannotExecuteEmptyFile"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Сannot execute a '{0}' resource, because it is empty."
		/// </summary>
		internal static string Usage_CannotExecuteEmptyResource
		{
			get { return GetString("Usage_CannotExecuteEmptyResource"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Сannot execute a pre-compiled script, because it was created for another mode with name `{0}`."
		/// </summary>
		internal static string Usage_CannotExecutePrecompiledScriptForAnotherJsEngineMode
		{
			get { return GetString("Usage_CannotExecutePrecompiledScriptForAnotherJsEngineMode"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Сannot pre-compile a '{0}' file, because it is empty."
		/// </summary>
		internal static string Usage_CannotPrecompileEmptyFile
		{
			get { return GetString("Usage_CannotPrecompileEmptyFile"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Сannot pre-compile a '{0}' resource, because it is empty."
		/// </summary>
		internal static string Usage_CannotPrecompileEmptyResource
		{
			get { return GetString("Usage_CannotPrecompileEmptyResource"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The embedded host object '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		internal static string Usage_EmbeddedHostObjectTypeNotSupported
		{
			get { return GetString("Usage_EmbeddedHostObjectTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The embedded host type `{0}` is not supported."
		/// </summary>
		internal static string Usage_EmbeddedHostTypeNotSupported
		{
			get { return GetString("Usage_EmbeddedHostTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "One of the function parameters '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		internal static string Usage_FunctionParameterTypeNotSupported
		{
			get { return GetString("Usage_FunctionParameterTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The document name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidDocumentNameFormat
		{
			get { return GetString("Usage_InvalidDocumentNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The file name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidFileNameFormat
		{
			get { return GetString("Usage_InvalidFileNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The function name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidFunctionNameFormat
		{
			get { return GetString("Usage_InvalidFunctionNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The resource name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidResourceNameFormat
		{
			get { return GetString("Usage_InvalidResourceNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The script item name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidScriptItemNameFormat
		{
			get { return GetString("Usage_InvalidScriptItemNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The variable name '{0}' has incorrect format."
		/// </summary>
		internal static string Usage_InvalidVariableNameFormat
		{
			get { return GetString("Usage_InvalidVariableNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Selected '{0}' mode of JavaScript engine is not supported."
		/// </summary>
		internal static string Usage_JsEngineModeNotSupported
		{
			get { return GetString("Usage_JsEngineModeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "It is prohibited to use the {0} and {1} engines in one process."
		/// </summary>
		internal static string Usage_JsEnginesConflictInProcess
		{
			get { return GetString("Usage_JsEnginesConflictInProcess"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "It is prohibited to use the Chakra JsRT and Chakra ActiveScript engines on one machine at a time."
		/// </summary>
		internal static string Usage_JsEnginesConflictOnMachine
		{
			get { return GetString("Usage_JsEnginesConflictOnMachine"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The type of return value `{0}` is not supported."
		/// </summary>
		internal static string Usage_ReturnValueTypeNotSupported
		{
			get { return GetString("Usage_ReturnValueTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The variable '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		internal static string Usage_VariableTypeNotSupported
		{
			get { return GetString("Usage_VariableTypeNotSupported"); }
		}

		private static string GetString(string name)
		{
			string value = ResourceManager.GetString(name, _resourceCulture);

			return value;
		}
	}
}
#if !NETSTANDARD1_3
using System;
using System.Reflection;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Active Script version of Classic JS engine
	/// </summary>
	internal sealed class ClassicActiveScriptJsEngine : ActiveScriptJsEngineBase
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
		/// Constructs instance of the Classic Active Script engine
		/// </summary>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		/// <param name="useEcmaScript5Polyfill">Flag for whether to use the ECMAScript 5 Polyfill</param>
		/// <param name="useJson2Library">Flag for whether to use the JSON2 library</param>
		public ClassicActiveScriptJsEngine(bool enableDebugging, bool useEcmaScript5Polyfill, bool useJson2Library)
			: base(JsEngineMode.Classic, enableDebugging)
		{
			LoadResources(useEcmaScript5Polyfill, useJson2Library);
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
					"resourceName", string.Format(CommonStrings.Common_ArgumentIsNull, "resourceName"));
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					"assembly", string.Format(CommonStrings.Common_ArgumentIsNull, "assembly"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			Execute(code, resourceName);
		}
	}
}
#endif
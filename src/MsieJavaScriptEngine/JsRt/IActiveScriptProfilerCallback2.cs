using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// IActiveScriptProfilerCallback2 COM interface
	/// </summary>
	[Guid("31B7F8AD-A637-409C-B22F-040995B6103D")]
	internal interface IActiveScriptProfilerCallback2 : IActiveScriptProfilerCallback
	{
		/// <summary>
		/// Called when a function is entered by name
		/// </summary>
		/// <param name="functionName">The name of the function</param>
		/// <param name="type">The type of the function</param>
		void OnFunctionEnterByName(string functionName, ProfilerScriptType type);

		/// <summary>
		/// Called when a function is exited by name
		/// </summary>
		/// <param name="functionName">The name of the function</param>
		/// <param name="type">The type of the function</param>
		void OnFunctionExitByName(string functionName, ProfilerScriptType type);
	}
}
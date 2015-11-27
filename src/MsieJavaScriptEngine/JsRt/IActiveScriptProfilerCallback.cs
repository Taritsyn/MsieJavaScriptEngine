namespace MsieJavaScriptEngine.JsRt
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	/// IActiveScriptProfilerCallback COM interface
	/// </summary>
	[Guid("740eca23-7d9d-42e5-ba9d-f8b24b1c7a9b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptProfilerCallback
	{
		/// <summary>
		/// Called when the profile is started
		/// </summary>
		/// <param name="context">The context provided when profiling was started</param>
		void Initialize(uint context);

		/// <summary>
		/// Called when profiling is stopped
		/// </summary>
		/// <param name="reason">The reason code provided when profiling was stopped</param>
		void Shutdown(uint reason);

		/// <summary>
		/// Called when a script is compiled
		/// </summary>
		/// <param name="scriptId">The ID of the script</param>
		/// <param name="type">The type of the script</param>
		/// <param name="debugDocumentContext">The debug document context, if any</param>
		void ScriptCompiled(int scriptId, ProfilerScriptType type, IntPtr debugDocumentContext);

		/// <summary>
		/// Called when a function is compiled
		/// </summary>
		/// <param name="functionId">The ID of the function</param>
		/// <param name="scriptId">The ID of the script</param>
		/// <param name="functionName">The name of the function</param>
		/// <param name="functionNameHint">The function name hint</param>
		/// <param name="debugDocumentContext">The debug document context, if any</param>
		void FunctionCompiled(int functionId, int scriptId, [MarshalAs(UnmanagedType.LPWStr)] string functionName, [MarshalAs(UnmanagedType.LPWStr)] string functionNameHint, IntPtr debugDocumentContext);

		/// <summary>
		/// Called when a function is entered
		/// </summary>
		/// <param name="scriptId">The ID of the script</param>
		/// <param name="functionId">The ID of the function</param>
		void OnFunctionEnter(int scriptId, int functionId);

		/// <summary>
		/// Called when a function is exited
		/// </summary>
		/// <param name="scriptId">The ID of the script</param>
		/// <param name="functionId">The ID of the function</param>
		void OnFunctionExit(int scriptId, int functionId);
	}
}
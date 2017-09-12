using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Profiling
{
	/// <summary>
	/// Provides methods that are used by the scripting engine to notify a profiler object when events occur.
	/// This interface is implemented by the profiler object.
	/// </summary>
	[Guid("740eca23-7d9d-42e5-ba9d-f8b24b1c7a9b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptProfilerCallback
	{
		/// <summary>
		/// Called to initialize the profiler object whenever profiling is started on a scripting engine
		/// </summary>
		/// <param name="context">The context provided when profiling was started</param>
		void Initialize(
			uint context
		);

		/// <summary>
		/// Called to free and release the profiler object whenever profiling is stopped on a scripting
		/// engine</summary>
		/// <param name="reason">The reason for shutting down</param>
		void Shutdown(
			uint reason
		);

		/// <summary>
		/// Notifies the profiler object that the scripting engine compiled the script
		/// </summary>
		/// <param name="scriptId">The unique ID of the script that was compiled</param>
		/// <param name="type">The type of the script that was compiled</param>
		/// <param name="debugDocumentContext">The debug document context, if any</param>
		void ScriptCompiled(
			int scriptId,
			ProfilerScriptType type,
			IntPtr debugDocumentContext
		);

		/// <summary>
		/// Notifies the profiler object that the scripting engine encountered a function when compiling a
		/// script
		/// </summary>
		/// <param name="functionId">The unique ID of the function</param>
		/// <param name="scriptId">The unique ID of the script that the function is part of</param>
		/// <param name="functionName">The name of the function, or null for an anonymous function</param>
		/// <param name="functionNameHint">The inferred name of the function, or null if the scripting
		/// engine does not infer any name</param>
		/// <param name="debugDocumentContext">The debug document context, if any</param>
		void FunctionCompiled(
			int functionId,
			int scriptId,
			[MarshalAs(UnmanagedType.LPWStr)] string functionName,
			[MarshalAs(UnmanagedType.LPWStr)] string functionNameHint,
			IntPtr debugDocumentContext
		);

		/// <summary>
		/// Notifies the profiler object that the scripting engine is about to execute a function call
		/// that is not a call into the Document Object Model (DOM).
		/// </summary>
		/// <param name="scriptId">The unique ID of the script that the function is part of</param>
		/// <param name="functionId">The unique ID of the function</param>
		void OnFunctionEnter(
			int scriptId,
			int functionId
		);

		/// <summary>
		/// Notifies the profiler object that the scripting engine finished executing a function call
		/// that is not a call into the DOM
		/// </summary>
		/// <param name="scriptId">The unique ID of the script that the function is part of</param>
		/// <param name="functionId">The unique ID of the function</param>
		void OnFunctionExit(
			int scriptId,
			int functionId
		);
	}
}
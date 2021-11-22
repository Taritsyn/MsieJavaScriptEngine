using System;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Exposes non-remote debugging methods for use by language engines and hosts
	/// </summary>
	[ComImport]
	[Guid("51973C32-CB0C-11d0-B5C9-00A0244A0E7A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IDebugApplication32 // : IRemoteDebugApplication
	{
#if NETFRAMEWORK
		#region IRemoteDebugApplication methods

		/// <summary>
		/// Continues an application that is currently in a breakpoint
		/// </summary>
		/// <param name="thread">For stepping modes, the thread which is to be affected by the stepping mode</param>
		/// <param name="breakResumeAction">The action to take upon resuming the application</param>
		/// <param name="errorResumeAction">The action to take in the case that the application stopped because of an error</param>
		void ResumeFromBreakPoint(
			[In] [MarshalAs(UnmanagedType.Interface)] IRemoteDebugApplicationThread thread,
			[In] BreakResumeAction breakResumeAction,
			[In] ErrorResumeAction errorResumeAction
		);

		/// <summary>
		/// Causes the application to break into the debugger at the earliest opportunity
		/// </summary>
		void CauseBreak();

		/// <summary>
		/// Connects a debugger to this application
		/// </summary>
		/// <param name="debugger">The debugger to attach to this application</param>
		void ConnectDebugger(
			[In] [MarshalAs(UnmanagedType.Interface)] IApplicationDebugger debugger
		);

		/// <summary>
		/// Disconnects the current debugger from the application
		/// </summary>
		void DisconnectDebugger();

		/// <summary>
		/// Returns the current debugger connected to the application
		/// </summary>
		/// <param name="debugger">The current debugger connected to the application</param>
		/// <returns>The method returns an HRESULT</returns>
		[PreserveSig]
		uint GetDebugger(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IApplicationDebugger debugger
		);

		/// <summary>
		/// Allows the creation of objects in the application process by code that is out-of-process
		/// to the application
		/// </summary>
		/// <param name="clsid">Class identifier (CLSID) of the object to create</param>
		/// <param name="outer">The aggregate object's IUnknown interface</param>
		/// <param name="clsContext">Context for running executable code</param>
		/// <param name="iid">The interface identifier used to communicate with the object</param>
		/// <param name="instance">Variable that receives the interface pointer requested in <paramref name="iid"/></param>
		void CreateInstanceAtApplication(
			[In] ref Guid clsid,
			[In] [MarshalAs(UnmanagedType.IUnknown)] object outer,
			[In] uint clsContext,
			[In] ref Guid iid,
			[MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 3)] out object instance
		);

		/// <summary>
		/// Indicates if the application is responsive
		/// </summary>
		void QueryAlive();

		/// <summary>
		/// Enumerates all threads known to be associated with the application
		/// </summary>
		/// <param name="enumThreads">Enumerator that lists all threads known to be associated with
		/// the application</param>
		void EnumThreads(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumRemoteDebugApplicationThreads enumThreads
		);

		/// <summary>
		/// Returns the name of this application node
		/// </summary>
		/// <param name="name">Name of this application node</param>
		void GetName(
			[Out] [MarshalAs(UnmanagedType.BStr)] out string name
		);

		/// <summary>
		/// Returns the application node under which all nodes associated with the application are added
		/// </summary>
		/// <param name="node">The debug application node under which all nodes associated with
		/// the application are added</param>
		void GetRootNode(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationNode node
		);

		/// <summary>
		/// Enumerates the global expression contexts for all languages running in this application
		/// </summary>
		/// <param name="enumContexts">Enumerator that lists the global expression contexts for all
		/// languages running in this application</param>
		void EnumGlobalExpressionContexts(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugExpressionContexts enumContexts
		);

		#endregion

		/// <summary>
		/// Sets the name of the application
		/// </summary>
		/// <param name="name">The name of the application</param>
		void SetName(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string name
		);

		/// <summary>
		/// Notifies the process debug manager that a language engine in single-step mode is about
		/// to return to its caller
		/// </summary>
		void StepOutComplete();

		/// <summary>
		/// Causes the given string to be displayed by the debugger IDE
		/// </summary>
		/// <param name="str">String to display in the debugger</param>
		void DebugOutput(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string str
		);

		/// <summary>
		/// Starts the default debugger IDE and attaches a debug session to this application, if one
		/// is not already attached
		/// </summary>
		void StartDebugSession();

		/// <summary>
		/// Causes the current thread to block and sends a notification of the breakpoint to the debugger IDE
		/// </summary>
		/// <param name="reason">The reason for the break</param>
		/// <param name="resumeAction">Action to take when the debugger resumes the application</param>
		void HandleBreakPoint(
			[In] BreakReason reason,
			[Out] out BreakResumeAction resumeAction
		);

		/// <summary>
		/// Causes this application to release all references and enter an inactive state
		/// </summary>
		void Close();

		/// <summary>
		/// Returns the current break flags for the application
		/// </summary>
		/// <param name="flags">The current break flags for the application</param>
		/// <param name="thread">The currently running thread</param>
		void GetBreakFlags(
			[Out] out AppBreakFlags flags,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IRemoteDebugApplicationThread thread
		);

		/// <summary>
		/// Returns the thread associated with the currently running thread
		/// </summary>
		/// <param name="thread">The thread associated with the currently running thread</param>
		void GetCurrentThread(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationThread thread
		);

		/// <summary>
		/// Provides asynchronous access to a given synchronous debug operation
		/// </summary>
		/// <param name="syncOperation">The synchronous debug operation object</param>
		/// <param name="asyncOperation">The asynchronous debug operation object</param>
		void CreateAsyncDebugOperation(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugSyncOperation syncOperation,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugAsyncOperation asyncOperation
		);

		/// <summary>
		/// Adds a stack frame enumerator provider to this application
		/// </summary>
		/// <param name="sniffer">The stack frame enumerator provider to add to this application</param>
		/// <param name="cookie">A cookie that is used to remove this stack frame enumerator provider
		/// from the application</param>
		void AddStackFrameSniffer(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugStackFrameSniffer sniffer,
			[Out] out uint cookie
		);

		/// <summary>
		/// Removes a stack frame enumerator provider from this application
		/// </summary>
		/// <param name="cookie">The cookie returned by the <see cref="AddStackFrameSniffer"/> method
		/// when the stack frame enumerator provider was added</param>
		void RemoveStackFrameSniffer(
			[In] uint cookie
		);

		/// <summary>
		/// Determines if the current running thread is the debugger thread
		/// </summary>
		/// <returns>The method returns an HRESULT</returns>
		[PreserveSig]
		uint QueryCurrentThreadIsDebuggerThread();

		/// <summary>
		/// Provides a mechanism for the caller to run code in the debugger thread
		/// </summary>
		/// <param name="call">The object to call</param>
		/// <param name="param1">First parameter to pass to the IDebugThreadCall.ThreadCallHandler method</param>
		/// <param name="param2">Second parameter to pass to the IDebugThreadCall.ThreadCallHandler method</param>
		/// <param name="param3">Third parameter to pass to the IDebugThreadCall.ThreadCallHandler method</param>
		void SynchronousCallInDebuggerThread(
			[In] [MarshalAs(UnmanagedType.Interface)] IDebugThreadCall32 call,
			[In] uint param1,
			[In] uint param2,
			[In] uint param3
		);

		/// <summary>
		/// Creates a new application node that is associated with a specific document provider
		/// </summary>
		/// <param name="node">The application node associated with this document provider</param>
		void CreateApplicationNode(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugApplicationNode node
		);

		/// <summary>
		/// Fires a generic event to the debugger's <see cref="IApplicationDebugger"/> interface
		/// </summary>
		/// <param name="iid">A GUID for the object</param>
		/// <param name="eventObject">An event object to pass to the debugger</param>
		void FireDebuggerEvent(
			[In] ref Guid iid,
			[In] [MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)] object eventObject
		);

		/// <summary>
		/// Causes the current thread to block and sends a notification of the error to the debugger IDE
		/// </summary>
		/// <param name="errorDebug">The error that occurred</param>
		/// <param name="scriptSite">The script site of the thread</param>
		/// <param name="breakResumeAction">Action to take when the debugger resumes the application</param>
		/// <param name="errorResumeAction">Action to take when the debugger resumes the application
		/// if there is an error</param>
		/// <param name="callOnScriptError">Flag which is <code>true</code> if the engine should call
		/// the <see cref="IActiveScriptSite.OnScriptError"/> method</param>
		void HandleRuntimeError(
			[In] [MarshalAs(UnmanagedType.Interface)] IActiveScriptErrorDebug errorDebug,
			[In] [MarshalAs(UnmanagedType.Interface)] IActiveScriptSite scriptSite,
			[Out] out BreakResumeAction breakResumeAction,
			[Out] out ErrorResumeAction errorResumeAction,
			[Out] [MarshalAs(UnmanagedType.Bool)] out bool callOnScriptError
		);

		/// <summary>
		/// Determines if a JIT debugger is registered
		/// </summary>
		/// <returns>If the method succeeds and a JIT debugger is registered, the method returns <code>true</code>.
		/// Otherwise, it returns <code>false</code>.</returns>
		[PreserveSig]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool FCanJitDebug();

		/// <summary>
		/// Determines if a JIT debugger is registered to auto-debug dumb hosts
		/// </summary>
		/// <returns>If the method succeeds and a JIT debugger is registered to auto-debug dumb hosts,
		/// the method returns <code>true</code>. Otherwise, it returns <code>false</code>.</returns>
		[PreserveSig]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool FIsAutoJitDebugEnabled();

		/// <summary>
		/// Adds a global expression context provider to this application
		/// </summary>
		/// <param name="provider">The global context provider to add to this application</param>
		/// <param name="cookie">A cookie that is used to remove this global expression context provider
		/// from the application</param>
		void AddGlobalExpressionContextProvider(
			[In] [MarshalAs(UnmanagedType.Interface)] IProvideExpressionContexts provider,
			[Out] out uint cookie
		);

		/// <summary>
		/// Removes a global expression context provider from this application
		/// </summary>
		/// <param name="cookie">The cookie returned by the <see cref="AddGlobalExpressionContextProvider"/> method
		/// when the global context provider was added</param>
		void RemoveGlobalExpressionContextProvider(
			[In] uint cookie
		);
#endif
	}
}
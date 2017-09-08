﻿#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Provides the methods necessary to initialize the scripting engine. The scripting engine
	/// must implement the <see cref="IActiveScript"/> interface.
	/// </summary>
	[ComImport]
	[Guid("bb1a2ae1-a4f9-11cf-8f20-00805f2cd064")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScript
	{
		/// <summary>
		/// Informs the scripting engine of the <see cref="IActiveScriptSite"/> interface site
		/// provided by the host. Call this method before any other <see cref="IActiveScript"/>
		/// interface methods is used.
		/// </summary>
		/// <param name="site">The host-supplied script site to be associated with this instance
		/// of the scripting engine. The site must be uniquely assigned to this scripting engine
		/// instance; it cannot be shared with other scripting engines.</param>
		void SetScriptSite(
			[In] IActiveScriptSite site
		);

		/// <summary>
		/// Retrieves the site object associated with the script engine
		/// </summary>
		/// <param name="iid">Identifier of the requested interface</param>
		/// <param name="site">The host's site object</param>
		void GetScriptSite(
			[In] Guid iid,
			[Out] [MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)] out IActiveScriptSite site
		);

		/// <summary>
		/// Puts the scripting engine into the given state. This method can be called from non-base
		/// threads without resulting in a non-base callout to host objects or to the
		/// <see cref="IActiveScriptSite"/> interface.
		/// </summary>
		/// <param name="state">Sets the scripting engine to the given state</param>
		void SetScriptState(
			[In] ScriptState state
		);

		/// <summary>
		/// Retrieves the current state of the scripting engine. This method can be called from
		/// non-base threads without resulting in a non-base callout to host objects or to the
		/// <see cref="IActiveScriptSite"/> interface.
		/// </summary>
		/// <param name="state">The value indicates the current state of the scripting engine
		/// associated with the calling thread</param>
		void GetScriptState(
			[Out] out ScriptState state
		);

		/// <summary>
		/// Causes the scripting engine to abandon any currently loaded script, lose its state, and
		/// release any interface pointers it has to other objects, thus entering a closed state.
		/// Event sinks, immediately executed script text, and macro invocations that are already
		/// in progress are completed before the state changes (use
		/// <see cref="IActiveScript.InterruptScriptThread"/> to cancel a running script thread).
		/// This method must be called by the creating host before the interface is released to
		/// prevent circular reference problems.
		/// </summary>
		void Close();

		/// <summary>
		/// Adds the name of a root-level item to the scripting engine's name space. A root-level item
		/// is an object with properties and methods, an event source, or all three.
		/// </summary>
		/// <param name="name">The name of the item as viewed from the script. The name must be unique
		/// and persistable</param>
		/// <param name="flags">Flags associated with an item</param>
		void AddNamedItem(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string name,
			[In] ScriptItemFlags flags
		);

		/// <summary>
		/// Adds a type library to the name space for the script. This is similar to the
		/// <code>#include</code> directive in C/C++. It allows a set of predefined items such as
		/// class definitions, <code>typedefs</code>, and named constants to be added to the run-time
		/// environment available to the script.
		/// </summary>
		/// <param name="clsId">CLSID of the type library to add</param>
		/// <param name="majorVersion">Major version number</param>
		/// <param name="minorVersion">Minor version number</param>
		/// <param name="typeLibFlags">Option flags</param>
		void AddTypeLib(
			[In] Guid clsId,
			[In] uint majorVersion,
			[In] uint minorVersion,
			[In] ScriptTypeLibFlags typeLibFlags
		);

		/// <summary>
		/// Retrieves the IDispatch interface for the methods and properties associated
		/// with the currently running script
		/// </summary>
		/// <param name="itemName">The name of the item for which the caller needs the associated
		/// dispatch object. If this parameter is null, the dispatch object contains as its members
		/// all of the global methods and properties defined by the script. Through the
		/// IDispatch interface and the associated <see cref="ITypeInfo"/> interface,
		/// the host can invoke script methods or view and modify script variables.</param>
		/// <param name="dispatch">The object associated with the script's global methods and
		/// properties. If the scripting engine does not support such an object, null is returned.</param>
		void GetScriptDispatch(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string itemName,
			[Out] [MarshalAs(UnmanagedType.IDispatch)] out object dispatch
		);

		/// <summary>
		/// Retrieves a scripting-engine-defined identifier for the currently executing thread.
		/// The identifier can be used in subsequent calls to script thread execution-control methods
		/// such as the <see cref="IActiveScript.InterruptScriptThread"/> method.
		/// </summary>
		/// <param name="threadId">The script thread identifier associated with the current thread.
		/// The interpretation of this identifier is left to the scripting engine, but it can be just
		/// a copy of the Windows thread identifier. If the Win32 thread terminates, this identifier
		/// becomes unassigned and can subsequently be assigned to another thread.</param>
		void GetCurrentScriptThreadId(
			[Out] out uint threadId
		);

		/// <summary>
		/// Retrieves a scripting-engine-defined identifier for the thread associated with the given
		/// Win32 thread
		/// </summary>
		/// <param name="win32ThreadId">Thread identifier of a running Win32 thread in the current
		/// process. Use the <see cref="IActiveScript.GetCurrentScriptThreadId"/> function to retrieve
		/// the thread identifier of the currently executing thread.</param>
		/// <param name="scriptThreadId">The script thread identifier associated with the given Win32
		/// thread. The interpretation of this identifier is left to the scripting engine, but it can
		/// be just a copy of the Windows thread identifier. Note that if the Win32 thread terminates,
		/// this identifier becomes unassigned and may subsequently be assigned to another thread.</param>
		void GetScriptThreadId(
			[In] uint win32ThreadId,
			[Out] out uint scriptThreadId
		);

		/// <summary>
		/// Retrieves the current state of a script thread
		/// </summary>
		/// <param name="scriptThreadId">Identifier of the thread for which the state is desired</param>
		/// <param name="threadState">The state of the indicated thread</param>
		void GetScriptThreadState(
			[In] uint scriptThreadId,
			[Out] out ScriptThreadState threadState
		);

		/// <summary>
		/// Interrupts the execution of a running script thread (an event sink, an immediate execution,
		/// or a macro invocation). This method can be used to terminate a script that is stuck (for
		/// example, in an infinite loop). It can be called from non-base threads without resulting in
		/// a non-base callout to host objects or to the <see cref="IActiveScriptSite"/> method.
		/// </summary>
		/// <param name="scriptThreadId">Identifier of the thread to interrupt</param>
		/// <param name="exceptionInfo">The error information that should be reported to the aborted script</param>
		/// <param name="flags">Option flags associated with the interruption</param>
		void InterruptScriptThread(
			[In] uint scriptThreadId,
			[In] ref EXCEPINFO exceptionInfo,
			[In] ScriptInterruptFlags flags
		);

		/// <summary>
		/// Clones the current scripting engine (minus any current execution state), returning
		/// a loaded scripting engine that has no site in the current thread. The properties
		/// of this new scripting engine will be identical to the properties the original scripting
		/// engine would be in if it were transitioned back to the initialized state.
		/// </summary>
		/// <param name="script">The cloned scripting engine. The host must create a site and call
		/// the <see cref="IActiveScript.SetScriptSite"/> method on the new scripting engine before it
		/// will be in the initialized state and, therefore, usable.</param>
		void Clone(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IActiveScript script
		);
	}
}
#endif
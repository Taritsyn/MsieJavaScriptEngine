﻿#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// Specifies the state of a scripting engine
	/// </summary>
	internal enum ScriptState : uint
	{
		/// <summary>
		/// Script has just been created, but has not yet been initialized using an <c>IPersist*</c>
		/// interface and <see cref="IActiveScript.SetScriptSite"/>
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		/// Script has been initialized, but is not running (connecting to other objects or
		/// sinking events) or executing any code. Code can be queried for execution by
		/// calling the <c>IActiveScriptParse.ParseScriptText</c> method.
		/// </summary>
		Initialized = 1,

		/// <summary>
		/// Script can execute code, but is not yet sinking the events of objects added by
		/// the <c>IActiveScript.AddNamedItem</c> method
		/// </summary>
		Started = 2,

		/// <summary>
		/// Script is loaded and connected for sinking events
		/// </summary>
		Connected = 3,

		/// <summary>
		/// Script is loaded and has a run-time execution state, but is temporarily
		/// disconnected from sinking events
		/// </summary>
		Disconnected = 4,

		/// <summary>
		/// Script has been closed. The scripting engine no longer works and returns errors
		/// for most methods
		/// </summary>
		Closed = 5
	}
}
#endif
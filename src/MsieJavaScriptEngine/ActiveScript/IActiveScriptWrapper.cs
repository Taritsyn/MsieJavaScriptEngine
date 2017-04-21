#if !NETSTANDARD1_3
using System;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.ActiveScript.Debugging;

namespace MsieJavaScriptEngine.ActiveScript
{
	[ComVisible(false)]
	internal interface IActiveScriptWrapper : IDisposable
	{
		/// <summary>
		/// Informs the scripting engine of the IActiveScriptSite interface site provided by the host.
		/// Call this method before any other IActiveScript interface methods is used
		/// </summary>
		/// <param name="site">The host-supplied script site to be associated with this instance
		/// of the scripting engine. The site must be uniquely assigned to this scripting engine
		/// instance; it cannot be shared with other scripting engines.</param>
		void SetScriptSite(
			[In] IActiveScriptSite site);

		/// <summary>
		/// Puts the scripting engine into the given state. This method can be called from non-base
		/// threads without resulting in a non-base callout to host objects or to the IActiveScriptSite
		/// interface.
		/// </summary>
		/// <param name="state">Sets the scripting engine to the given state</param>
		void SetScriptState(
			[In] ScriptState state);

		/// <summary>
		/// Adds the name of a root-level item to the scripting engine's name space. A root-level item
		/// is an object with properties and methods, an event source, or all three
		/// </summary>
		/// <param name="name">The name of the item as viewed from the script. The name must be unique
		/// and persistable</param>
		/// <param name="flags">Flags associated with an item</param>
		void AddNamedItem(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string name,
			[In] ScriptItemFlags flags);

		/// <summary>
		/// Retrieves the IDispatch interface for the methods and properties associated with the
		/// currently running script
		/// </summary>
		/// <param name="itemName">The name of the item for which the caller needs the associated
		/// dispatch object. If this parameter is NULL, the dispatch object contains as its members
		/// all of the global methods and properties defined by the script. Through the IDispatch
		/// interface and the associated ITypeInfo interface, the host can invoke script methods
		/// or view and modify script variables.</param>
		/// <param name="dispatch">The object associated with the script's global methods and
		/// properties. If the scripting engine does not support such an object, NULL is returned.</param>
		void GetScriptDispatch(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string itemName,
			[Out] [MarshalAs(UnmanagedType.IDispatch)] out object dispatch);

		/// <summary>
		/// Initializes the scripting engine.
		/// </summary>
		void InitNew();

		/// <summary>
		/// Adds a code scriptlet to the script. This method is used in environments where the
		/// persistent state of the script is intertwined with the host document and the host
		/// is responsible for restoring the script, rather than through an IPersist* interface.
		/// The primary examples are HTML scripting languages that allow scriptlets of code
		/// embedded in the HTML document to be attached to intrinsic events (for instance,
		/// ONCLICK="button1.text='Exit'").
		/// </summary>
		/// <param name="defaultName">The default name to associate with the scriptlet. If the
		/// scriptlet does not contain naming information (as in the ONCLICK example above),
		/// this name will be used to identify the scriptlet. If this parameter is NULL, the
		/// scripting engine manufactures a unique name, if necessary.</param>
		/// <param name="code">The scriptlet text to add. The interpretation of this string
		/// depends on the scripting language</param>
		/// <param name="itemName">The item name associated with this scriptlet. This parameter,
		/// in addition to pstrSubItemName, identifies the object for which the scriptlet is
		/// an event handler</param>
		/// <param name="subItemName">The name of a subobject of the named item with which this
		/// scriptlet is associated; this name must be found in the named item's type
		/// information. This parameter is NULL if the scriptlet is to be associated with the
		/// named item instead of a subitem. This parameter, in addition to pstrItemName,
		/// identifies the specific object for which the scriptlet is an event handler.</param>
		/// <param name="eventName">The name of the event for which the scriptlet is an event
		/// handler</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When the pstrCode parameter
		/// is parsed from a stream of text, the host typically uses a delimiter, such as two
		/// single quotation marks (''), to detect the end of the scriptlet. This parameter
		/// specifies the delimiter that the host used, allowing the scripting engine to
		/// provide some conditional primitive preprocessing (for example, replacing a single
		/// quotation mark ['] with two single quotation marks for use as a delimiter).
		/// Exactly how (and if) the scripting engine makes use of this information depends
		/// on the scripting engine. Set this parameter to NULL if the host did not use a
		/// delimiter to mark the end of the scriptlet.</param>
		/// <param name="sourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet.</param>
		/// <returns>Actual name used to identify the scriptlet. This is to be in
		/// order of preference: a name explicitly specified in the scriptlet text, the
		/// default name provided in pstrDefaultName, or a unique name synthesized by the
		/// scripting engine</returns>
		string AddScriptlet(
			string defaultName,
			string code,
			string itemName,
			string subItemName,
			string eventName,
			string delimiter,
			IntPtr sourceContextCookie,
			uint startingLineNumber,
			ScriptTextFlags flags);

		/// <summary>
		/// Parses the given code scriptlet, adding declarations into the namespace and
		/// evaluating code as appropriate
		/// </summary>
		/// <param name="code">The scriptlet text to evaluate. The interpretation of this
		/// string depends on the scripting language</param>
		/// <param name="itemName">The item name that gives the context in which the
		/// scriptlet is to be evaluated. If this parameter is NULL, the code is evaluated
		/// in the scripting engine's global context.</param>
		/// <param name="context">The context object. This object is reserved for use in a
		/// debugging environment, where such a context may be provided by the debugger to
		/// represent an active run-time context. If this parameter is NULL, the engine
		/// uses pstrItemName to identify the context</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When pstrCode is parsed
		/// from a stream of text, the host typically uses a delimiter, such as two single
		/// quotation marks (''), to detect the end of the scriptlet. This parameter specifies
		/// the delimiter that the host used, allowing the scripting engine to provide some
		/// conditional primitive preprocessing (for example, replacing a single quotation
		/// mark ['] with two single quotation marks for use as a delimiter). Exactly how
		/// (and if) the scripting engine makes use of this information depends on the
		/// scripting engine. Set this parameter to NULL if the host did not use a delimiter
		/// to mark the end of the scriptlet.</param>
		/// <param name="sourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <returns>The results of scriptlet processing, or NULL if the caller
		/// expects no result (that is, the SCRIPTTEXT_ISEXPRESSION value is not set)</returns>
		object ParseScriptText(
			string code,
			string itemName,
			object context,
			string delimiter,
			UIntPtr sourceContextCookie,
			uint startingLineNumber,
			ScriptTextFlags flags);

		void EnumCodeContextsOfPosition(
			UIntPtr sourceContext,
			uint offset,
			uint length,
			out IEnumDebugCodeContexts enumContexts);

		/// <summary>
		/// Starts a garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		void CollectGarbage(ScriptGCType type);
	}
}
#endif
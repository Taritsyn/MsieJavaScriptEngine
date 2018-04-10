#if !NETSTANDARD1_3
using System;

using MsieJavaScriptEngine.ActiveScript.Debugging;

namespace MsieJavaScriptEngine.ActiveScript
{
	internal interface IActiveScriptWrapper : IDisposable
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
			IActiveScriptSite site
		);

		/// <summary>
		/// Puts the scripting engine into the given state. This method can be called from non-base
		/// threads without resulting in a non-base callout to host objects or to the
		/// <see cref="IActiveScriptSite"/> interface.
		/// </summary>
		/// <param name="state">Sets the scripting engine to the given state</param>
		void SetScriptState(
			ScriptState state
		);

		/// <summary>
		/// Adds the name of a root-level item to the scripting engine's name space. A root-level item
		/// is an object with properties and methods, an event source, or all three.
		/// </summary>
		/// <param name="name">The name of the item as viewed from the script. The name must be unique
		/// and persistable</param>
		/// <param name="flags">Flags associated with an item</param>
		void AddNamedItem(
			string name,
			ScriptItemFlags flags
		);

		/// <summary>
		/// Gets a script dispatch
		/// </summary>
		/// <returns>The object associated with the script's global methods and properties</returns>
		object GetScriptDispatch();

		/// <summary>
		/// Initializes the scripting engine
		/// </summary>
		void InitNew();

		/// <summary>
		/// Parses the given code scriptlet, adding declarations into the namespace and
		/// evaluating code as appropriate
		/// </summary>
		/// <param name="code">The scriptlet text to evaluate. The interpretation of this
		/// string depends on the scripting language</param>
		/// <param name="itemName">The item name that gives the context in which the
		/// scriptlet is to be evaluated. If this parameter is null, the code is evaluated
		/// in the scripting engine's global context</param>
		/// <param name="context">The context object. This object is reserved for use in a
		/// debugging environment, where such a context may be provided by the debugger to
		/// represent an active run-time context. If this parameter is null, the engine
		/// uses <paramref name="itemName"/> to identify the context.</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When <paramref name="code"/>
		/// is parsed from a stream of text, the host typically uses a delimiter, such as two
		/// single quotation marks (''), to detect the end of the scriptlet. This parameter
		/// specifies the delimiter that the host used, allowing the scripting engine to provide
		/// some conditional primitive preprocessing (for example, replacing a single quotation
		/// mark ['] with two single quotation marks for use as a delimiter). Exactly how
		/// (and if) the scripting engine makes use of this information depends on the
		/// scripting engine. Set this parameter to null if the host did not use a delimiter
		/// to mark the end of the scriptlet.</param>
		/// <param name="sourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <returns>The results of scriptlet processing, or null if the caller expects no
		/// result (that is, the <see cref="ScriptTextFlags.IsExpression"/> value is not set)</returns>
		object ParseScriptText(
			string code,
			string itemName,
			object context,
			string delimiter,
			UIntPtr sourceContextCookie,
			uint startingLineNumber,
			ScriptTextFlags flags
		);

		/// <summary>
		/// Used by a smart host to delegate the <see cref="IDebugDocumentContext.EnumCodeContexts"/> method
		/// </summary>
		/// <param name="sourceContext">The source context as provided to
		/// <see cref="IActiveScriptParse32.ParseScriptText"/> or
		/// <see cref="IActiveScriptParse32.AddScriptlet"/></param>
		/// <param name="offset">Character offset relative to start of script text</param>
		/// <param name="length">Number of characters in this context</param>
		/// <param name="enumContexts">An enumerator of the code contexts in the specified range</param>
		void EnumCodeContextsOfPosition(
			UIntPtr sourceContext,
			uint offset,
			uint length,
			out IEnumDebugCodeContexts enumContexts
		);

		void EnumStackFrames(
			out IEnumDebugStackFrames enumFrames
		);

		/// <summary>
		/// The Active Script host calls this method to start garbage collection
		/// </summary>
		/// <param name="type">The type of garbage collection</param>
		void CollectGarbage(
			ScriptGCType type
		);
	}
}
#endif
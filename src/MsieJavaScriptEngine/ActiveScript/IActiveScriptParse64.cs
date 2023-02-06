#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// If the Active Script engine allows raw text code scriptlets to be added to the script
	/// or allows expression text to be evaluated at run time, it implements the
	/// <see cref="IActiveScriptParse64"/> interface.
	/// </summary>
	/// <remarks>
	/// Before the scripting engine can be used, one of the following methods must be called:
	/// <c>IPersist.Load</c>, <c>IPersist.InitNew</c>, or <see cref="IActiveScriptParse64.InitNew"/>. The semantics of
	/// this method are identical to <c>IPersistStreamInit.InitNew</c>, in that this method tells the scripting
	/// engine to initialize itself. Note that it is not valid to call both <c>IPersist.InitNew</c> or
	/// <see cref="IActiveScriptParse64.InitNew"/> and <c>IPersist.Load</c>, nor is it valid to call
	/// <c>IPersist.InitNew</c>, <see cref="IActiveScriptParse64.InitNew"/>, or <c>IPersist.Load</c> more than once.
	/// </remarks>
	[ComImport]
	[Guid("c7ef7658-e1ee-480e-97ea-d52cb4d76d17")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptParse64
	{
		/// <summary>
		/// Initializes the scripting engine
		/// </summary>
		void InitNew();

		/// <summary>
		/// Adds a code scriptlet to the script. This method is used in environments where the
		/// persistent state of the script is intertwined with the host document and the host
		/// is responsible for restoring the script, rather than through an <c>IPersist</c> interface.
		/// The primary examples are HTML scripting languages that allow scriptlets of code
		/// embedded in the HTML document to be attached to intrinsic events (for instance,
		/// <c>ONCLICK="button1.text='Exit'"</c>).
		/// </summary>
		/// <param name="defaultName">The default name to associate with the scriptlet. If the
		/// scriptlet does not contain naming information (as in the <c>ONCLICK</c> example above),
		/// this name will be used to identify the scriptlet. If this parameter is <c>null</c>, the
		/// scripting engine manufactures a unique name, if necessary.</param>
		/// <param name="code">The scriptlet text to add. The interpretation of this string
		/// depends on the scripting language.</param>
		/// <param name="itemName">The item name associated with this scriptlet. This parameter,
		/// in addition to <paramref name="subItemName"/>, identifies the object for which the
		/// scriptlet is an event handler.</param>
		/// <param name="subItemName">The name of a subobject of the named item with which this
		/// scriptlet is associated; this name must be found in the named item's type
		/// information. This parameter is <c>null</c> if the scriptlet is to be associated with the
		/// named item instead of a subitem. This parameter, in addition to
		/// <paramref name="itemName"/>, identifies the specific object for which the scriptlet
		/// is an event handler.</param>
		/// <param name="eventName">The name of the event for which the scriptlet is an event
		/// handler</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When the <paramref name="code"/>
		/// parameter is parsed from a stream of text, the host typically uses a delimiter, such as
		/// two single quotation marks (''), to detect the end of the scriptlet. This parameter
		/// specifies the delimiter that the host used, allowing the scripting engine to
		/// provide some conditional primitive preprocessing (for example, replacing a single
		/// quotation mark ['] with two single quotation marks for use as a delimiter).
		/// Exactly how (and if) the scripting engine makes use of this information depends
		/// on the scripting engine. Set this parameter to <c>null</c> if the host did not use a
		/// delimiter to mark the end of the scriptlet.</param>
		/// <param name="pSourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <param name="name">Actual name used to identify the scriptlet. This is to be in
		/// order of preference: a name explicitly specified in the scriptlet text, the
		/// default name provided in <paramref name="defaultName"/>, or a unique name
		/// synthesized by the scripting engine.</param>
		/// <param name="exceptionInfo">Exception information. This structure should be
		/// filled in if <c>DISP_E_EXCEPTION</c> is returned</param>
		void AddScriptlet(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string defaultName,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string code,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string itemName,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string subItemName,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string eventName,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string delimiter,
			[In] IntPtr pSourceContextCookie,
			[In] uint startingLineNumber,
			[In] ScriptTextFlags flags,
			[Out] [MarshalAs(UnmanagedType.BStr)] out string name,
			[Out] out EXCEPINFO exceptionInfo
		);

		/// <summary>
		/// Parses the given code scriptlet, adding declarations into the namespace and
		/// evaluating code as appropriate
		/// </summary>
		/// <param name="code">The scriptlet text to evaluate. The interpretation of this
		/// string depends on the scripting language</param>
		/// <param name="itemName">The item name that gives the context in which the
		/// scriptlet is to be evaluated. If this parameter is <c>null</c>, the code is evaluated
		/// in the scripting engine's global context</param>
		/// <param name="context">The context object. This object is reserved for use in a
		/// debugging environment, where such a context may be provided by the debugger to
		/// represent an active run-time context. If this parameter is <c>null</c>, the engine
		/// uses <paramref name="itemName"/> to identify the context.</param>
		/// <param name="delimiter">The end-of-scriptlet delimiter. When <paramref name="code"/>
		/// is parsed from a stream of text, the host typically uses a delimiter, such as two
		/// single quotation marks (''), to detect the end of the scriptlet. This parameter
		/// specifies the delimiter that the host used, allowing the scripting engine to provide
		/// some conditional primitive preprocessing (for example, replacing a single quotation
		/// mark ['] with two single quotation marks for use as a delimiter). Exactly how
		/// (and if) the scripting engine makes use of this information depends on the
		/// scripting engine. Set this parameter to <c>null</c> if the host did not use a delimiter
		/// to mark the end of the scriptlet.</param>
		/// <param name="pSourceContextCookie">Application-defined value that is used for
		/// debugging purposes</param>
		/// <param name="startingLineNumber">Zero-based value that specifies which line the
		/// parsing will begin at</param>
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <param name="result">The results of scriptlet processing, or <c>null</c> if the caller
		/// expects no result (that is, the <see cref="ScriptTextFlags.IsExpression"/> value is
		/// not set)</param>
		/// <param name="exceptionInfo">The exception information. This structure is filled if
		/// <see cref="IActiveScriptParse64.ParseScriptText"/> returns <c>DISP_E_EXCEPTION</c>.</param>
		void ParseScriptText(
			[In] [MarshalAs(UnmanagedType.LPWStr)] string code,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string itemName,
			[In] [MarshalAs(UnmanagedType.IUnknown)] object context,
			[In] [MarshalAs(UnmanagedType.LPWStr)] string delimiter,
			[In] UIntPtr pSourceContextCookie,
			[In] uint startingLineNumber,
			[In] ScriptTextFlags flags,
			[Out] out object result,
			[Out] out EXCEPINFO exceptionInfo
		);
	}
}
#endif
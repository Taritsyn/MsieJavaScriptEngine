namespace MsieJavaScriptEngine.ActiveScript
{
	using System;

	using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

	using Helpers;
	using Resources;

	internal sealed class ActiveScriptParseWrapper : IActiveScriptParseWrapper
	{
		/// <summary>
		/// Flag that the current process is a 64-bit process
		/// </summary>
		private readonly bool _is64Bit;

		/// <summary>
		/// Pointer to an instance of 32-bit ActiveScript parser
		/// </summary>
		private IntPtr _pActiveScriptParse32 = IntPtr.Zero;

		/// <summary>
		/// Instance of 32-bit ActiveScript parser
		/// </summary>
		private IActiveScriptParse32 _activeScriptParse32;

		/// <summary>
		/// Pointer to an instance of 64-bit ActiveScript parser
		/// </summary>
		private IntPtr _pActiveScriptParse64 = IntPtr.Zero;

		/// <summary>
		/// Instance of 64-bit ActiveScript parser
		/// </summary>
		private IActiveScriptParse64 _activeScriptParse64;

		/// <summary>
		/// Last COM exception
		/// </summary>
		private EXCEPINFO _lastException;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private bool _disposed;
		
		/// <summary>
		/// Gets a last COM exception
		/// </summary>
		public EXCEPINFO LastException
		{
			get { return _lastException; }
		}


		/// <summary>
		/// Constructs instance of the <see cref="ActiveScriptParseWrapper"/> class
		/// </summary>
		/// <param name="pActiveScript">Pointer to an instance of native JavaScript engine</param>
		/// <param name="activeScript">Instance of native JavaScript engine.
		/// Must implement IActiveScriptParse32 or IActiveScriptParse64.</param>
		public ActiveScriptParseWrapper(IntPtr pActiveScript, IActiveScript activeScript)
		{
			_is64Bit = Environment.Is64BitProcess;

			if (_is64Bit)
			{
				_pActiveScriptParse64 = ComHelpers.QueryInterface<IActiveScriptParse64>(pActiveScript);
				_activeScriptParse64 = activeScript as IActiveScriptParse64;
			}
			else
			{
				_pActiveScriptParse32 = ComHelpers.QueryInterface<IActiveScriptParse32>(pActiveScript);
				_activeScriptParse32 = activeScript as IActiveScriptParse32;
			}

			if (_activeScriptParse64 == null && _activeScriptParse32 == null)
			{
				throw new NotSupportedException(Strings.Runtime_InvalidParserImplementationError);
			}
		}

		/// <summary>
		/// Destructs instance of <see cref="ActiveScriptSiteWrapper"/>
		/// </summary>
		~ActiveScriptParseWrapper()
		{
			Dispose(false);
		}


		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of 
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (_is64Bit)
				{
					_activeScriptParse64 = null;
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse64);
				}
				else
				{
					_activeScriptParse32 = null;
					ComHelpers.ReleaseAndEmpty(ref _pActiveScriptParse32);
				}
			}
		}

		#region IActiveScriptParseWrapper implementation

		/// <summary>
		/// Initializes the scripting engine
		/// </summary>
		public void InitNew() {
			if (_is64Bit)
			{
				_activeScriptParse64.InitNew();
			}
			else
			{
				_activeScriptParse32.InitNew();
			}
		}

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
		/// depends on the scripting language.</param>
		/// <param name="itemName">The item name associated with this scriptlet. This parameter,
		/// in addition to pstrSubItemName, identifies the object for which the scriptlet is
		/// an event handler.</param>
		/// <param name="subItemName">The name of a subobject of the named item with which this
		/// scriptlet is associated; this name must be found in the named item's type
		/// information. This parameter is NULL if the scriptlet is to be associated with the
		/// named item instead of a subitem. This parameter, in addition to pstrItemName,
		/// identifies the specific object for which the scriptlet is an event handler.</param>
		/// <param name="eventName">The name of the event for which the scriptlet is an event
		/// handler.</param>
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
		/// <param name="flags">Flags associated with the scriptlet</param>
		/// <returns>
		/// Actual name used to identify the scriptlet. This is to be in
		/// order of preference: a name explicitly specified in the scriptlet text, the
		/// default name provided in pstrDefaultName, or a unique name synthesized by the
		/// scripting engine.
		/// </returns>
		public string AddScriptlet(
			string defaultName,
			string code,
			string itemName,
			string subItemName,
			string eventName,
			string delimiter,
			IntPtr sourceContextCookie,
			uint startingLineNumber,
			ScriptTextFlags flags) {

			string name;

			if (_is64Bit)
			{
				_activeScriptParse64.AddScriptlet(
					defaultName,
					code,
					itemName,
					subItemName,
					eventName,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out name,
					out _lastException);
			}
			else
			{
				_activeScriptParse32.AddScriptlet(
					defaultName,
					code,
					itemName,
					subItemName,
					eventName,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out name,
					out _lastException);
			}

			return name;
		}

		/// <summary>
		/// Parses the given code scriptlet, adding declarations into the namespace and
		/// evaluating code as appropriate.
		/// </summary>
		/// <param name="code">The scriptlet text to evaluate. The interpretation of this
		/// string depends on the scripting language</param>
		/// <param name="itemName">The item name that gives the context in which the
		/// scriptlet is to be evaluated. If this parameter is NULL, the code is evaluated
		/// in the scripting engine's global context</param>
		/// <param name="context">The context object. This object is reserved for use in a
		/// debugging environment, where such a context may be provided by the debugger to
		/// represent an active run-time context. If this parameter is NULL, the engine
		/// uses pstrItemName to identify the context.</param>
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
		/// <returns>
		/// The results of scriptlet processing, or NULL if the caller
		/// expects no result (that is, the SCRIPTTEXT_ISEXPRESSION value is not set).
		/// </returns>
		public object ParseScriptText(
			string code,
			string itemName,
			object context,
			string delimiter,
			IntPtr sourceContextCookie,
			uint startingLineNumber,
			ScriptTextFlags flags) {

			object result;

			if (_is64Bit)
			{
				_activeScriptParse64.ParseScriptText(
					code,
					itemName,
					context,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out result,
					out _lastException);
			}
			else
			{
				_activeScriptParse32.ParseScriptText(
					code,
					itemName,
					context,
					delimiter,
					sourceContextCookie,
					startingLineNumber,
					flags,
					out result,
					out _lastException);
			}

			return result;
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
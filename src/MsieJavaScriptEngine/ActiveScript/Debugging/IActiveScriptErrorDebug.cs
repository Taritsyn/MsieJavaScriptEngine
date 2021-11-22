#if NETFRAMEWORK
using System.Runtime.InteropServices;

using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Provides document context information for compile-time errors and run-time exceptions
	/// </summary>
	[ComImport]
	[Guid("51973c12-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptErrorDebug // : IActiveScriptError
	{
		#region IActiveScriptError methods

		/// <summary>
		/// Retrieves information about an error that occurred while the scripting engine was running a script
		/// </summary>
		/// <param name="exceptionInfo">An EXCEPINFO structure that receives error information</param>
		void GetExceptionInfo(
			[Out] out EXCEPINFO exceptionInfo
		);

		/// <summary>
		/// Retrieves the location in the source code where an error occurred while the scripting engine
		/// was running a script
		/// </summary>
		/// <param name="sourceContext">A cookie that identifies the context. The interpretation of
		/// this parameter depends on the host application.</param>
		/// <param name="lineNumber">The line number in the source file where the error occurred</param>
		/// <param name="characterPosition">The character position in the line where the error occurred</param>
		void GetSourcePosition(
			[Out] out uint sourceContext,
			[Out] out uint lineNumber,
			[Out] out int characterPosition
		);

		/// <summary>
		/// Retrieves the line in the source file where an error occurred while a scripting engine
		/// was running a script
		/// </summary>
		/// <param name="sourceLine">The line of source code in which the error occurred</param>
		void GetSourceLineText(
			[Out] [MarshalAs(UnmanagedType.BStr)] out string sourceLine
		);

		#endregion
	}
}
#endif
#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Describes how to continue from a runtime error
	/// </summary>
	internal enum ErrorResumeAction
	{
		/// <summary>
		/// Re-executes the statement that produced the error
		/// </summary>
		ReexecuteErrorStatement,

		/// <summary>
		/// Lets the language engine handle the error
		/// </summary>
		AbortCallAndReturnErrorToCaller,

		/// <summary>
		/// Resumes execution in the code following the statement that produced the error
		/// </summary>
		SkipErrorStatement
	}
}
#endif
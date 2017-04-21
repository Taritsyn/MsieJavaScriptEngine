#if !NETSTANDARD1_3
using System;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Describe the attributes of the document
	/// </summary>
	[Flags]
	internal enum TextDocAttrs : uint
	{
		None = 0,

		/// <summary>
		/// The document is read-only
		/// </summary>
		ReadOnly = 0x00000001,

		/// <summary>
		/// The document is the primary file of this document tree
		/// </summary>
		TypePrimary = 0x00000002,

		/// <summary>
		/// The document is a worker
		/// </summary>
		TypeWorker = 0x00000004,

		/// <summary>
		/// The document is a script file
		/// </summary>
		TypeScript = 0x00000008
	}
}
#endif
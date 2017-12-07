#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Describes which type to get for a document
	/// </summary>
	internal enum DocumentNameType
	{
		/// <summary>
		/// Gets the name as it appears in the application tree
		/// </summary>
		AppNode,

		/// <summary>
		/// Gets the name as it appears on the viewer title bar
		/// </summary>
		Title,

		/// <summary>
		/// Gets the file name without a path
		/// </summary>
		FileTail,

		/// <summary>
		/// Gets the URL of the document
		/// </summary>
		Url,

		/// <summary>
		/// Gets the title appended with enumeration for identification
		/// </summary>
		UniqueTitle
	}
}
#endif
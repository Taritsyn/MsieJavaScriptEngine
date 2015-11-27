namespace MsieJavaScriptEngine.JsRt.Ie
{
	using System.Runtime.InteropServices;

	/// <summary>
	/// IProcessDebugManager32 COM interface
	/// </summary>
	[Guid("51973C2f-CB0C-11d0-B5C9-00A0244A0E7A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IProcessDebugManager32
	{
		/// <summary>
		/// Creates a new debug application
		/// </summary>
		/// <param name="debugApplication">The new debug application</param>
		void CreateApplication(out IDebugApplication32 debugApplication);

		/// <summary>
		/// Gets a default debug application
		/// </summary>
		/// <param name="debugApplication">The default debug application</param>
		void GetDefaultApplication(out IDebugApplication32 debugApplication);

		/// <summary>
		/// Adds a new debug application
		/// </summary>
		/// <param name="debugApplication">The new debug application</param>
		/// <param name="cookie">An engine-defined cookie</param>
		void AddApplication(IDebugApplication32 debugApplication, out uint cookie);

		/// <summary>
		/// Removes a debug application
		/// </summary>
		/// <param name="cookie">The cookie of the debug application to remove</param>
		void RemoveApplication(uint cookie);

		/// <summary>
		/// Creates a debug document helper
		/// </summary>
		/// <param name="outerUnknown">The outer unknown</param>
		/// <param name="helper">The new debug document helper</param>
		void CreateDebugDocumentHelper(object outerUnknown, out IDebugDocumentHelper32 helper);
	}
}
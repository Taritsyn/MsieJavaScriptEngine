namespace MsieJavaScriptEngine.JsRt.Ie
{
	using System.Runtime.InteropServices;

	/// <summary>
	/// IProcessDebugManager64 COM interface
	/// </summary>
	[Guid("56b9fC1C-63A9-4CC1-AC21-087D69A17FAB")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IProcessDebugManager64
	{
		/// <summary>
		/// Creates a new debug application
		/// </summary>
		/// <param name="debugApplication">The new debug application</param>
		void CreateApplication(out IDebugApplication64 debugApplication);

		/// <summary>
		/// Gets a default debug application
		/// </summary>
		/// <param name="debugApplication">The default debug application</param>
		void GetDefaultApplication(out IDebugApplication64 debugApplication);

		/// <summary>
		/// Adds a new debug application
		/// </summary>
		/// <param name="debugApplication">The new debug application</param>
		/// <param name="cookie">An engine-defined cookie</param>
		void AddApplication(IDebugApplication64 debugApplication, out uint cookie);

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
		void CreateDebugDocumentHelper(object outerUnknown, out IDebugDocumentHelper64 helper);
	}
}
#if !NETSTANDARD
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Enumerates the code contexts that correspond to a document context
	/// </summary>
	[ComImport]
	[Guid("51973c1d-cb0c-11d0-b5c9-00a0244a0e7a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumDebugCodeContexts
	{
		/// <summary>
		/// Retrieves a specified number of segments in the enumeration sequence
		/// </summary>
		/// <param name="count">The number of segments to retrieve</param>
		/// <param name="contexts">Returns an array of <see cref="IDebugCodeContext"/> interfaces that
		/// represents the segments being retrieved</param>
		/// <param name="countFetched">The actual number of segments fetched by the enumerator</param>
		void Next(
			[In] uint count,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IDebugCodeContext[] contexts,
			[Out] out uint countFetched
		);

		/// <summary>
		/// Skips a specified number of segments in an enumeration sequence
		/// </summary>
		/// <param name="count">Number of segments in the enumeration sequence to skip</param>
		void Skip(
			[In] uint count
		);

		/// <summary>
		/// Resets an enumeration sequence to the beginning
		/// </summary>
		void Reset();

		/// <summary>
		/// Creates an enumerator that contains the same state as the current enumerator
		/// </summary>
		/// <param name="enumContexts">Returns the <see cref="IEnumDebugCodeContexts"/> interface of
		/// the clone of the enumerator</param>
		void Clone(
			[Out] [MarshalAs(UnmanagedType.Interface)] out IEnumDebugCodeContexts enumContexts
		);
	}
}
#endif
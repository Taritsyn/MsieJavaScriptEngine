#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Information about line of source code
	/// </summary>
	internal struct DebugLineInfo
	{
		/// <summary>
		/// Gets a line number
		/// </summary>
		public readonly uint Number;

		/// <summary>
		/// Gets a position of line
		/// </summary>
		public readonly uint Position;

		/// <summary>
		/// Gets a length of line
		/// </summary>
		public readonly uint Length;

		/// <summary>
		/// Gets a length of line break
		/// </summary>
		public readonly uint BreakLength;


		/// <summary>
		/// Constructs an instance of the information about line of source code
		/// </summary>
		/// <param name="number">Line number</param>
		/// <param name="position">Position of line</param>
		/// <param name="length">Length of line</param>
		/// <param name="breakLength">Length of line break</param>
		public DebugLineInfo(uint number, uint position, uint length, uint breakLength)
		{
			Number = number;
			Position = position;
			Length = length;
			BreakLength = breakLength;
		}
	}
}
#endif
#if !NETSTANDARD1_3
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Debug document context
	/// </summary>
	internal sealed class DebugDocumentContext : IDebugDocumentContext
	{
		/// <summary>
		/// Debug document
		/// </summary>
		private readonly DebugDocument _document;

		/// <summary>
		/// Position
		/// </summary>
		private readonly uint _position;

		/// <summary>
		/// Length
		/// </summary>
		private readonly uint _length;

		/// <summary>
		/// Code context enumerator
		/// </summary>
		private readonly IEnumDebugCodeContexts _enumCodeContexts;

		/// <summary>
		/// Gets a position
		/// </summary>
		public uint Position
		{
			get { return _position; }
		}

		/// <summary>
		/// Gets a length
		/// </summary>
		public uint Length
		{
			get { return _length; }
		}


		/// <summary>
		/// Constructs an instance of the debug document context
		/// </summary>
		/// <param name="document">Debug document</param>
		/// <param name="position">Position</param>
		/// <param name="length">Length</param>
		/// <param name="enumCodeContexts">Code context enumerator</param>
		public DebugDocumentContext(DebugDocument document, uint position, uint length,
			IEnumDebugCodeContexts enumCodeContexts)
		{
			_document = document;
			_position = position;
			_length = length;
			_enumCodeContexts = enumCodeContexts;
		}


		#region IDebugDocumentContext implementation

		public void GetDocument(out IDebugDocument debugDocument)
		{
			debugDocument = _document;
		}

		public void EnumCodeContexts(out IEnumDebugCodeContexts enumContexts)
		{
			_enumCodeContexts.Clone(out enumContexts);
		}

		#endregion
	}
}
#endif
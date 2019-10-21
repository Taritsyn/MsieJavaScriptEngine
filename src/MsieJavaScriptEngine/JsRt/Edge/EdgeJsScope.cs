using System;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” scope automatically sets a context to current and resets the original context
	/// when disposed
	/// </summary>
	internal struct EdgeJsScope : IDisposable
	{
		/// <summary>
		/// The previous context
		/// </summary>
		private readonly EdgeJsContext _previousContext;

		/// <summary>
		/// Whether the structure has been disposed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Initializes a new instance of the <see cref="EdgeJsScope"/> struct
		/// </summary>
		/// <param name="context">The context to create the scope for</param>
		public EdgeJsScope(EdgeJsContext context)
		{
			_disposed = false;
			_previousContext = EdgeJsContext.Current;
			EdgeJsContext.Current = context;
		}

		#region IDisposable implementation

		/// <summary>
		/// Disposes the scope and sets the previous context to current
		/// </summary>
		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			EdgeJsContext.Current = _previousContext;
			_disposed = true;
		}

		#endregion
	}
}
using System;

namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// “IE” scope automatically sets a context to current and resets the original context
	/// when disposed
	/// </summary>
	internal struct IeJsScope : IDisposable
	{
		/// <summary>
		/// The previous context
		/// </summary>
		private readonly IeJsContext _previousContext;

		/// <summary>
		/// Whether the structure has been disposed
		/// </summary>
		private bool _disposed;


		/// <summary>
		/// Initializes a new instance of the <see cref="IeJsScope"/> struct
		/// </summary>
		/// <param name="context">The context to create the scope for</param>
		public IeJsScope(IeJsContext context)
		{
			_disposed = false;
			_previousContext = IeJsContext.Current;
			IeJsContext.Current = context;
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

			IeJsContext.Current = _previousContext;
			_disposed = true;
		}

		#endregion
	}
}
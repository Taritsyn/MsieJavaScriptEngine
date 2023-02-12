using System;

using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Base class of the inner JS engine
	/// </summary>
	internal abstract class InnerJsEngineBase : IInnerJsEngine
	{
		/// <summary>
		/// JS engine settings
		/// </summary>
		protected JsEngineSettings _settings;

		/// <summary>
		/// Name of JS engine mode
		/// </summary>
		protected readonly string _engineModeName;

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		protected StatedFlag _disposedFlag = new StatedFlag();


		/// <summary>
		/// Constructs an instance of the inner JS engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		protected InnerJsEngineBase(JsEngineSettings settings)
		{
			_settings = settings;
			_engineModeName = JsEngineModeHelpers.GetModeName(_settings.EngineMode);
		}


		#region IInnerJsEngine implementation

		/// <inheritdoc/>
		public string Mode
		{
			get { return _engineModeName; }
		}

		/// <inheritdoc/>
		public abstract bool SupportsScriptPrecompilation { get; }


		/// <inheritdoc/>
		public abstract PrecompiledScript Precompile(string code, string documentName);

		/// <inheritdoc/>
		public abstract object Evaluate(string expression, string documentName);

		/// <inheritdoc/>
		public abstract void Execute(string code, string documentName);

		/// <inheritdoc/>
		public abstract void Execute(PrecompiledScript precompiledScript);

		/// <inheritdoc/>
		public abstract object CallFunction(string functionName, params object[] args);

		/// <inheritdoc/>
		public abstract bool HasVariable(string variableName);

		/// <inheritdoc/>
		public abstract object GetVariableValue(string variableName);

		/// <inheritdoc/>
		public abstract void SetVariableValue(string variableName, object value);

		/// <inheritdoc/>
		public abstract void RemoveVariable(string variableName);

		/// <inheritdoc/>
		public abstract void EmbedHostObject(string itemName, object value);

		/// <inheritdoc/>
		public abstract void EmbedHostType(string itemName, Type type);

		/// <inheritdoc/>
		public abstract void Interrupt();

		/// <inheritdoc/>
		public abstract void CollectGarbage();

		#endregion

		#region IDisposable implementation

		public abstract void Dispose();

		#endregion
	}
}
using System;

using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Base class of the inner JS engine
	/// </summary>
	internal abstract class InnerJsEngineBase : IInnerJsEngine
	{
		/// <summary>
		/// JS engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

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
		/// <param name="engineMode">JS engine mode</param>
		protected InnerJsEngineBase(JsEngineMode engineMode)
		{
			_engineMode = engineMode;
			_engineModeName = JsEngineModeHelpers.GetModeName(engineMode);
		}


		#region IInnerJsEngine implementation

		public abstract string Mode { get; }


		public abstract object Evaluate(string expression, string documentName);

		public abstract void Execute(string code, string documentName);

		public abstract object CallFunction(string functionName, params object[] args);

		public abstract bool HasVariable(string variableName);

		public abstract object GetVariableValue(string variableName);

		public abstract void SetVariableValue(string variableName, object value);

		public abstract void RemoveVariable(string variableName);

		public abstract void EmbedHostObject(string itemName, object value);

		public abstract void EmbedHostType(string itemName, Type type);

		public abstract void Interrupt();

		public abstract void CollectGarbage();

		#endregion

		#region IDisposable implementation

		public abstract void Dispose();

		#endregion
	}
}
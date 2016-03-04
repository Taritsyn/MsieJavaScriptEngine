namespace MsieJavaScriptEngine.JsRt
{
	using System;

	using Helpers;

	/// <summary>
	/// Base class of the Chakra JsRT JavaScript engine
	/// </summary>
	internal abstract class ChakraJsRtJsEngineBase : IInnerJsEngine
	{
		/// <summary>
		/// JavaScript engine mode
		/// </summary>
		protected readonly JsEngineMode _engineMode;

		/// <summary>
		/// Name of JavaScript engine mode
		/// </summary>
		protected readonly string _engineModeName;

		/// <summary>
		/// Flag for whether to enable script debugging features
		/// </summary>
		protected readonly bool _enableDebugging;

		/// <summary>
		/// Flag indicating whether debugging started
		/// </summary>
		private StatedFlag _debuggingStartedFlag;


		/// <summary>
		/// Constructs instance of the Chakra JsRT JavaScript engine
		/// </summary>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="enableDebugging">Flag for whether to enable script debugging features</param>
		protected ChakraJsRtJsEngineBase(JsEngineMode engineMode, bool enableDebugging)
		{
			_engineMode = engineMode;
			_engineModeName = JsEngineModeHelpers.GetModeName(engineMode);
			_enableDebugging = enableDebugging;
		}


		/// <summary>
		/// Starts debugging
		/// </summary>
		protected void StartDebugging()
		{
			if (_debuggingStartedFlag.Set())
			{
				InnerStartDebugging();
			}
		}

		protected abstract void InnerStartDebugging();

		#region IInnerJsEngine implementation

		public abstract string Mode { get; }


		public abstract object Evaluate(string expression);

		public abstract void Execute(string code);

		public abstract object CallFunction(string functionName, params object[] args);

		public abstract bool HasVariable(string variableName);

		public abstract object GetVariableValue(string variableName);

		public abstract void SetVariableValue(string variableName, object value);

		public abstract void RemoveVariable(string variableName);

		public abstract void EmbedHostObject(string itemName, object value);

		public abstract void EmbedHostType(string itemName, Type type);

		#endregion

		#region IDisposable implementation

		public abstract void Dispose();

		#endregion
	}
}
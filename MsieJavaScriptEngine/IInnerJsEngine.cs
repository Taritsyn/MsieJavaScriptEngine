namespace MsieJavaScriptEngine
{
	using System;

	/// <summary>
	/// Interface for the inner JavaScript engine
	/// </summary>
	internal interface IInnerJsEngine : IDisposable
	{
		/// <summary>
		/// Gets a name of JavaScript engine mode
		/// </summary>
		string Mode { get; }


		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JavaScript expression</param>
		/// <returns>Result of the expression</returns>
		object Evaluate(string expression);

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JavaScript code</param>
		void Execute(string code);

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		object CallFunction(string functionName, params object[] args);

		/// <summary>
		/// Сhecks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Result of check (true - exists; false - not exists</returns>
		bool HasVariable(string variableName);

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		object GetVariableValue(string variableName);

		/// <summary>
		/// Sets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="value">Value of variable</param>
		void SetVariableValue(string variableName, object value);

		/// <summary>
		/// Removes a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		void RemoveVariable(string variableName);
	}
}
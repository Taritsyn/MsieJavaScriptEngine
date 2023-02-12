using System;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Interface for the inner JS engine
	/// </summary>
	internal interface IInnerJsEngine : IDisposable
	{
		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		string Mode { get; }

		/// <summary>
		/// Gets a value that indicates if the JS engine supports script pre-compilation
		/// </summary>
		bool SupportsScriptPrecompilation { get; }


		/// <summary>
		/// Creates a pre-compiled script from JS code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <param name="documentName">Document name</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		PrecompiledScript Precompile(string code, string documentName);

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS expression</param>
		/// <param name="documentName">Document name</param>
		/// <returns>Result of the expression</returns>
		object Evaluate(string expression, string documentName);

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <param name="documentName">Document name</param>
		void Execute(string code, string documentName);

		/// <summary>
		/// Executes a pre-compiled script
		/// </summary>
		/// <param name="precompiledScript">A pre-compiled script that can be executed by different
		/// instances of JS engine</param>
		void Execute(PrecompiledScript precompiledScript);

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		object CallFunction(string functionName, params object[] args);

		/// <summary>
		/// Checks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Result of check (<c>true</c> - exists; <c>false</c> - not exists</returns>
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

		/// <summary>
		/// Embeds a host object to script code
		/// </summary>
		/// <remarks>
		/// Allows to embed instances of simple classes (or structures) and delegates.
		/// </remarks>
		/// <param name="itemName">The name for the new global variable or function that will represent the object</param>
		/// <param name="value">The object to expose</param>
		void EmbedHostObject(string itemName, object value);

		/// <summary>
		/// Embeds a host type to script code
		/// </summary>
		/// <remarks>
		/// Host types are exposed to script code in the form of objects whose properties and
		/// methods are bound to the type's static members.
		/// </remarks>
		/// <param name="itemName">The name for the new global variable that will represent the type</param>
		/// <param name="type">The type to expose</param>
		void EmbedHostType(string itemName, Type type);

		/// <summary>
		/// Interrupts script execution and causes the JS engine to throw an exception
		/// </summary>
		void Interrupt();

		/// <summary>
		/// Performs a full garbage collection
		/// </summary>
		void CollectGarbage();
	}
}
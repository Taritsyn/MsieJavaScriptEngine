using System;
using System.Collections.Generic;

namespace MsieJavaScriptEngine.JsRt.Embedding
{
	/// <summary>
	/// Embedded type
	/// </summary>
	/// <typeparam name="TValue">The type of the JavaScript value</typeparam>
	/// <typeparam name="TFunction">The type of the native function</typeparam>
	internal sealed class EmbeddedType<TValue, TFunction> : EmbeddedItem<TValue, TFunction>
		where TValue : struct
		where TFunction : Delegate
	{
		/// <summary>
		/// Constructs an instance of the embedded type
		/// </summary>
		/// <param name="hostType">Host type</param>
		/// <param name="scriptValue">JavaScript value created from an host type</param>
		public EmbeddedType(Type hostType, TValue scriptValue)
			: base(hostType, null, scriptValue, new List<TFunction>())
		{ }

		/// <summary>
		/// Constructs an instance of the embedded type
		/// </summary>
		/// <param name="hostType">Host type</param>
		/// <param name="scriptValue">JavaScript value created from an host type</param>
		/// <param name="nativeFunctions">List of native functions, that used to access to members of type</param>
		public EmbeddedType(Type hostType, TValue scriptValue, IList<TFunction> nativeFunctions)
			: base(hostType, null, scriptValue, nativeFunctions)
		{ }

		#region EmbeddedItem overrides

		/// <summary>
		/// Gets a value that indicates if the host item is an instance
		/// </summary>
		public override bool IsInstance
		{
			get { return false; }
		}

		#endregion
	}
}
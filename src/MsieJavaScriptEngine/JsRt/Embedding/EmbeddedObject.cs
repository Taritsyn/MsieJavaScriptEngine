using System;
using System.Collections.Generic;

namespace MsieJavaScriptEngine.JsRt.Embedding
{
	/// <summary>
	/// Embedded object
	/// </summary>
	/// <typeparam name="TValue">The type of the JavaScript value</typeparam>
	/// <typeparam name="TFunction">The type of the native function</typeparam>
	internal sealed class EmbeddedObject<TValue, TFunction> : EmbeddedItem<TValue, TFunction>
		where TValue : struct
		where TFunction : Delegate
	{
		/// <summary>
		/// Constructs an instance of the embedded object
		/// </summary>
		/// <param name="hostObject">Instance of host type</param>
		/// <param name="scriptValue">JavaScript value created from an host object</param>
		public EmbeddedObject(object hostObject, TValue scriptValue)
			: base(hostObject.GetType(), hostObject, scriptValue, new List<TFunction>())
		{ }

		/// <summary>
		/// Constructs an instance of the embedded object
		/// </summary>
		/// <param name="hostObject">Instance of host type</param>
		/// <param name="scriptValue">JavaScript value created from an host object</param>
		/// <param name="nativeFunctions">List of native functions, that used to access to members of host object</param>
		public EmbeddedObject(object hostObject, TValue scriptValue, IList<TFunction> nativeFunctions)
			: base(hostObject.GetType(), hostObject, scriptValue, nativeFunctions)
		{ }

		#region EmbeddedItem overrides

		/// <summary>
		/// Gets a value that indicates if the host item is an instance
		/// </summary>
		public override bool IsInstance
		{
			get { return true; }
		}

		#endregion
	}
}
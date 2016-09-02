namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// Extensions for the “Edge” JavaScript value
	/// </summary>
	internal static class EdgeJsValueExtensions
	{
		/// <summary>
		/// Gets a property descriptor for an object's own property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="propertyName">The name of the property</param>
		/// <returns>The property descriptor</returns>
		public static EdgeJsValue GetOwnPropertyDescriptor(this EdgeJsValue source, string propertyName)
		{
			EdgeJsPropertyId propertyId = EdgeJsPropertyId.FromString(propertyName);
			EdgeJsValue resultValue = source.GetOwnPropertyDescriptor(propertyId);

			return resultValue;
		}

		/// <summary>
		/// Determines whether an object has a property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="propertyName">The name of the property</param>
		/// <returns>Whether the object (or a prototype) has the property</returns>
		public static bool HasProperty(this EdgeJsValue source, string propertyName)
		{
			EdgeJsPropertyId propertyId = EdgeJsPropertyId.FromString(propertyName);
			bool result = source.HasProperty(propertyId);

			return result;
		}

		/// <summary>
		/// Gets an object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="name">The name of the property</param>
		/// <returns>The value of the property</returns>
		public static EdgeJsValue GetProperty(this EdgeJsValue source, string name)
		{
			EdgeJsPropertyId id = EdgeJsPropertyId.FromString(name);
			EdgeJsValue resultValue = source.GetProperty(id);

			return resultValue;
		}

		/// <summary>
		/// Sets an object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="name">The name of the property</param>
		/// <param name="value">The new value of the property</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules</param>
		public static void SetProperty(this EdgeJsValue source, string name, EdgeJsValue value, bool useStrictRules)
		{
			EdgeJsPropertyId id = EdgeJsPropertyId.FromString(name);
			source.SetProperty(id, value, useStrictRules);
		}

		/// <summary>
		/// Deletes an object's property
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="propertyName">The name of the property</param>
		/// <param name="useStrictRules">The property set should follow strict mode rules</param>
		/// <returns>Whether the property was deleted</returns>
		public static EdgeJsValue DeleteProperty(this EdgeJsValue source, string propertyName, bool useStrictRules)
		{
			EdgeJsPropertyId propertyId = EdgeJsPropertyId.FromString(propertyName);
			EdgeJsValue resultValue = source.DeleteProperty(propertyId, useStrictRules);

			return resultValue;
		}

		/// <summary>
		/// Defines a new object's own property from a property descriptor
		/// </summary>
		/// <remarks>
		/// Requires an active script context.
		/// </remarks>
		/// <param name="source">The JavaScript value</param>
		/// <param name="propertyName">The name of the property</param>
		/// <param name="propertyDescriptor">The property descriptor</param>
		/// <returns>Whether the property was defined</returns>
		public static bool DefineProperty(this EdgeJsValue source, string propertyName, EdgeJsValue propertyDescriptor)
		{
			EdgeJsPropertyId propertyId = EdgeJsPropertyId.FromString(propertyName);
			bool result = source.DefineProperty(propertyId, propertyDescriptor);

			return result;
		}
	}
}
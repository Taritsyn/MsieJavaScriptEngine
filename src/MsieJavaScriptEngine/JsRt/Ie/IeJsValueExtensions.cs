namespace MsieJavaScriptEngine.JsRt.Ie
{
	/// <summary>
	/// Extensions for the “IE” JavaScript value
	/// </summary>
	internal static class IeJsValueExtensions
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
		public static IeJsValue GetOwnPropertyDescriptor(this IeJsValue source, string propertyName)
		{
			IeJsPropertyId propertyId = IeJsPropertyId.FromString(propertyName);
			IeJsValue resultValue = source.GetOwnPropertyDescriptor(propertyId);

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
		public static bool HasProperty(this IeJsValue source, string propertyName)
		{
			IeJsPropertyId propertyId = IeJsPropertyId.FromString(propertyName);
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
		public static IeJsValue GetProperty(this IeJsValue source, string name)
		{
			IeJsPropertyId id = IeJsPropertyId.FromString(name);
			IeJsValue resultValue = source.GetProperty(id);

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
		public static void SetProperty(this IeJsValue source, string name, IeJsValue value, bool useStrictRules)
		{
			IeJsPropertyId id = IeJsPropertyId.FromString(name);
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
		public static IeJsValue DeleteProperty(this IeJsValue source, string propertyName, bool useStrictRules)
		{
			IeJsPropertyId propertyId = IeJsPropertyId.FromString(propertyName);
			IeJsValue resultValue = source.DeleteProperty(propertyId, useStrictRules);

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
		public static bool DefineProperty(this IeJsValue source, string propertyName, IeJsValue propertyDescriptor)
		{
			IeJsPropertyId propertyId = IeJsPropertyId.FromString(propertyName);
			bool result = source.DefineProperty(propertyId, propertyDescriptor);

			return result;
		}
	}
}
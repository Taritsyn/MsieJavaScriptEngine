namespace MsieJavaScriptEngine
{
	using System;
	using System.Web.Script.Serialization;

	using Helpers;
	using Resources;

	/// <summary>
	/// JavaScript type converter
	/// </summary>
	public static class JsTypeConverter
	{
		/// <summary>
		/// JavaScript serializer
		/// </summary>
		private static readonly JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();

		/// <summary>
		/// Converts a value to JSON string
		/// </summary>
		/// <param name="value">The value to serialize</param>
		/// <returns>The serialized JSON string</returns>
		internal static string Serialize(object value)
		{
			if (value is Undefined)
			{
				return "undefined";
			}

			return _jsSerializer.Serialize(value);
		}

		/// <summary>
		/// Converts the specified value to the specified type
		/// </summary>
		/// <typeparam name="T">The type to convert the value to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The value that has been converted to the target type</returns>
		public static T ConvertToType<T>(object value)
		{
			return (T)ConvertToType(value, typeof(T));
		}

		/// <summary>
		/// Converts the specified value to the specified type
		/// </summary>
		/// <param name="value">The value to convert</param>
		/// <param name="targetType">The type to convert the value to</param>
		/// <returns>The value that has been converted to the target type</returns>
		public static object ConvertToType(object value, Type targetType)
		{
			object result;

			if (ValidationHelpers.IsSupportedType(targetType))
			{
				result = _jsSerializer.ConvertToType(value, targetType);
			}
			else
			{
				throw new ArgumentException(
					string.Format(Strings.Runtime_TypeUnsupported, targetType), "value");
			}

			return result;
		}
	}
}
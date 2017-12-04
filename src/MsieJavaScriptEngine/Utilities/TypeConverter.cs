﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
#if NET45 || NETSTANDARD1_3
using System.Reflection;
#endif
using OriginalTypeConverter = System.ComponentModel.TypeConverter;

using MsieJavaScriptEngine.Resources;

namespace MsieJavaScriptEngine.Utilities
{
	/// <summary>
	/// Type converter
	/// </summary>
	public static class TypeConverter
	{
		/// <summary>
		/// List of primitive type codes
		/// </summary>
		private static readonly TypeCode[] _primitiveTypeCodes =
		{
			TypeCode.Boolean,
			TypeCode.SByte, TypeCode.Byte,
			TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64,
			TypeCode.Single, TypeCode.Double, TypeCode.Decimal,
			TypeCode.Char, TypeCode.String
		};

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
			ConvertObjectToType(value, targetType, true, out result);

			return result;
		}

		/// <summary>
		/// Converts the specified value to the specified type.
		/// A return value indicates whether the conversion succeeded.
		/// </summary>
		/// <typeparam name="T">The type to convert the value to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <param name="convertedValue">The value that has been converted to the target type</param>
		/// <returns>Result of conversion (true - success; false - failure)</returns>
		public static bool TryConvertToType<T>(object value, out T convertedValue)
		{
			object resultValue;

			bool result = TryConvertToType(value, typeof(T), out resultValue);
			convertedValue = (T)resultValue;

			return result;
		}

		/// <summary>
		/// Converts the specified value to the specified type.
		/// A return value indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="value">The value to convert</param>
		/// <param name="targetType">The type to convert the value to</param>
		/// <param name="convertedValue">The value that has been converted to the target type</param>
		/// <returns>Result of conversion (true - success; false - failure)</returns>
		public static bool TryConvertToType(object value, Type targetType, out object convertedValue)
		{
			bool result = ConvertObjectToType(value, targetType, false, out convertedValue);

			return result;
		}

		/// <summary>
		/// Checks whether .NET type is primitive
		/// </summary>
		/// <param name="type">.NET type</param>
		/// <returns>Result of check (true - is primitive; false - is not primitive)</returns>
		internal static bool IsPrimitiveType(Type type)
		{
			TypeCode typeCode = type.GetTypeCode();
			bool result = IsPrimitiveType(typeCode);

			return result;
		}

		/// <summary>
		/// Checks whether .NET type is primitive
		/// </summary>
		/// <param name="typeCode">.NET type code</param>
		/// <returns>Result of check (true - is primitive; false - is not primitive)</returns>
		internal static bool IsPrimitiveType(TypeCode typeCode)
		{
			bool result = _primitiveTypeCodes.Contains(typeCode);

			return result;
		}

		private static bool ConvertObjectToType(object obj, Type type, bool throwOnError,
			out object convertedObject)
		{
			if (obj == null)
			{
				if (IsNonNullableValueType(type))
				{
					if (throwOnError)
					{
						throw new InvalidOperationException(CommonStrings.Common_ValueTypeCannotBeNull);
					}

					convertedObject = null;
					return false;
				}

				convertedObject = null;
				return true;
			}

			if (type != null && obj.GetType() != type)
			{
				return InnerConvertObjectToType(obj, type, throwOnError, out convertedObject);
			}

			convertedObject = obj;
			return true;
		}

		private static bool InnerConvertObjectToType(object obj, Type type, bool throwOnError,
			out object convertedObject)
		{
			Type originalType = obj.GetType();
			OriginalTypeConverter converter = TypeDescriptor.GetConverter(type);

			if (converter.CanConvertFrom(originalType))
			{
				try
				{
					convertedObject = converter.ConvertFrom(null, CultureInfo.InvariantCulture, obj);
					return true;
				}
				catch
				{
					if (throwOnError)
					{
						throw;
					}

					convertedObject = null;
					return false;
				}
			}

			if (converter.CanConvertFrom(typeof(string)) && !(obj is DateTime))
			{
				try
				{
					string text = TypeDescriptor.GetConverter(originalType).ConvertToInvariantString(obj);

					convertedObject = converter.ConvertFromInvariantString(text);
					return true;
				}
				catch
				{
					if (throwOnError)
					{
						throw;
					}

					convertedObject = null;
					return false;
				}
			}

			if (type.GetTypeInfo().IsInstanceOfType(obj))
			{
				convertedObject = obj;
				return true;
			}

			if (throwOnError)
			{
				throw new InvalidOperationException(
					string.Format(CommonStrings.Common_CannotConvertObjectToType, originalType, type)
				);
			}

			convertedObject = null;
			return false;
		}

		private static bool IsNonNullableValueType(Type type)
		{
			if (type == null)
			{
				return false;
			}

			TypeInfo typeInfo = type.GetTypeInfo();
			if (!typeInfo.IsValueType)
			{
				return false;
			}

			if (typeInfo.IsGenericType)
			{
				return type.GetGenericTypeDefinition() != typeof(Nullable<>);
			}

			return true;
		}
	}
}
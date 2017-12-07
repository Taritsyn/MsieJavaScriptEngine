#if !NETSTANDARD
using System;
using System.Linq;

using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.Helpers
{
	/// <summary>
	/// Type mapping helpers
	/// </summary>
	internal static class TypeMappingHelpers
	{
		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <returns>The mapped value</returns>
		public static object MapToScriptType(object value, JsEngineMode engineMode)
		{
			if (value == null)
			{
				return DBNull.Value;
			}

			if (value is Undefined)
			{
				return null;
			}

			if (TypeConverter.IsPrimitiveType(value.GetType()))
			{
				return value;
			}

			var result = new HostObject(value, engineMode);

			return result;
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <returns>The mapped array</returns>
		public static object[] MapToScriptType(object[] args, JsEngineMode engineMode)
		{
			return args.Select(arg => MapToScriptType(arg, engineMode)).ToArray();
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public static object MapToHostType(object value)
		{
			if (value == null)
			{
				return Undefined.Value;
			}

			if (value is DBNull)
			{
				return null;
			}

			if (TypeConverter.IsPrimitiveType(value.GetType()))
			{
				return value;
			}

			var hostObj = value as HostObject;
			object result = hostObj != null ? hostObj.Target : value;

			return result;
		}

		/// <summary>
		/// Makes a mapping of array itemp from the script type to a host type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <returns>The mapped array</returns>
		public static object[] MapToHostType(object[] args)
		{
			return args.Select(MapToHostType).ToArray();
		}
	}
}
#endif
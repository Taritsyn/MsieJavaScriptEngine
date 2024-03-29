﻿#if NETFRAMEWORK
using System;
using System.Linq;

using MsieJavaScriptEngine.ActiveScript;
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
		/// <param name="allowReflection">Flag for whether to allow the usage of reflection API in the script code</param>
		/// <returns>The mapped value</returns>
		public static object MapToScriptType(object value, JsEngineMode engineMode, bool allowReflection)
		{
			if (value == null)
			{
				return DBNull.Value;
			}

			if (value is Undefined)
			{
				return null;
			}

			Type type = value.GetType();
			if (TypeConverter.IsPrimitiveType(type) || type.FullName == "System.__ComObject")
			{
				return value;
			}

			var result = new HostObject(value, engineMode, allowReflection);

			return result;
		}

		/// <summary>
		/// Makes a mapping of array items from the host type to a script type
		/// </summary>
		/// <param name="args">The source array</param>
		/// <param name="engineMode">JavaScript engine mode</param>
		/// <param name="allowReflection">Flag for whether to allow the usage of reflection API in the script code</param>
		/// <returns>The mapped array</returns>
		public static object[] MapToScriptType(object[] args, JsEngineMode engineMode, bool allowReflection)
		{
			return args.Select(arg => MapToScriptType(arg, engineMode, allowReflection)).ToArray();
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
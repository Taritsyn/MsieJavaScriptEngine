#if NET40
using System;

using MsieJavaScriptEngine.Polyfills.System.Reflection;

namespace MsieJavaScriptEngine.Polyfills.System
{
	/// <summary>
	/// Type extensions
	/// </summary>
	internal static class TypeExtensions
	{
		/// <summary>
		/// Returns the <see cref="TypeInfo"/> representation of the specified type
		/// </summary>
		/// <param name="source">The type to convert</param>
		/// <returns>The converted object</returns>
		public static TypeInfo GetTypeInfo(this Type source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return new TypeInfo(source);
		}
	}
}
#endif
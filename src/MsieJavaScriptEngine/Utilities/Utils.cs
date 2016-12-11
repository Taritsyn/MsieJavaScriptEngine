using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using MsieJavaScriptEngine.Resources;

namespace MsieJavaScriptEngine.Utilities
{
	internal static class Utils
	{
		/// <summary>
		/// Determines whether the current process is a 64-bit process
		/// </summary>
		/// <returns>true if the process is 64-bit; otherwise, false</returns>
		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		public static bool Is64BitProcess()
		{
#if NETSTANDARD1_3
			bool is64Bit = IntPtr.Size == 8;
#else
			bool is64Bit = Environment.Is64BitProcess;
#endif

			return is64Bit;
		}

		/// <summary>
		/// Gets a content of the embedded resource as string
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name without the namespace of the specified type</param>
		/// <param name="type">The type, that determines the assembly and whose namespace is used to scope
		/// the resource name</param>
		/// <returns>Сontent of the embedded resource as string</returns>
		public static string GetResourceAsString(string resourceName, Type type)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(CommonStrings.Common_ArgumentIsNull, "resourceName"));
			}

			if (type == null)
			{
				throw new ArgumentNullException(
					"type", string.Format(CommonStrings.Common_ArgumentIsNull, "type"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			Assembly assembly = type.GetTypeInfo().Assembly;
			string nameSpace = type.Namespace;
			string resourceFullName = nameSpace != null ? nameSpace + "." + resourceName : resourceName;

			return InnerGetResourceAsString(resourceFullName, assembly);
		}

		/// <summary>
		/// Gets a content of the embedded resource as string
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		/// <returns>Сontent of the embedded resource as string</returns>
		public static string GetResourceAsString(string resourceName, Assembly assembly)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(CommonStrings.Common_ArgumentIsNull, "resourceName"));
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					"assembly", string.Format(CommonStrings.Common_ArgumentIsNull, "assembly"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			return InnerGetResourceAsString(resourceName, assembly);
		}

		private static string InnerGetResourceAsString(string resourceName, Assembly assembly)
		{
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					throw new NullReferenceException(
						string.Format(CommonStrings.Resources_ResourceIsNull, resourceName));
				}

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Gets a text content of the specified file
		/// </summary>
		/// <param name="path">File path</param>
		/// <param name="encoding">Content encoding</param>
		/// <returns>Text content</returns>
		public static string GetFileTextContent(string path, Encoding encoding = null)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(
					string.Format(CommonStrings.Common_FileNotExist, path), path);
			}

			string content;

			using (var stream = File.OpenRead(path))
			using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
			{
				content = reader.ReadToEnd();
			}

			return content;
		}
	}
}
#if NETCOREAPP1_0 || NET451
using Microsoft.Extensions.PlatformAbstractions;
#elif NET40
using System;
using System.Text.RegularExpressions;
#else
#error No implementation for this target
#endif
using System.IO;

namespace MsieJavaScriptEngine.Test.Common
{
	public abstract class FileSystemTestsBase
	{
#if NET40
		/// <summary>
		/// Regular expression for working with the `bin` directory path
		/// </summary>
		private readonly Regex _binDirRegex = new Regex(@"\\bin\\(?:Debug|Release)\\?$", RegexOptions.IgnoreCase);

#endif
		protected string _baseDirectoryPath;


		protected FileSystemTestsBase()
		{
#if NETCOREAPP1_0 || NET451
			var appEnv = PlatformServices.Default.Application;
			_baseDirectoryPath = Path.Combine(appEnv.ApplicationBasePath, "../../../../");
#elif NET40
			string baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
			if (_binDirRegex.IsMatch(baseDirectoryPath))
			{
				baseDirectoryPath = Path.Combine(baseDirectoryPath, "../../../");
			}

			_baseDirectoryPath = baseDirectoryPath;
#else
#error No implementation for this target
#endif
		}
	}
}
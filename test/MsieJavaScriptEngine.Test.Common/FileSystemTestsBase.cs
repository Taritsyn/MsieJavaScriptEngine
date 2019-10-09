using System;
using System.IO;
#if NETCOREAPP1_0

using Microsoft.Extensions.PlatformAbstractions;
#endif

namespace MsieJavaScriptEngine.Test.Common
{
	public abstract class FileSystemTestsBase : TestsBase
	{
		private string _baseDirectoryPath;


		protected FileSystemTestsBase()
		{
#if NETCOREAPP1_0
			var appEnv = PlatformServices.Default.Application;
			string appDirectoryPath = appEnv.ApplicationBasePath;
#else
			string appDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
			_baseDirectoryPath = Path.Combine(appDirectoryPath, "../../../../");
		}


		protected string GetAbsolutePath(string relativePath)
		{
			string absolutePath = Path.GetFullPath(Path.Combine(_baseDirectoryPath, relativePath));

			return absolutePath;
		}
	}
}
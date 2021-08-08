using System;
using System.IO;

namespace MsieJavaScriptEngine.Test.Common
{
	public abstract class FileSystemTestsBase : TestsBase
	{
		private string _baseDirectoryPath;


		protected FileSystemTestsBase()
		{
			string appDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
			_baseDirectoryPath = Path.Combine(appDirectoryPath, "../../../../");
		}


		protected string GetAbsolutePath(string relativePath)
		{
			string absolutePath = Path.GetFullPath(Path.Combine(_baseDirectoryPath, relativePath));

			return absolutePath;
		}
	}
}
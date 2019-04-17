﻿using System.IO;
#if !NET40
using System.Reflection;
#endif
using System.Threading.Tasks;

using NUnit.Framework;

namespace MsieJavaScriptEngine.Test.Common
{
	[TestFixture]
	public abstract class PrecompilationTestsBase : FileSystemTestsBase
	{
		#region Execution of precompiled scripts

		[Test]
		public virtual void ExecutionOfPrecompiledCodeIsCorrect()
		{
			// Arrange
			const string libraryCode = @"function declensionOfNumerals(number, titles) {
	var result,
		titleIndex,
		cases = [2, 0, 1, 1, 1, 2],
		caseIndex
		;

	if (number % 100 > 4 && number % 100 < 20) {
		titleIndex = 2;
	}
	else {
		caseIndex = number % 10 < 5 ? number % 10 : 5;
		titleIndex = cases[caseIndex];
	}

	result = titles[titleIndex];

	return result;
}

function declinationOfSeconds(number) {
	return declensionOfNumerals(number, ['секунда', 'секунды', 'секунд']);
}";
			const string functionName = "declinationOfSeconds";
			const int itemCount = 4;

			int[] inputSeconds = new int[itemCount] { 0, 1, 42, 600 };
			string[] targetOutputStrings = new string[itemCount] { "секунд", "секунда", "секунды", "секунд" };
			string[] outputStrings = new string[itemCount];

			// Act
			bool supportsScriptPrecompilation = false;
			PrecompiledScript precompiledCode = null;

			using (var jsEngine = CreateJsEngine())
			{
				supportsScriptPrecompilation = jsEngine.SupportsScriptPrecompilation;
				if (supportsScriptPrecompilation)
				{
					precompiledCode = jsEngine.Precompile(libraryCode, "declinationOfSeconds.js");

					jsEngine.Execute(precompiledCode);
					outputStrings[0] = jsEngine.CallFunction<string>(functionName, inputSeconds[0]);
				}
			}

			if (supportsScriptPrecompilation)
			{
				Parallel.For(1, itemCount, itemIndex =>
				{
					using (var jsEngine = CreateJsEngine())
					{
						jsEngine.Execute(precompiledCode);
						outputStrings[itemIndex] = jsEngine.CallFunction<string>(functionName, inputSeconds[itemIndex]);
					}
				});
			}

			// Assert
			if (supportsScriptPrecompilation)
			{
				for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
				{
					Assert.AreEqual(targetOutputStrings[itemIndex], outputStrings[itemIndex]);
				}
			}
		}

		[Test]
		public virtual void ExecutionOfPrecompiledFileIsCorrect()
		{
			// Arrange
			string filePath = Path.GetFullPath(Path.Combine(_baseDirectoryPath, "SharedFiles/declinationOfMinutes.js"));
			const string functionName = "declinationOfMinutes";
			const int itemCount = 4;

			int[] inputMinutes = new int[itemCount] { 0, 1, 22, 88 };
			string[] targetOutputStrings = new string[itemCount] { "минут", "минута", "минуты", "минут" };
			string[] outputStrings = new string[itemCount];

			// Act
			bool supportsScriptPrecompilation = false;
			PrecompiledScript precompiledFile = null;

			using (var jsEngine = CreateJsEngine())
			{
				supportsScriptPrecompilation = jsEngine.SupportsScriptPrecompilation;
				if (supportsScriptPrecompilation)
				{
					precompiledFile = jsEngine.PrecompileFile(filePath);

					jsEngine.Execute(precompiledFile);
					outputStrings[0] = jsEngine.CallFunction<string>(functionName, inputMinutes[0]);
				}
			}

			if (supportsScriptPrecompilation)
			{
				Parallel.For(1, itemCount, itemIndex =>
				{
					using (var jsEngine = CreateJsEngine())
					{
						jsEngine.Execute(precompiledFile);
						outputStrings[itemIndex] = jsEngine.CallFunction<string>(functionName, inputMinutes[itemIndex]);
					}
				});
			}

			// Assert
			if (supportsScriptPrecompilation)
			{
				for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
				{
					Assert.AreEqual(targetOutputStrings[itemIndex], outputStrings[itemIndex]);
				}
			}
		}

		[Test]
		public virtual void ExecutionOfPrecompiledResourceByNameAndTypeIsCorrect()
		{
			// Arrange
			const string resourceName = "Resources.declinationOfHours.js";
			const string functionName = "declinationOfHours";
			const int itemCount = 4;

			int[] inputHours = new int[itemCount] { 0, 1, 24, 48 };
			string[] targetOutputStrings = new string[itemCount] { "часов", "час", "часа", "часов" };
			string[] outputStrings = new string[itemCount];

			// Act
			bool supportsScriptPrecompilation = false;
			PrecompiledScript precompiledResource = null;

			using (var jsEngine = CreateJsEngine())
			{
				supportsScriptPrecompilation = jsEngine.SupportsScriptPrecompilation;
				if (supportsScriptPrecompilation)
				{
					precompiledResource = jsEngine.PrecompileResource(resourceName, typeof(PrecompilationTestsBase));

					jsEngine.Execute(precompiledResource);
					outputStrings[0] = jsEngine.CallFunction<string>(functionName, inputHours[0]);
				}
			}

			if (supportsScriptPrecompilation)
			{
				Parallel.For(1, itemCount, itemIndex =>
				{
					using (var jsEngine = CreateJsEngine())
					{
						jsEngine.Execute(precompiledResource);
						outputStrings[itemIndex] = jsEngine.CallFunction<string>(functionName, inputHours[itemIndex]);
					}
				});
			}

			// Assert
			if (supportsScriptPrecompilation)
			{
				for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
				{
					Assert.AreEqual(targetOutputStrings[itemIndex], outputStrings[itemIndex]);
				}
			}
		}

		[Test]
		public virtual void ExecutionOfPrecompiledResourceByNameAndAssemblyIsCorrect()
		{
			// Arrange
			const string resourceName = "MsieJavaScriptEngine.Test.Common.Resources.declinationOfDays.js";
			const string functionName = "declinationOfDays";
			const int itemCount = 4;

			int[] inputDays = new int[itemCount] { 0, 1, 3, 80 };
			string[] targetOutputStrings = new string[itemCount] { "дней", "день", "дня", "дней" };
			string[] outputStrings = new string[itemCount];

			// Act
			bool supportsScriptPrecompilation = false;
			PrecompiledScript precompiledResource = null;

			using (var jsEngine = CreateJsEngine())
			{
				supportsScriptPrecompilation = jsEngine.SupportsScriptPrecompilation;
				if (supportsScriptPrecompilation)
				{
					precompiledResource = jsEngine.PrecompileResource(resourceName, typeof(PrecompilationTestsBase)
#if !NET40
						.GetTypeInfo()
#endif
						.Assembly
					);

					jsEngine.Execute(precompiledResource);
					outputStrings[0] = jsEngine.CallFunction<string>(functionName, inputDays[0]);
				}
			}

			if (supportsScriptPrecompilation)
			{
				Parallel.For(1, itemCount, itemIndex =>
				{
					using (var jsEngine = CreateJsEngine())
					{
						jsEngine.Execute(precompiledResource);
						outputStrings[itemIndex] = jsEngine.CallFunction<string>(functionName, inputDays[itemIndex]);
					}
				});
			}

			// Assert
			if (supportsScriptPrecompilation)
			{
				for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
				{
					Assert.AreEqual(targetOutputStrings[itemIndex], outputStrings[itemIndex]);
				}
			}
		}

		#endregion
	}
}
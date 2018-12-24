using System;
using System.IO;
using System.Reflection;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;

namespace MsieJavaScriptEngine.Benchmarks
{
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared)]
	public class JsExecutionBenchmark
	{
		/// <summary>
		/// Name of the file containing library for transliteration of Russian
		/// </summary>
		private const string LibraryFileName = "russian-translit.js";

		/// <summary>
		/// Name of transliterate function
		/// </summary>
		private const string FunctionName = "transliterate";

		/// <summary>
		/// Number of transliterated items
		/// </summary>
		private const int ItemCount = 6;

		/// <summary>
		/// Code of library for transliteration of Russian
		/// </summary>
		private static string _libraryCode;

		/// <summary>
		/// List of transliteration types
		/// </summary>
		private static string[] _inputTypes;

		/// <summary>
		/// List of input strings
		/// </summary>
		private static string[] _inputStrings;

		/// <summary>
		/// List of target output strings
		/// </summary>
		private static string[] _targetOutputStrings;


		/// <summary>
		/// Static constructor
		/// </summary>
		static JsExecutionBenchmark()
		{
			PopulateTestData();
		}


		/// <summary>
		/// Populates a test data
		/// </summary>
		public static void PopulateTestData()
		{
			Type type = typeof(JsExecutionBenchmark);
			Assembly assembly = type.Assembly;
			string resourceName = type.Namespace + ".Resources." + LibraryFileName;

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				_libraryCode = reader.ReadToEnd();
			}

			_inputTypes = new string[ItemCount]
			{
				"basic", "letters-numbers", "gost-16876-71", "gost-7-79-2000", "police", "foreign-passport"
			};
			_inputStrings = new string[ItemCount]
			{
				"SOLID — мнемонический акроним, введённый Майклом Фэзерсом для первых пяти принципов, названных " +
				"Робертом Мартином в начале 2000-х, которые означали пять основных принципов объектно-ориентированного " +
				"программирования и проектирования.",

				"Принцип единственной ответственности (The Single Responsibility Principle). " +
				"Каждый класс выполняет лишь одну задачу.",

				"Принцип открытости/закрытости (The Open Closed Principle). " +
				"«программные сущности … должны быть открыты для расширения, но закрыты для модификации.»",

				"Принцип подстановки Барбары Лисков (The Liskov Substitution Principle). " +
				"«объекты в программе должны быть заменяемыми на экземпляры их подтипов без изменения правильности выполнения программы.»",

				"Принцип разделения интерфейса (The Interface Segregation Principle). " +
				"«много интерфейсов, специально предназначенных для клиентов, лучше, чем один интерфейс общего назначения.»",

				"Принцип инверсии зависимостей (The Dependency Inversion Principle). " +
				"«Зависимость на Абстракциях. Нет зависимости на что-то конкретное.»"
			};
			_targetOutputStrings = new string[ItemCount]
			{
				"SOLID — mnemonicheskij akronim, vvedjonnyj Majklom Fjezersom dlja pervyh pjati principov, nazvannyh " +
				"Robertom Martinom v nachale 2000-h, kotorye oznachali pjat' osnovnyh principov ob#ektno-orientirovannogo " +
				"programmirovanija i proektirovanija.",

				"Princip edinstvennoj otvetstvennosti (The Single Responsibility Principle). " +
				"Ka#dyj klass vypolnjaet li6' odnu zada4u.",

				"Princip otkrytosti/zakrytosti (The Open Closed Principle). " +
				"«programmnye sushhnosti … dolzhny byt' otkryty dlja rasshirenija, no zakryty dlja modifikacii.»",

				"Princip podstanovki Barbary Liskov (The Liskov Substitution Principle). " +
				"«ob\"ekty v programme dolzhny byt' zamenyaemymi na e'kzemplyary ix podtipov bez izmeneniya pravil'nosti " +
				"vypolneniya programmy.»",

				"Printsip razdeleniia interfeisa (The Interface Segregation Principle). " +
				"«mnogo interfeisov, spetsialno prednaznachennykh dlia klientov, luchshe, chem odin interfeis obshchego " +
				"naznacheniia.»",

				"Printcip inversii zavisimostei (The Dependency Inversion Principle). " +
				"«Zavisimost na Abstraktciiakh. Net zavisimosti na chto-to konkretnoe.»"
			};
		}

		/// <summary>
		/// Transliterates a strings
		/// </summary>
		/// <param name="createJsEngine">Delegate for create an instance of the JS engine</param>
		/// <param name="withPrecompilation">Flag for whether to allow execution of JS code with pre-compilation</param>
		private static void TransliterateStrings(Func<MsieJsEngine> createJsEngine, bool withPrecompilation)
		{
			// Arrange
			string[] outputStrings = new string[ItemCount];
			PrecompiledScript precompiledCode = null;

			// Act
			using (var jsEngine = createJsEngine())
			{
				if (withPrecompilation)
				{
					if (!jsEngine.SupportsScriptPrecompilation)
					{
						throw new NotSupportedException($"{jsEngine.Mode} mode does not support precompilation.");
					}

					precompiledCode = jsEngine.Precompile(_libraryCode, LibraryFileName);
					jsEngine.Execute(precompiledCode);
				}
				else
				{
					jsEngine.Execute(_libraryCode, LibraryFileName);
				}

				outputStrings[0] = jsEngine.CallFunction<string>(FunctionName, _inputStrings[0], _inputTypes[0]);
			}

			for (int itemIndex = 1; itemIndex < ItemCount; itemIndex++)
			{
				using (var jsEngine = createJsEngine())
				{
					if (withPrecompilation)
					{
						jsEngine.Execute(precompiledCode);
					}
					else
					{
						jsEngine.Execute(_libraryCode, LibraryFileName);
					}
					outputStrings[itemIndex] = jsEngine.CallFunction<string>(FunctionName, _inputStrings[itemIndex],
						_inputTypes[itemIndex]);
				}
			}

			// Assert
			for (int itemIndex = 0; itemIndex < ItemCount; itemIndex++)
			{
				Assert.Equal(_targetOutputStrings[itemIndex], outputStrings[itemIndex]);
			}
		}
#if NET46

		[Benchmark]
		public void Classic()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings{
				EngineMode = JsEngineMode.Classic
			});
			TransliterateStrings(createJsEngine, false);
		}

		[Benchmark]
		public void ChakraActiveScript()
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraActiveScript
			});
			TransliterateStrings(createJsEngine, false);
		}
#endif

		[Benchmark]
		[Arguments(false)]
		[Arguments(true)]
		public void ChakraIeJsRt(bool withPrecompilation)
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraIeJsRt
			});
			TransliterateStrings(createJsEngine, withPrecompilation);
		}

		[Benchmark]
		[Arguments(false)]
		[Arguments(true)]
		public void ChakraEdgeJsRt(bool withPrecompilation)
		{
			Func<MsieJsEngine> createJsEngine = () => new MsieJsEngine(new JsEngineSettings
			{
				EngineMode = JsEngineMode.ChakraEdgeJsRt
			});
			TransliterateStrings(createJsEngine, withPrecompilation);
		}
	}
}
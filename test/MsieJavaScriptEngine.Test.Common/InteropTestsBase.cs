﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common.Interop;
using MsieJavaScriptEngine.Test.Common.Interop.Animals;
using MsieJavaScriptEngine.Test.Common.Interop.Logging;

namespace MsieJavaScriptEngine.Test.Common
{
	[TestFixture]
	public abstract class InteropTestsBase : FileSystemTestsBase
	{
		#region Embedding of objects

		#region Objects with fields

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomValueTypeWithFields()
		{
			// Arrange
			var date = new Date(2015, 12, 29);
			const string updateCode = "date.Day += 2;";

			const string input1 = "date.Year";
			const int targetOutput1 = 2015;

			const string input2 = "date.Month";
			const int targetOutput2 = 12;

			const string input3 = "date.Day";
			const int targetOutput3 = 31;

			// Act
			int output1;
			int output2;
			int output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("date", date);
				jsEngine.Execute(updateCode);

				output1 = jsEngine.Evaluate<int>(input1);
				output2 = jsEngine.Evaluate<int>(input2);
				output3 = jsEngine.Evaluate<int>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomReferenceTypeWithFields()
		{
			// Arrange
			var product = new Product
			{
				Name = "Red T-shirt",
				Description = string.Empty,
				Price = 995.00
			};

			const string updateCode = @"product.Description = null;
product.Price *= 1.15;";

			const string input1 = "product.Name";
			const string targetOutput1 = "Red T-shirt";

			const string input2 = "product.Description";
			const string targetOutput2 = null;

			const string input3 = "product.Price";
			const double targetOutput3 = 1144.25;

			// Act
			string output1;
			string output2;
			double output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("product", product);
				jsEngine.Execute(updateCode);

				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<double>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		#endregion

		#region Objects with properties

		[Test]
		public virtual void EmbeddingOfInstanceOfBuiltinValueTypeWithProperties()
		{
			// Arrange
			var timeSpan = new TimeSpan(4840780000000);

			const string input1 = "timeSpan.Days";
			const int targetOutput1 = 5;

			const string input2 = "timeSpan.Hours";
			const int targetOutput2 = 14;

			const string input3 = "timeSpan.Minutes";
			const int targetOutput3 = 27;

			const string input4 = "timeSpan.Seconds";
			const int targetOutput4 = 58;

			// Act
			int output1;
			int output2;
			int output3;
			int output4;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("timeSpan", timeSpan);

				output1 = jsEngine.Evaluate<int>(input1);
				output2 = jsEngine.Evaluate<int>(input2);
				output3 = jsEngine.Evaluate<int>(input3);
				output4 = jsEngine.Evaluate<int>(input4);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
			Assert.AreEqual(targetOutput4, output4);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfBuiltinReferenceTypeWithProperties()
		{
			// Arrange
			var uri = new Uri("https://github.com/Taritsyn/MsieJavaScriptEngine");

			const string input1 = "uri.Scheme";
			const string targetOutput1 = "https";

			const string input2 = "uri.Host";
			const string targetOutput2 = "github.com";

			const string input3 = "uri.PathAndQuery";
			const string targetOutput3 = "/Taritsyn/MsieJavaScriptEngine";

			// Act
			string output1;
			string output2;
			string output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("uri", uri);

				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<string>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomValueTypeWithProperties()
		{
			// Arrange
			var temperature = new Temperature(-17.3, TemperatureUnits.Celsius);

			const string input1 = "temperature.Celsius";
			const double targetOutput1 = -17.3;

			const string input2 = "temperature.Kelvin";
			const double targetOutput2 = 255.85;

			const string input3 = "temperature.Fahrenheit";
			const double targetOutput3 = 0.86;

			// Act
			double output1;
			double output2;
			double output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("temperature", temperature);

				output1 = Math.Round(jsEngine.Evaluate<double>(input1), 2);
				output2 = Math.Round(jsEngine.Evaluate<double>(input2), 2);
				output3 = Math.Round(jsEngine.Evaluate<double>(input3), 2);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomReferenceTypeWithProperties()
		{
			// Arrange
			var person = new Person("Vanya", "Ivanov");
			const string updateCode = @"person.LastName = person.LastName.substr(0, 5) + 'ff';
person.Patronymic = null;";

			const string input1 = "person.FirstName";
			const string targetOutput1 = "Vanya";

			const string input2 = "person.LastName";
			const string targetOutput2 = "Ivanoff";

			const string input3 = "person.Patronymic";
			const string targetOutput3 = null;

			// Act
			string output1;
			string output2;
			string output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("person", person);
				jsEngine.Execute(updateCode);

				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<string>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfAnonymousTypeWithProperties()
		{
			// Arrange
			var person = new
			{
				FirstName = "John",
				LastName = "Doe",
				Address = new
				{
					StreetAddress = "103 Elm Street",
					City = "Atlanta",
					State = "GA",
					PostalCode = 30339
				}
			};

			const string input1 = "person.FirstName";
			const string targetOutput1 = "John";

			const string input2 = "person.LastName";
			const string targetOutput2 = "Doe";

			const string input3 = "person.Address.StreetAddress";
			const string targetOutput3 = "103 Elm Street";

			const string input4 = "person.Address.City";
			const string targetOutput4 = "Atlanta";

			const string input5 = "person.Address.State";
			const string targetOutput5 = "GA";

			const string input6 = "person.Address.PostalCode";
			const int targetOutput6 = 30339;

			// Act
			string output1;
			string output2;
			string output3;
			string output4;
			string output5;
			int output6;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("person", person);

				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<string>(input3);
				output4 = jsEngine.Evaluate<string>(input4);
				output5 = jsEngine.Evaluate<string>(input5);
				output6 = jsEngine.Evaluate<int>(input6);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
			Assert.AreEqual(targetOutput4, output4);
			Assert.AreEqual(targetOutput5, output5);
			Assert.AreEqual(targetOutput6, output6);
		}

		#endregion

		#region Objects with methods

		[Test]
		public virtual void EmbeddingOfInstanceOfBuiltinValueTypeWithMethods()
		{
			// Arrange
			var color = Color.FromArgb(84, 139, 212);

			const string input1 = "color.GetHue()";
			const double targetOutput1 = 214.21875;

			const string input2 = "color.GetSaturation()";
			const double targetOutput2 = 0.59813;

			const string input3 = "color.GetBrightness()";
			const double targetOutput3 = 0.58039;

			// Act
			double output1;
			double output2;
			double output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("color", color);

				output1 = Math.Round(jsEngine.Evaluate<double>(input1), 5);
				output2 = Math.Round(jsEngine.Evaluate<double>(input2), 5);
				output3 = Math.Round(jsEngine.Evaluate<double>(input3), 5);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfBuiltinReferenceTypeWithMethod()
		{
			// Arrange
			var random = new Random();

			const string input = "random.Next(1, 3)";
			IEnumerable<int> targetOutput = Enumerable.Range(1, 3);

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("random", random);
				output = jsEngine.Evaluate<int>(input);
			}

			// Assert
			Assert.IsTrue(targetOutput.Contains(output));
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomValueTypeWithMethods()
		{
			// Arrange
			var programmerDayDate = new Date(2015, 9, 13);

			const string input1 = "programmerDay.GetDayOfYear()";
			const int targetOutput1 = 256;

			const string input2 = @"var smileDay = programmerDay.AddDays(6);
smileDay.GetDayOfYear();";
			const int targetOutput2 = 262;

			// Act
			int output1;
			int output2;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("programmerDay", programmerDayDate);
				output1 = jsEngine.Evaluate<int>(input1);
				output2 = jsEngine.Evaluate<int>(input2);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfCustomReferenceTypeWithMethod()
		{
			// Arrange
			var fileManager = new FileManager();
			string filePath = GetAbsolutePath("SharedFiles/link.txt");

			string input = string.Format("fileManager.ReadFile('{0}', null)", filePath.Replace(@"\", @"\\"));
			const string targetOutput = "http://www.panopticoncentral.net/2015/09/09/the-two-faces-of-jsrt-in-windows-10/";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("fileManager", fileManager);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void CallingOfMethodOfCustomReferenceTypeWithInterfaceParameter()
		{
			// Arrange
			var animalTrainer = new AnimalTrainer();
			var cat = new Cat();
			var dog = new Dog();

			const string input1 = "animalTrainer.ExecuteVoiceCommand(cat)";
			const string targetOutput1 = "Meow!";

			const string input2 = "animalTrainer.ExecuteVoiceCommand(dog)";
			const string targetOutput2 = "Woof!";

			// Act
			string output1;
			string output2;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("animalTrainer", animalTrainer);
				jsEngine.EmbedHostObject("cat", cat);
				jsEngine.EmbedHostObject("dog", dog);
				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
		}

		#endregion

		#region Delegates

		[Test]
		public virtual void EmbeddingOfInstanceOfDelegateWithoutParameters()
		{
			// Arrange
			var generateRandomStringFunc = new Func<string>(() =>
			{
				const string symbolString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
				int symbolStringLength = symbolString.Length;
				Random randomizer = new Random();
				string result = string.Empty;

				for (int i = 0; i < 20; i++)
				{
					int randomNumber = randomizer.Next(symbolStringLength);
					string randomSymbol = symbolString.Substring(randomNumber, 1);

					result += randomSymbol;
				}

				return result;
			});

			const string input = "generateRandomString()";
			const int targetOutputLength = 20;

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("generateRandomString", generateRandomStringFunc);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.That(output, Is.Not.Null.Or.Empty);
			Assert.IsTrue(output.Length == targetOutputLength);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfDelegateWithOneParameter()
		{
			// Arrange
			var squareFunc = new Func<int, int>(a => a * a);

			const string input = "square(7)";
			const int targetOutput = 49;

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("square", squareFunc);
				output = jsEngine.Evaluate<int>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfDelegateWithTwoParameters()
		{
			// Arrange
			var sumFunc = new Func<double, double, double>((a, b) => a + b);

			const string input = "sum(3.14, 2.20)";
			const double targetOutput = 5.34;

			// Act
			double output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("sum", sumFunc);
				output = jsEngine.Evaluate<double>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfInstanceOfDelegateWithoutResult()
		{
			// Arrange
			var logBuilder = new StringBuilder();
			Action<string> log = (string value) =>
			{
				logBuilder.AppendLine(value);
			};

			const string input = @"(function(log, undefined) {
	var num = 2, count = 0;

	log('-= Start code execution =-');

	while (num != Infinity) {
		num = num * num;
		count++;
	}

	log('-= End of code execution =-');

	return count;
}(log));";
			const int targetOutput = 10;
			string targetLogOutput = "-= Start code execution =-" + Environment.NewLine +
				"-= End of code execution =-" + Environment.NewLine;

			// Act
			int output;
			string logOutput;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("log", log);
				output = jsEngine.Evaluate<int>(input);

				logOutput = logBuilder.ToString();
				logBuilder.Clear();
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
			Assert.AreEqual(targetLogOutput, logOutput);
		}

		[Test]
		public virtual void CallingOfEmbeddedDelegateWithMissingParameter()
		{
			// Arrange
			var sumFunc = new Func<int, int, int>((a, b) => a + b);

			const string input = "sum(678)";
			JsRuntimeException exception = null;

			// Act
			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("sum", sumFunc);

				try
				{
					int result = jsEngine.Evaluate<int>(input);
				}
				catch (JsRuntimeException e)
				{
					exception = e;
				}
			}

			// Assert
			Assert.NotNull(exception);
		}

		[Test]
		public virtual void CallingOfEmbeddedDelegateWithExtraParameter()
		{
			// Arrange
			var sumFunc = new Func<int, int, int>((a, b) => a + b);

			const string input = "sum(678, 711, 611)";
			const int targetOutput = 1389;

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("sum", sumFunc);
				output = jsEngine.Evaluate<int>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Integration

		[Test]
		public virtual void InteractionOfEmbeddedCustomValueTypeAndDelegateInstances()
		{
			// Arrange
			var informaticsDayDate = new Date(2015, 12, 4);
			var addDaysFunc = new Func<Date, int, Date>((value, numberOfDays) =>
			{
				var originalDateTime = new DateTime(value.Year, value.Month, value.Day);
				DateTime changedDateTime = originalDateTime.AddDays(numberOfDays);

				return new Date(changedDateTime.Year, changedDateTime.Month, changedDateTime.Day);
			});

			const string updateCode = @"var webmasterDay = addDays(informaticsDay, -244);
var sysadminDay = addDays(webmasterDay, 118);";

			const string input1 = "sysadminDay.Year";
			const int targetOutput1 = 2015;

			const string input2 = "sysadminDay.Month";
			const int targetOutput2 = 7;

			const string input3 = "sysadminDay.Day";
			const int targetOutput3 = 31;

			// Act
			int output1;
			int output2;
			int output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("informaticsDay", informaticsDayDate);
				jsEngine.EmbedHostObject("addDays", addDaysFunc);
				jsEngine.Execute(updateCode);

				output1 = jsEngine.Evaluate<int>(input1);
				output2 = jsEngine.Evaluate<int>(input2);
				output3 = jsEngine.Evaluate<int>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		#endregion

		#region Recursive calls

		[Test]
		public virtual void RecursiveEvaluationOfFiles()
		{
			// Arrange
			string directoryPath = GetAbsolutePath("SharedFiles/recursiveEvaluation/noError");
			const string input = "require('index').calculateResult();";
			const double targetOutput = 132.14;

			// Act
			double output;

			using (var jsEngine = CreateJsEngine())
			{
				Func<string, object> loadModule = path => {
					string absolutePath = Path.Combine(directoryPath, $"{path}.js");
					string code = File.ReadAllText(absolutePath);
					object result = jsEngine.Evaluate(code, absolutePath);

					return result;
				};

				jsEngine.EmbedHostObject("require", loadModule);
				output = jsEngine.Evaluate<double>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void RecursiveExecutionOfFiles()
		{
			// Arrange
			string directoryPath = GetAbsolutePath("SharedFiles/recursiveExecution/noError");
			const string variableName = "num";
			const int targetOutput = 12;

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				Action<string> executeFile = path => jsEngine.ExecuteFile(path);

				jsEngine.SetVariableValue("directoryPath", directoryPath);
				jsEngine.EmbedHostObject("executeFile", executeFile);
				jsEngine.ExecuteFile(Path.Combine(directoryPath, "mainFile.js"));

				output = jsEngine.GetVariableValue<int>(variableName);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Removal

		[Test]
		public virtual void RemovingOfEmbeddedInstanceOfCustomReferenceType()
		{
			// Arrange
			var person = new Person("Vasya", "Pupkin");

			// Act
			Exception currentException = null;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostObject("person", person);

				try
				{
					jsEngine.RemoveVariable("person");
				}
				catch (Exception e)
				{
					currentException = e;
				}
			}

			// Assert
			Assert.Null(currentException);
		}

		#endregion

		#endregion


		#region Embedding of types

		#region Creating of instances

		[Test]
		public virtual void CreatingAnInstanceOfEmbeddedBuiltinValueType()
		{
			// Arrange
			Type pointType = typeof(Point);

			const string input = "(new Point()).ToString()";
			const string targetOutput = "{X=0,Y=0}";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Point", pointType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void CreatingAnInstanceOfEmbeddedBuiltinReferenceType()
		{
			// Arrange
			Type uriType = typeof(Uri);

			const string input = @"var baseUri = new Uri('https://github.com'),
	relativeUri = 'Taritsyn/MsieJavaScriptEngine'
	;

(new Uri(baseUri, relativeUri)).ToString()";
			const string targetOutput = "https://github.com/Taritsyn/MsieJavaScriptEngine";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Uri", uriType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void CreatingAnInstanceOfEmbeddedCustomValueType()
		{
			// Arrange
			Type point3DType = typeof(Point3D);

			const string input = "(new Point3D(2, 5, 14)).ToString()";
			const string targetOutput = "{X=2,Y=5,Z=14}";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Point3D", point3DType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void CreatingAnInstanceOfEmbeddedCustomReferenceType()
		{
			// Arrange
			Type personType = typeof(Person);

			const string input = "(new Person('Vanya', 'Tutkin')).ToString()";
			const string targetOutput = "{FirstName=Vanya,LastName=Tutkin}";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Person", personType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Types with constants

		[Test]
		public virtual void EmbeddingOfBuiltinReferenceTypeWithConstants()
		{
			// Arrange
			Type mathType = typeof(Math);

			const string input1 = "Math2.PI";
			const double targetOutput1 = 3.1415926535897931d;

			const string input2 = "Math2.E";
			const double targetOutput2 = 2.7182818284590451d;

			// Act
			double output1;
			double output2;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Math2", mathType);

				output1 = jsEngine.Evaluate<double>(input1);
				output2 = jsEngine.Evaluate<double>(input2);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
		}

		[Test]
		public virtual void EmbeddingOfCustomValueTypeWithConstants()
		{
			// Arrange
			Type predefinedStringsType = typeof(PredefinedStrings);

			const string input1 = "PredefinedStrings.VeryLongName";
			const string targetOutput1 = "Very Long Name";

			const string input2 = "PredefinedStrings.AnotherVeryLongName";
			const string targetOutput2 = "Another Very Long Name";

			const string input3 = "PredefinedStrings.TheLastVeryLongName";
			const string targetOutput3 = "The Last Very Long Name";

			// Act
			string output1;
			string output2;
			string output3;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("PredefinedStrings", predefinedStringsType);

				output1 = jsEngine.Evaluate<string>(input1);
				output2 = jsEngine.Evaluate<string>(input2);
				output3 = jsEngine.Evaluate<string>(input3);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
			Assert.AreEqual(targetOutput3, output3);
		}

		[Test]
		public virtual void EmbeddingOfCustomReferenceTypeWithConstant()
		{
			// Arrange
			Type base64EncoderType = typeof(Base64Encoder);

			const string input = "Base64Encoder.DATA_URI_MAX";
			const int targetOutput = 32768;

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Base64Encoder", base64EncoderType);
				output = jsEngine.Evaluate<int>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Types with fields

		[Test]
		public virtual void EmbeddingOfBuiltinValueTypeWithField()
		{
			// Arrange
			Type guidType = typeof(Guid);

			const string input = "Guid.Empty.ToString()";
			const string targetOutput = "00000000-0000-0000-0000-000000000000";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Guid", guidType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfBuiltinReferenceTypeWithField()
		{
			// Arrange
			Type bitConverterType = typeof(BitConverter);

			const string input = "BitConverter.IsLittleEndian";
			const bool targetOutput = true;

			// Act
			bool output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("BitConverter", bitConverterType);
				output = (bool)jsEngine.Evaluate(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfCustomValueTypeWithField()
		{
			// Arrange
			Type point3DType = typeof(Point3D);

			const string input = "Point3D.Empty.ToString()";
			const string targetOutput = "{X=0,Y=0,Z=0}";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Point3D", point3DType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfCustomReferenceTypeWithField()
		{
			// Arrange
			Type defaultLoggerType = typeof(DefaultLogger);
			Type throwExceptionLoggerType = typeof(ThrowExceptionLogger);
			const string updateCode = "DefaultLogger.Current = new ThrowExceptionLogger();";

			const string input = "DefaultLogger.Current.ToString()";
			const string targetOutput = "[throw exception logger]";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("DefaultLogger", defaultLoggerType);
				jsEngine.EmbedHostType("ThrowExceptionLogger", throwExceptionLoggerType);
				jsEngine.Execute(updateCode);

				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Types with properties

		[Test]
		public virtual void EmbeddingOfBuiltinValueTypeWithProperty()
		{
			// Arrange
			Type colorType = typeof(Color);

			const string input = "Color.OrangeRed.ToString()";
			const string targetOutput = "Color [OrangeRed]";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Color", colorType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfBuiltinReferenceTypeWithProperty()
		{
			// Arrange
			Type environmentType = typeof(Environment);

			const string input = "Environment.NewLine";
			string[] targetOutput = { "\r", "\r\n", "\n", "\n\r" };

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Environment", environmentType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.IsTrue(targetOutput.Contains(output));
		}

		[Test]
		public virtual void EmbeddingOfCustomValueTypeWithProperty()
		{
			// Arrange
			Type dateType = typeof(Date);

			const string initCode = "var currentDate = Date2.Today;";

			const string inputYear = "currentDate.Year";
			const string inputMonth = "currentDate.Month";
			const string inputDay = "currentDate.Day";

			DateTime targetOutput = DateTime.Today;

			// Act
			DateTime output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Date2", dateType);
				jsEngine.Execute(initCode);

				var outputYear = jsEngine.Evaluate<int>(inputYear);
				var outputMonth = jsEngine.Evaluate<int>(inputMonth);
				var outputDay = jsEngine.Evaluate<int>(inputDay);

				output = new DateTime(outputYear, outputMonth, outputDay);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfCustomReferenceTypeWithProperty()
		{
			// Arrange
			Type bundleTableType = typeof(BundleTable);
			const string updateCode = "BundleTable.EnableOptimizations = false;";

			const string input = "BundleTable.EnableOptimizations";
			const bool targetOutput = false;

			// Act
			bool output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("BundleTable", bundleTableType);
				jsEngine.Execute(updateCode);

				output = jsEngine.Evaluate<bool>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Types with methods

		[Test]
		public virtual void EmbeddingOfBuiltinValueTypeWithMethod()
		{
			// Arrange
			Type dateTimeType = typeof(DateTime);

			const string input = "DateTime.DaysInMonth(2016, 2)";
			const int targetOutput = 29;

			// Act
			int output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("DateTime", dateTimeType);
				output = jsEngine.Evaluate<int>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfBuiltinReferenceTypeWithMethods()
		{
			// Arrange
			Type mathType = typeof(Math);

			const string input1 = "Math2.Max(5.37, 5.56)";
			const double targetOutput1 = 5.56;

			const string input2 = "Math2.Log10(23)";
			const double targetOutput2 = 1.36172783601759;

			// Act
			double output1;
			double output2;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Math2", mathType);
				output1 = jsEngine.Evaluate<double>(input1);
				output2 = Math.Round(jsEngine.Evaluate<double>(input2), 14);
			}

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
		}

		[Test]
		public virtual void EmbeddingOfCustomValueTypeWithMethod()
		{
			// Arrange
			var dateType = typeof(Date);

			const string input = "Date2.IsLeapYear(2016)";
			const bool targetOutput = true;

			// Act
			bool output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Date2", dateType);
				output = jsEngine.Evaluate<bool>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public virtual void EmbeddingOfCustomReferenceTypeWithMethod()
		{
			// Arrange
			Type base64EncoderType = typeof(Base64Encoder);

			const string input = "Base64Encoder.Encode('https://github.com/Taritsyn/MsieJavaScriptEngine')";
			const string targetOutput = "aHR0cHM6Ly9naXRodWIuY29tL1Rhcml0c3luL01zaWVKYXZhU2NyaXB0RW5naW5l";

			// Act
			string output;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Base64Encoder", base64EncoderType);
				output = jsEngine.Evaluate<string>(input);
			}

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		#endregion

		#region Removal

		[Test]
		public virtual void RemovingOfEmbeddedCustomReferenceType()
		{
			// Arrange
			Type personType = typeof(Person);

			// Act
			Exception currentException = null;

			using (var jsEngine = CreateJsEngine())
			{
				jsEngine.EmbedHostType("Person", personType);

				try
				{
					jsEngine.RemoveVariable("Person");
				}
				catch (Exception e)
				{
					currentException = e;
				}
			}

			// Assert
			Assert.Null(currentException);
		}

		#endregion

		#endregion
	}
}
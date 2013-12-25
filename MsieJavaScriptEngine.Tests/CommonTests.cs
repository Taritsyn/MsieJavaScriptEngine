namespace MsieJavaScriptEngine.Tests
{
	using System;
	using System.IO;
	using System.Reflection;

	using NUnit.Framework;

	using MsieJavaScriptEngine;

	[TestFixture]
	public class CommonTests
	{
		private MsieJsEngine _jsEngine;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_jsEngine = new MsieJsEngine(false, false);
		}

		#region Checking of name format
		[Test]
		public void NameFormatIsCorrect()
		{
			// Arrange

			// Act
			bool name1FormatIsCorrect = _jsEngine.CheckNameFormat("good_parts");
			bool name2FormatIsCorrect = _jsEngine.CheckNameFormat("i18n");
			bool name3FormatIsCorrect = _jsEngine.CheckNameFormat("fooBar");
			bool name4FormatIsCorrect = _jsEngine.CheckNameFormat("$grid");
			bool name5FormatIsCorrect = _jsEngine.CheckNameFormat("a");

			// Assert
			Assert.IsTrue(name1FormatIsCorrect);
			Assert.IsTrue(name2FormatIsCorrect);
			Assert.IsTrue(name3FormatIsCorrect);
			Assert.IsTrue(name4FormatIsCorrect);
			Assert.IsTrue(name5FormatIsCorrect);
		}

		[Test]
		public void NameFormatIsWrong()
		{
			// Arrange

			// Act
			bool name1FormatIsCorrect = _jsEngine.CheckNameFormat("good-parts");
			bool name2FormatIsCorrect = _jsEngine.CheckNameFormat("1sale");
			bool name3FormatIsCorrect = _jsEngine.CheckNameFormat("Foo Bar");
			bool name4FormatIsCorrect = _jsEngine.CheckNameFormat("@grid");
			bool name5FormatIsCorrect = _jsEngine.CheckNameFormat("2");

			// Assert
			Assert.IsFalse(name1FormatIsCorrect);
			Assert.IsFalse(name2FormatIsCorrect);
			Assert.IsFalse(name3FormatIsCorrect);
			Assert.IsFalse(name4FormatIsCorrect);
			Assert.IsFalse(name5FormatIsCorrect);
		}

		[Test]
		public void PropertyNameFormatIsCorrect()
		{
			// Arrange

			// Act
			bool propertyName1IsCorrect = _jsEngine.CheckPropertyNameFormat("good_parts");
			bool propertyName2IsCorrect = _jsEngine.CheckPropertyNameFormat("good_parts.list");
			bool propertyName3IsCorrect = _jsEngine.CheckPropertyNameFormat("Products.Product.Price");
			bool propertyName4IsCorrect = _jsEngine.CheckPropertyNameFormat("$grid.rows.cells");
			bool propertyName5IsCorrect = _jsEngine.CheckPropertyNameFormat("a");

			// Assert
			Assert.IsTrue(propertyName1IsCorrect);
			Assert.IsTrue(propertyName2IsCorrect);
			Assert.IsTrue(propertyName3IsCorrect);
			Assert.IsTrue(propertyName4IsCorrect);
			Assert.IsTrue(propertyName5IsCorrect);
		}

		[Test]
		public void PropertyNameFormatIsWrong()
		{
			// Arrange

			// Act
			bool propertyName1IsCorrect = _jsEngine.CheckPropertyNameFormat("bad-parts");
			bool propertyName2IsCorrect = _jsEngine.CheckPropertyNameFormat("bad-parts.list");
			bool propertyName3IsCorrect = _jsEngine.CheckPropertyNameFormat("Products Product.Price");
			bool propertyName4IsCorrect = _jsEngine.CheckPropertyNameFormat("@grid.rows.cells");
			bool propertyName5IsCorrect = _jsEngine.CheckPropertyNameFormat("@a.b.c");

			// Assert
			Assert.IsFalse(propertyName1IsCorrect);
			Assert.IsFalse(propertyName2IsCorrect);
			Assert.IsFalse(propertyName3IsCorrect);
			Assert.IsFalse(propertyName4IsCorrect);
			Assert.IsFalse(propertyName5IsCorrect);
		}

		[Test]
		public void NameIsAllowed()
		{
			// Arrange

			// Act
			bool name1IsAllowed = _jsEngine.CheckNameAllowability("page_total");
			bool name2IsAllowed = _jsEngine.CheckNameAllowability("L10n");
			bool name3IsAllowed = _jsEngine.CheckNameAllowability("jQuery");
			bool name4IsAllowed = _jsEngine.CheckNameAllowability("$pager");

			// Assert
			Assert.IsTrue(name1IsAllowed);
			Assert.IsTrue(name2IsAllowed);
			Assert.IsTrue(name3IsAllowed);
			Assert.IsTrue(name4IsAllowed);
		}

		[Test]
		public void NameIsForbidden()
		{
			// Arrange
			var forbiddenNames = new[]
			{
				"abstract",
				"boolean", "break", "byte",
				"case", "catch", "char", "class", "const", "continue",
				"debugger", "default", "delete", "do", "double",
				"else", "enum", "export", "extends",
				"false", "final", "finally", "float", "for", "function",
				"goto",
				"if", "implements", "import", "in", "instanceof", "int", "interface",
				"long",
				"native", "new", "null",
				"package", "private", "protected", "public",
				"return",
				"short", "static", "super", "switch", "synchronized",
				"this", "throw", "throws", "transient", "true", "try", "typeof",
				"var", "volatile", "void",
				"while", "with"
			};

			// Act
			bool isAllowed = false;

			foreach(string forbiddenName in forbiddenNames)
			{
				isAllowed = _jsEngine.CheckNameAllowability(forbiddenName);
				if (isAllowed)
				{
					break;
				}
			}

			// Assert
			Assert.IsFalse(isAllowed);
		}

		[Test]
		public void PropertyNameIsAllowed()
		{
			// Arrange

			// Act
			bool propertyName1IsAllowed = _jsEngine.CheckPropertyNameAllowability("annual_report");
			bool propertyName2IsAllowed = _jsEngine.CheckPropertyNameAllowability("annual_report.total_sum");
			bool propertyName3IsAllowed = _jsEngine.CheckPropertyNameAllowability("jQuery.extend");
			bool propertyName4IsAllowed = _jsEngine.CheckPropertyNameAllowability("$pager.buttons.nextPageButton");
			bool propertyName5IsAllowed = _jsEngine.CheckPropertyNameAllowability("b.a.c.e.d");

			// Assert
			Assert.IsTrue(propertyName1IsAllowed);
			Assert.IsTrue(propertyName2IsAllowed);
			Assert.IsTrue(propertyName3IsAllowed);
			Assert.IsTrue(propertyName4IsAllowed);
			Assert.IsTrue(propertyName5IsAllowed);
		}

		[Test]
		public void PropertyNameIsForbidden()
		{
			// Arrange

			// Act
			bool propertyName1IsAllowed = _jsEngine.CheckPropertyNameAllowability("debugger");
			bool propertyName2IsAllowed = _jsEngine.CheckPropertyNameAllowability("annual_report.export");
			bool propertyName3IsAllowed = _jsEngine.CheckPropertyNameAllowability("jQuery.extends");
			bool propertyName4IsAllowed = _jsEngine.CheckPropertyNameAllowability("this.$pager.nextPage.goto");
			bool propertyName5IsAllowed = _jsEngine.CheckPropertyNameAllowability("b.a.do.e.in");

			// Assert
			Assert.IsFalse(propertyName1IsAllowed);
			Assert.IsFalse(propertyName2IsAllowed);
			Assert.IsFalse(propertyName3IsAllowed);
			Assert.IsFalse(propertyName4IsAllowed);
			Assert.IsFalse(propertyName5IsAllowed);
		}
		#endregion

		#region Evaluation of code
		[Test]
		public void EvaluationOfExpressionWithUndefinedResultIsCorrect()
		{
			// Arrange
			const string input = "undefined";
			var targetOutput = Undefined.Value;

			// Act
			var output = _jsEngine.Evaluate(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void EvaluationOfExpressionWithNullResultIsCorrect()
		{
			// Arrange
			const string input = "null";
			const object targetOutput = null;

			// Act
			var output = _jsEngine.Evaluate(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void EvaluationOfExpressionWithBooleanResultIsCorrect()
		{
			// Arrange
			const string input1 = "7 > 5";
			const bool targetOutput1 = true;

			const string input2 = "null === undefined";
			const bool targetOutput2 = false;

			// Act
			var output1 = _jsEngine.Evaluate<bool>(input1);
			var output2 = _jsEngine.Evaluate<bool>(input2);

			// Assert
			Assert.AreEqual(targetOutput1, output1);
			Assert.AreEqual(targetOutput2, output2);
		}

        [Test]
        public void EvaluationOfExpressionWithIntegerResultIsCorrect()
        {
            // Arrange
            const string input = "7 * 8 - 20";
            const int targetOutput = 36;

            // Act
            var output = _jsEngine.Evaluate<int>(input);

            // Assert
            Assert.AreEqual(targetOutput, output);
        }

		[Test]
		public void EvaluationOfExpressionWithDoubleResultIsCorrect()
		{
			// Arrange
			const string input = "Math.PI + 0.22";
			const double targetOutput = 3.36;

			// Act
			var output = Math.Round(_jsEngine.Evaluate<double>(input), 2);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void EvaluationOfExpressionWithStringResultIsCorrect()
		{
			// Arrange
			const string input = "'Hello, ' + \"Vasya\" + '?';";
			const string targetOutput = "Hello, Vasya?";

			// Act
			var output = _jsEngine.Evaluate<string>(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}
		#endregion

		#region Execution of code
		[Test]
		public void ExecutionOfCodeIsCorrect()
		{
			// Arrange
			const string functionCode = @"function add(num1, num2) {
				return (num1 + num2);
			}";
			const string input = "add(7, 9);";
			const int targetOutput = 16;

			// Act
			_jsEngine.Execute(functionCode);
			var output = _jsEngine.Evaluate<int>(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void ExecutionOfFileIsCorrect()
		{
			// Arrange
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Resources/square.js");
			const string input = "square(6);";
			const int targetOutput = 36;

			// Act
			_jsEngine.ExecuteFile(filePath);
			var output = _jsEngine.Evaluate<int>(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void ExecutionOfResourceByTypeIsCorrect()
		{
			// Arrange
			const string resourceName = "MsieJavaScriptEngine.Tests.Resources.cube.js";
			const string input = "cube(5);";
			const int targetOutput = 125;

			// Act
			_jsEngine.ExecuteResource(resourceName, GetType());
			var output = _jsEngine.Evaluate<int>(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}

		[Test]
		public void ExecutionOfResourceByAssemblyIsCorrect()
		{
			// Arrange
			const string resourceName = "MsieJavaScriptEngine.Tests.Resources.power.js";
			const string input = "power(4, 3);";
			const int targetOutput = 64;

			// Act
			_jsEngine.ExecuteResource(resourceName, Assembly.GetExecutingAssembly());
			var output = _jsEngine.Evaluate<int>(input);

			// Assert
			Assert.AreEqual(targetOutput, output);
		}
		#endregion

		#region Calling of functions
        [Test]
        public void CallingOfFunctionWithUndefinedResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function testUndefined(value) {
	if (typeof value !== 'undefined') {
		throw new TypeError();
	}

	return undefined;
}";
            object input = Undefined.Value;

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction("testUndefined", input);

            // Assert
            Assert.AreEqual(input, output);
        }

        [Test]
        public void CallingOfFunctionWithNullResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function testNull(value) {
	if (value !== null) {
		throw new TypeError();
	}

	return null;
}";
            const object input = null;

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction("testNull", input);

            // Assert
            Assert.AreEqual(input, output);
        }

        [Test]
        public void CallingOfFunctionWithBooleanResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function inverse(value) {
	return !value;
}";
            const bool input = false;
            const bool targetOutput = true;

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<bool>("inverse", input);

            // Assert
            Assert.AreEqual(targetOutput, output);
        }

        [Test]
        public void CallingOfFunctionWithIntegerResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function negate(value) {
	return -1 * value;
}";
            const int input = 28;
            const int targetOutput = -28;

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<int>("negate", input);

            // Assert
            Assert.AreEqual(targetOutput, output);
        }

        [Test]
        public void CallingOfFunctionWithDoubleResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function triple(value) {
	return 3 * value;
}";
            const double input = 3.2;
            const double targetOutput = 9.6;

            // Act
            _jsEngine.Execute(functionCode);
            var output = Math.Round(_jsEngine.CallFunction<double>("triple", input), 1);

            // Assert
            Assert.AreEqual(targetOutput, output);
        }

        [Test]
        public void CallingOfFunctionWithStringResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function greeting(name) {
	return 'Hello, ' + name + '!';
}";
            const string input = "Vovan";
            const string targetOutput = "Hello, Vovan!";

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<string>("greeting", input);

            // Assert
            Assert.AreEqual(targetOutput, output);
        }

        [Test]
        public void CallingOfFunctionWithManyParametersIsCorrect()
        {
            // Arrange
            const string functionCode = @"function determineArgumentsTypes() {
	var result = '', 
		argumentIndex,
		argumentCount = arguments.length
		;

	for (argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++) {
        if (argumentIndex > 0) {
            result += ', ';
        }
		result += typeof arguments[argumentIndex];
	}

	return result;
}";

            // Act
            _jsEngine.Execute(functionCode);
            var output = (string)_jsEngine.CallFunction("determineArgumentsTypes", Undefined.Value, null, 
                true, 12, 3.14, "test");

            // Assert
            Assert.AreEqual("undefined, object, boolean, number, number, string", output);
        }

        [Test]
        public void CallingOfFunctionWithManyParametersAndBooleanResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function and() {
	var result = null, 
		argumentIndex,
		argumentCount = arguments.length,
		argumentValue
		;

	for (argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++) {
		argumentValue = arguments[argumentIndex];
	
		if (result !== null) {
			result = result && argumentValue;
		}
		else {
			result = argumentValue;
		}
	}

	return result;
}";

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<bool>("and", true, true, false, true);

            // Assert
            Assert.AreEqual(false, output);
        }

        [Test]
        public void CallingOfFunctionWithManyParametersAndIntegerResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function sum() {
	var result = 0, 
		argumentIndex,
		argumentCount = arguments.length
		;

	for (argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++) {
		result += arguments[argumentIndex];
	}

	return result;
}";

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<int>("sum", 120, 5, 18, 63);

            // Assert
            Assert.AreEqual(206, output);
        }

        [Test]
        public void CallingOfFunctionWithManyParametersAndDoubleResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function sum() {
	var result = 0, 
		argumentIndex,
		argumentCount = arguments.length
		;

	for (argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++) {
		result += arguments[argumentIndex];
	}

	return result;
}";

            // Act
            _jsEngine.Execute(functionCode);
            var output = Math.Round(_jsEngine.CallFunction<double>("sum", 22000, 8.5, 0.05, 3), 2);

            // Assert
            Assert.AreEqual(22011.55, output);
        }

        [Test]
        public void CallingOfFunctionWithManyParametersAndStringResultIsCorrect()
        {
            // Arrange
            const string functionCode = @"function concatenate() {
	var result = '', 
		argumentIndex,
		argumentCount = arguments.length
		;

	for (argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++) {
		result += arguments[argumentIndex];
	}

	return result;
}";

            // Act
            _jsEngine.Execute(functionCode);
            var output = _jsEngine.CallFunction<string>("concatenate", "Hello", ",", " ", "Petya", "!");

            // Assert
            Assert.AreEqual("Hello, Petya!", output);
        }
		#endregion

		#region Getting, setting and removing variables
        [Test]
        public void SettingAndGettingVariableWithUndefinedValueIsCorrect()
        {
            // Arrange
            const string variableName = "myVar1";
            object input = Undefined.Value;

            // Act
            _jsEngine.SetVariableValue(variableName, input);
            bool variableExists = _jsEngine.HasVariable(variableName);
            var output = _jsEngine.GetVariableValue(variableName);

            // Assert
            Assert.IsFalse(variableExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingVariableWithNullValueIsCorrect()
        {
            // Arrange
            const string variableName = "myVar2";
            const object input = null;

            // Act
            _jsEngine.SetVariableValue(variableName, input);
            bool variableExists = _jsEngine.HasVariable(variableName);
            var output = _jsEngine.GetVariableValue(variableName);

            // Assert
            Assert.IsTrue(variableExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingVariableWithBooleanValueIsCorrect()
        {
            // Arrange
            const string variableName = "isVisible";

            const bool input1 = true;
            const bool targetOutput1 = false;

            const bool input2 = true;

            // Act
            _jsEngine.SetVariableValue(variableName, input1);
            bool variableExists = _jsEngine.HasVariable(variableName);
            _jsEngine.Execute(string.Format("{0} = !{0};", variableName));
            var output1 = _jsEngine.GetVariableValue<bool>(variableName);

            _jsEngine.SetVariableValue(variableName, input2);
            var output2 = _jsEngine.GetVariableValue<bool>(variableName);

            // Assert
            Assert.IsTrue(variableExists);
            Assert.AreEqual(targetOutput1, output1);

            Assert.AreEqual(input2, output2);
        }

        [Test]
        public void SettingAndGettingVariableWithIntegerValueIsCorrect()
        {
            // Arrange
            const string variableName = "amount";

            const int input1 = 38;
            const int targetOutput1 = 41;

            const int input2 = 711;

            // Act
            _jsEngine.SetVariableValue(variableName, input1);
            bool variableExists = _jsEngine.HasVariable(variableName);
            _jsEngine.Execute(string.Format("{0} += 3;", variableName));
            var output1 = _jsEngine.GetVariableValue<int>(variableName);

            _jsEngine.SetVariableValue(variableName, input2);
            var output2 = _jsEngine.GetVariableValue<int>(variableName);

            // Assert
            Assert.IsTrue(variableExists);
            Assert.AreEqual(targetOutput1, output1);

            Assert.AreEqual(input2, output2);
        }
        
        [Test]
        public void SettingAndGettingVariableWithDoubleValueIsCorrect()
        {
            // Arrange
            const string variableName = "price";

            const double input1 = 2.20;
            const double targetOutput1 = 2.17;

            const double input2 = 3.50;

            // Act
            _jsEngine.SetVariableValue(variableName, input1);
            bool variableExists = _jsEngine.HasVariable(variableName);
            _jsEngine.Execute(string.Format("{0} -= 0.03;", variableName));
            var output1 = Math.Round(_jsEngine.GetVariableValue<double>(variableName), 2);

            _jsEngine.SetVariableValue(variableName, input2);
            var output2 = Math.Round(_jsEngine.GetVariableValue<double>(variableName), 2);

            // Assert
            Assert.IsTrue(variableExists);
            Assert.AreEqual(targetOutput1, output1);

            Assert.AreEqual(input2, output2);
        }

        [Test]
        public void SettingAndGettingVariableWithStringValueIsCorrect()
        {
            // Arrange
            const string variableName = "word";

            const string input1 = "Hooray";
            const string targetOutput1 = "Hooray!";

            const string input2 = "Hurrah";

            // Act
            _jsEngine.SetVariableValue(variableName, input1);
            bool variableExists = _jsEngine.HasVariable(variableName);
            _jsEngine.Execute(string.Format("{0} += '!';", variableName));
            var output1 = _jsEngine.GetVariableValue<string>(variableName);

            _jsEngine.SetVariableValue(variableName, input2);
            var output2 = _jsEngine.GetVariableValue<string>(variableName);

            // Assert
            Assert.IsTrue(variableExists);
            Assert.AreEqual(targetOutput1, output1);

            Assert.AreEqual(input2, output2);
        }

        [Test]
        public void RemovingVariableIsCorrect()
        {
            // Arrange
            const string variableName = "price";
            const double input = 120.55;

            // Act
            _jsEngine.SetVariableValue(variableName, input);
            bool variableBeforeRemovingExists = _jsEngine.HasVariable(variableName);
            _jsEngine.RemoveVariable(variableName);
            bool variableAfterRemovingExists = _jsEngine.HasVariable(variableName);

            // Assert
            Assert.IsTrue(variableBeforeRemovingExists);
            Assert.IsFalse(variableAfterRemovingExists);
        }
        #endregion

        #region Getting, setting and removing properties
        [Test]
        public void SettingAndGettingPropertyWithUndefinedValue()
        {
            // Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.name";
            object input = Undefined.Value;

            // Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue(objectName, propertyName);

            // Assert
            Assert.IsFalse(propertyExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingPropertyWithNullValue()
        {
            // Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.name";
            const string input = null;

            // Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue(objectName, propertyName);

            // Assert
            Assert.IsTrue(propertyExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingPropertyWithBooleanValue()
        {
            // Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.isVisible";
            const bool input = true;

            // Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue<bool>(objectName, propertyName);

            // Assert
            Assert.IsTrue(propertyExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingPropertyWithIntegerValue()
        {
            // Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.amount";
            const int input = 38;

            // Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue<int>(objectName, propertyName);

            // Assert
            Assert.IsTrue(propertyExists);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void SettingAndGettingPropertyWithDoubleValue()
		{
			// Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.price";
            const double input = 120.55;

			// Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue<double>(objectName, propertyName);

			// Assert
            Assert.IsTrue(propertyExists);
            Assert.AreEqual(input, output);
		}

        [Test]
        public void SettingAndGettingPropertyWithStringValue()
        {
            // Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.partNumber";
            const string input = "BT-342011";

            // Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool propertyExists = _jsEngine.HasProperty(objectName, propertyName);
            var output = _jsEngine.GetPropertyValue<string>(objectName, propertyName);

            // Assert
            Assert.IsTrue(propertyExists);
            Assert.AreEqual(input, output);
        }

		[Test]
		public void RemovingPropertyIsCorrect()
		{
			// Arrange
            const string objectName = "shop";
            const string propertyName = "products.headphones.price";
            const double input = 120.55;

			// Act
            _jsEngine.SetPropertyValue(objectName, propertyName, input);
            bool priceBeforeRemovingExists = _jsEngine.HasProperty(objectName, propertyName);
            _jsEngine.RemoveProperty(objectName, propertyName);
            bool priceAfterRemovingExists = _jsEngine.HasProperty(objectName, propertyName);

			// Assert
			Assert.IsTrue(priceBeforeRemovingExists);
			Assert.IsFalse(priceAfterRemovingExists);
		}
		#endregion

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (_jsEngine != null)
			{
				_jsEngine.Dispose();
				_jsEngine = null;
			}
		}
	}
}
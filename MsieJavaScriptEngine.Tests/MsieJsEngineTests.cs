namespace MsieJavaScriptEngine.Tests
{
	using MsieJavaScriptEngine;
	using NUnit.Framework;

	[TestFixture]
	public class MsieJsEngineTests
	{
		private MsieJsEngine _msieJsEngine;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_msieJsEngine = new MsieJsEngine(true, true);
		}

		[Test]
		public void NameFormatIsCorrect()
		{
			// Arrange

			// Act
			bool name1FormatIsCorrect = _msieJsEngine.CheckNameFormat("good_parts");
			bool name2FormatIsCorrect = _msieJsEngine.CheckNameFormat("i18n");
			bool name3FormatIsCorrect = _msieJsEngine.CheckNameFormat("fooBar");
			bool name4FormatIsCorrect = _msieJsEngine.CheckNameFormat("$grid");
			bool name5FormatIsCorrect = _msieJsEngine.CheckNameFormat("a");

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
			bool name1FormatIsCorrect = _msieJsEngine.CheckNameFormat("good-parts");
			bool name2FormatIsCorrect = _msieJsEngine.CheckNameFormat("1sale");
			bool name3FormatIsCorrect = _msieJsEngine.CheckNameFormat("Foo Bar");
			bool name4FormatIsCorrect = _msieJsEngine.CheckNameFormat("@grid");
			bool name5FormatIsCorrect = _msieJsEngine.CheckNameFormat("2");

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
			bool propertyName1IsCorrect = _msieJsEngine.CheckPropertyNameFormat("good_parts");
			bool propertyName2IsCorrect = _msieJsEngine.CheckPropertyNameFormat("good_parts.list");
			bool propertyName3IsCorrect = _msieJsEngine.CheckPropertyNameFormat("Products.Product.Price");
			bool propertyName4IsCorrect = _msieJsEngine.CheckPropertyNameFormat("$grid.rows.cells");
			bool propertyName5IsCorrect = _msieJsEngine.CheckPropertyNameFormat("a");

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
			bool propertyName1IsCorrect = _msieJsEngine.CheckPropertyNameFormat("bad-parts");
			bool propertyName2IsCorrect = _msieJsEngine.CheckPropertyNameFormat("bad-parts.list");
			bool propertyName3IsCorrect = _msieJsEngine.CheckPropertyNameFormat("Products Product.Price");
			bool propertyName4IsCorrect = _msieJsEngine.CheckPropertyNameFormat("@grid.rows.cells");
			bool propertyName5IsCorrect = _msieJsEngine.CheckPropertyNameFormat("@a.b.c");

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
			bool name1IsAllowed = _msieJsEngine.CheckNameAllowability("page_total");
			bool name2IsAllowed = _msieJsEngine.CheckNameAllowability("L10n");
			bool name3IsAllowed = _msieJsEngine.CheckNameAllowability("jQuery");
			bool name4IsAllowed = _msieJsEngine.CheckNameAllowability("$pager");

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
				isAllowed = _msieJsEngine.CheckNameAllowability(forbiddenName);
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
			bool propertyName1IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("annual_report");
			bool propertyName2IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("annual_report.total_sum");
			bool propertyName3IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("jQuery.extend");
			bool propertyName4IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("$pager.buttons.nextPageButton");
			bool propertyName5IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("b.a.c.e.d");

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
			bool propertyName1IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("debugger");
			bool propertyName2IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("annual_report.export");
			bool propertyName3IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("jQuery.extends");
			bool propertyName4IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("this.$pager.nextPage.goto");
			bool propertyName5IsAllowed = _msieJsEngine.CheckPropertyNameAllowability("b.a.do.e.in");

			// Assert
			Assert.IsFalse(propertyName1IsAllowed);
			Assert.IsFalse(propertyName2IsAllowed);
			Assert.IsFalse(propertyName3IsAllowed);
			Assert.IsFalse(propertyName4IsAllowed);
			Assert.IsFalse(propertyName5IsAllowed);
		}

		[Test]
		public void EvaluationOfExpressionIsCorrect()
		{
			// Arrange

			// Act
			var result = _msieJsEngine.Evaluate<int>("7 * 8 - 20");

			// Assert
			Assert.AreEqual(36, result);
		}

		[Test]
		public void ExecutionOfCodeAndCallingOfFunctionIsCorrect()
		{
			// Arrange
			const string jsLibraryCode = @"function add(num1, num2) {
				return (num1 + num2);
			}";

			_msieJsEngine.Execute(jsLibraryCode);

			// Act
			var result = _msieJsEngine.CallFunction<int>("add", 2, 3);

			// Assert
			Assert.AreEqual(5, result);
		}

		[Test]
		public void SettingVariableIsCorrect()
		{
			// Arrange
			_msieJsEngine.SetVariableValue("price", 120.55m);

			// Act
			bool priceExists = _msieJsEngine.HasVariable("price");
			var price = _msieJsEngine.GetVariableValue<double>("price");

			// Assert
			Assert.IsTrue(priceExists);
			Assert.AreEqual(120.55m, price);
		}

		[Test]
		public void RemovingVariableIsCorrect()
		{
			// Arrange
			_msieJsEngine.SetVariableValue("price", 120.55m);

			// Act
			bool priceBeforeRemovingExists = _msieJsEngine.HasVariable("price");
			_msieJsEngine.RemoveVariable("price");
			bool priceAfterRemovingExists = _msieJsEngine.HasVariable("price");

			// Assert
			Assert.IsTrue(priceBeforeRemovingExists);
			Assert.IsFalse(priceAfterRemovingExists);
		}

		[Test]
		public void SettingPropertyIsCorrect()
		{
			// Arrange
			_msieJsEngine.SetPropertyValue("shop", "products.product.price", 120.55m);

			// Act
			bool priceExists = _msieJsEngine.HasProperty("shop", "products.product.price");
			var price = _msieJsEngine.GetPropertyValue<decimal>("shop", "products.product.price");

			// Assert
			Assert.IsTrue(priceExists);
			Assert.AreEqual(120.55m, price);
		}

		[Test]
		public void RemovingPropertyIsCorrect()
		{
			// Arrange
			_msieJsEngine.SetPropertyValue("shop", "products.product.price", 120.55m);

			// Act
			bool priceBeforeRemovingExists = _msieJsEngine.HasProperty("shop", "products.product.price");
			_msieJsEngine.RemoveProperty("shop", "products.product.price");
			bool priceAfterRemovingExists = _msieJsEngine.HasProperty("shop", "products.product.price");

			// Assert
			Assert.IsTrue(priceBeforeRemovingExists);
			Assert.IsFalse(priceAfterRemovingExists);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (_msieJsEngine != null)
			{
				_msieJsEngine.Dispose();
				_msieJsEngine = null;
			}
		}
	}
}

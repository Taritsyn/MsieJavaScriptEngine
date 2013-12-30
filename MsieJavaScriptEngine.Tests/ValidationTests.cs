namespace MsieJavaScriptEngine.Tests
{
	using NUnit.Framework;

	using Helpers;

	[TestFixture]
	public class ValidationTests
	{
		[Test]
		public void NameFormatIsCorrect()
		{
			// Arrange

			// Act
			bool name1FormatIsCorrect = ValidationHelpers.CheckNameFormat("good_parts");
			bool name2FormatIsCorrect = ValidationHelpers.CheckNameFormat("i18n");
			bool name3FormatIsCorrect = ValidationHelpers.CheckNameFormat("fooBar");
			bool name4FormatIsCorrect = ValidationHelpers.CheckNameFormat("$grid");
			bool name5FormatIsCorrect = ValidationHelpers.CheckNameFormat("a");

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
			bool name1FormatIsCorrect = ValidationHelpers.CheckNameFormat("good-parts");
			bool name2FormatIsCorrect = ValidationHelpers.CheckNameFormat("1sale");
			bool name3FormatIsCorrect = ValidationHelpers.CheckNameFormat("Foo Bar");
			bool name4FormatIsCorrect = ValidationHelpers.CheckNameFormat("@grid");
			bool name5FormatIsCorrect = ValidationHelpers.CheckNameFormat("2");

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
			bool propertyName1IsCorrect = ValidationHelpers.CheckPropertyNameFormat("good_parts");
			bool propertyName2IsCorrect = ValidationHelpers.CheckPropertyNameFormat("good_parts.list");
			bool propertyName3IsCorrect = ValidationHelpers.CheckPropertyNameFormat("Products.Product.Price");
			bool propertyName4IsCorrect = ValidationHelpers.CheckPropertyNameFormat("$grid.rows.cells");
			bool propertyName5IsCorrect = ValidationHelpers.CheckPropertyNameFormat("a");

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
			bool propertyName1IsCorrect = ValidationHelpers.CheckPropertyNameFormat("bad-parts");
			bool propertyName2IsCorrect = ValidationHelpers.CheckPropertyNameFormat("bad-parts.list");
			bool propertyName3IsCorrect = ValidationHelpers.CheckPropertyNameFormat("Products Product.Price");
			bool propertyName4IsCorrect = ValidationHelpers.CheckPropertyNameFormat("@grid.rows.cells");
			bool propertyName5IsCorrect = ValidationHelpers.CheckPropertyNameFormat("@a.b.c");

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
			bool name1IsAllowed = ValidationHelpers.CheckNameAllowability("page_total");
			bool name2IsAllowed = ValidationHelpers.CheckNameAllowability("L10n");
			bool name3IsAllowed = ValidationHelpers.CheckNameAllowability("jQuery");
			bool name4IsAllowed = ValidationHelpers.CheckNameAllowability("$pager");

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

			foreach (string forbiddenName in forbiddenNames)
			{
				isAllowed = ValidationHelpers.CheckNameAllowability(forbiddenName);
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
			bool propertyName1IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("annual_report");
			bool propertyName2IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("annual_report.total_sum");
			bool propertyName3IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("jQuery.extend");
			bool propertyName4IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("$pager.buttons.nextPageButton");
			bool propertyName5IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("b.a.c.e.d");

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
			bool propertyName1IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("debugger");
			bool propertyName2IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("annual_report.export");
			bool propertyName3IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("jQuery.extends");
			bool propertyName4IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("this.$pager.nextPage.goto");
			bool propertyName5IsAllowed = ValidationHelpers.CheckPropertyNameAllowability("b.a.do.e.in");

			// Assert
			Assert.IsFalse(propertyName1IsAllowed);
			Assert.IsFalse(propertyName2IsAllowed);
			Assert.IsFalse(propertyName3IsAllowed);
			Assert.IsFalse(propertyName4IsAllowed);
			Assert.IsFalse(propertyName5IsAllowed);
		}
	}
}
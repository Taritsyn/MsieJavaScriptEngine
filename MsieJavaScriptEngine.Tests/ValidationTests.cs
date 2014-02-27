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
	}
}
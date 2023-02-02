using NUnit.Framework;

using MsieJavaScriptEngine.Helpers;

namespace MsieJavaScriptEngine.Test.Common
{
	[TestFixture]
	public class ValidationTests
	{
		[Test]
		public void CheckingOfCorrectNameFormat()
		{
			// Arrange

			// Act
			bool name1FormatIsCorrect = ValidationHelpers.CheckNameFormat("good_parts");
			bool name2FormatIsCorrect = ValidationHelpers.CheckNameFormat("i18n");
			bool name3FormatIsCorrect = ValidationHelpers.CheckNameFormat("fooBar");
			bool name4FormatIsCorrect = ValidationHelpers.CheckNameFormat("$grid");
			bool name5FormatIsCorrect = ValidationHelpers.CheckNameFormat("a");
			bool name6FormatIsCorrect = ValidationHelpers.CheckNameFormat("À_la_maison");

			// Assert
			Assert.IsTrue(name1FormatIsCorrect);
			Assert.IsTrue(name2FormatIsCorrect);
			Assert.IsTrue(name3FormatIsCorrect);
			Assert.IsTrue(name4FormatIsCorrect);
			Assert.IsTrue(name5FormatIsCorrect);
			Assert.IsTrue(name6FormatIsCorrect);
		}

		[Test]
		public void CheckingOfWrongNameFormat()
		{
			// Arrange

			// Act
			bool name1FormatIsWrong = ValidationHelpers.CheckNameFormat("good-parts");
			bool name2FormatIsWrong = ValidationHelpers.CheckNameFormat("1sale");
			bool name3FormatIsWrong = ValidationHelpers.CheckNameFormat("Foo Bar");
			bool name4FormatIsWrong = ValidationHelpers.CheckNameFormat("@grid");
			bool name5FormatIsWrong = ValidationHelpers.CheckNameFormat("2");

			// Assert
			Assert.IsFalse(name1FormatIsWrong);
			Assert.IsFalse(name2FormatIsWrong);
			Assert.IsFalse(name3FormatIsWrong);
			Assert.IsFalse(name4FormatIsWrong);
			Assert.IsFalse(name5FormatIsWrong);
		}

		[Test]
		public void CheckingOfCorrectDocumentNameFormat()
		{
			// Arrange

			// Act
			bool documentName1FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat("Script Document");
			bool documentName2FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat("Script Document [2]");
			bool documentName3FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat("doc01.js");
			bool documentName4FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat("/res/scripts.min.js");
			bool documentName5FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat(
				@"C:\Users\Vasya\AppData\Roaming\npm\node_modules\typescript\lib\tsc.js");
			bool documentName6FormatIsCorrect = ValidationHelpers.CheckDocumentNameFormat(
				"BundleTransformer.Less.Resources.less-combined.min.js");

			// Assert
			Assert.IsTrue(documentName1FormatIsCorrect);
			Assert.IsTrue(documentName2FormatIsCorrect);
			Assert.IsTrue(documentName3FormatIsCorrect);
			Assert.IsTrue(documentName4FormatIsCorrect);
			Assert.IsTrue(documentName5FormatIsCorrect);
			Assert.IsTrue(documentName6FormatIsCorrect);
		}

		[Test]
		public void CheckingOfWrongDocumentNameFormat()
		{
			// Arrange

			// Act
			bool documentName1FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat("Script	Document");
			bool documentName2FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat("Script Document <2>");
			bool documentName3FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat(" doc01.js");
			bool documentName4FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat(@"Document ""Test""");
			bool documentName5FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat("src/*.js");
			bool documentName6FormatIsWrong = ValidationHelpers.CheckDocumentNameFormat(
				"/js/shared/SubScribeModal/subscribeChecker.js?v=2017-11-09");

			// Assert
			Assert.IsFalse(documentName1FormatIsWrong);
			Assert.IsFalse(documentName2FormatIsWrong);
			Assert.IsFalse(documentName3FormatIsWrong);
			Assert.IsFalse(documentName4FormatIsWrong);
			Assert.IsFalse(documentName5FormatIsWrong);
			Assert.IsFalse(documentName6FormatIsWrong);
		}
	}
}
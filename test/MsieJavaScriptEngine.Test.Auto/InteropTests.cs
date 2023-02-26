using NUnit.Framework;

using MsieJavaScriptEngine.Test.Common;

namespace MsieJavaScriptEngine.Test.Auto
{
	[TestFixture]
	public class InteropTests : InteropTestsBase
	{
		protected override JsEngineMode EngineMode => JsEngineMode.Auto;


		#region Embedding of objects

		#region Objects with methods

		public override void EmbeddingOfInstanceOfCustomValueTypeAndCallingOfItsGetTypeMethod()
		{ }

		public override void EmbeddingOfInstanceOfCustomReferenceTypeAndCallingOfItsGetTypeMethod()
		{ }

		#endregion

		#region Delegates

		public override void EmbeddingOfInstanceOfDelegateAndCheckingItsPrototype()
		{ }

		public override void EmbeddingOfInstanceOfDelegateAndGettingItsMethodProperty()
		{ }

		#endregion

		#endregion


		#region Embedding of types

		#region Creating of instances

		public override void CreatingAnInstanceOfEmbeddedBuiltinExceptionAndGettingItsTargetSiteProperty()
		{ }

		public override void CreatingAnInstanceOfEmbeddedCustomExceptionAndCallingOfItsGetTypeMethod()
		{ }

		#endregion

		#endregion
	}
}
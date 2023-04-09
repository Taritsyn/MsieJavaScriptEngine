namespace MsieJavaScriptEngine.Test.Common.Interop
{
	public class SomeClass
	{
		public int SomeProperty { get; set; } = 123;
		public string SomeOtherProperty { get; set; } = "abc";

		public static SomeClass Instance { get; } = new SomeClass();
	}
}
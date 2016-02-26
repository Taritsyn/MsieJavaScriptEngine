namespace MsieJavaScriptEngine.Test.Common.Interop
{
	public class SimpleSingleton
	{
		public static readonly SimpleSingleton Instance = new SimpleSingleton();


		private SimpleSingleton()
		{ }


		public override string ToString()
		{
			return "[simple singleton]";
		}
	}
}
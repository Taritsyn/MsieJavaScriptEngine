using System;

namespace MsieJavaScriptEngine.Test.Common.Interop
{
	public struct Age
	{
		public readonly int Year;


		public Age(int year)
		{
			Year = year;
		}


		public override string ToString()
		{
			int age = DateTime.Now.Year - Year;

			return age.ToString();
		}
	}
}
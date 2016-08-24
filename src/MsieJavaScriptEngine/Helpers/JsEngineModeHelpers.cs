using System;

namespace MsieJavaScriptEngine.Helpers
{
	internal static class JsEngineModeHelpers
	{
		public static string GetModeName(JsEngineMode mode)
		{
			string modeName;

			switch (mode)
			{
				case JsEngineMode.Classic:
					modeName = "Classic";
					break;
				case JsEngineMode.ChakraActiveScript:
					modeName = "Chakra ActiveScript";
					break;
				case JsEngineMode.ChakraIeJsRt:
					modeName = "Chakra IE JsRT";
					break;
				case JsEngineMode.ChakraEdgeJsRt:
					modeName = "Chakra Edge JsRT";
					break;
				default:
					throw new NotSupportedException();
			}

			return modeName;
		}
	}
}
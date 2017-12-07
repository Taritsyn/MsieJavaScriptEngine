#if !NETSTANDARD
namespace MsieJavaScriptEngine.Constants
{
	/// <summary>
	/// COM error codes
	/// </summary>
	internal static class ComErrorCode
	{
		// ReSharper disable InconsistentNaming
		public const int SEVERITY_SUCCESS = 0;
		public const int SEVERITY_ERROR = 1;

		public const int FACILITY_NULL = 0;
		public const int FACILITY_RPC = 1;
		public const int FACILITY_DISPATCH = 2;
		public const int FACILITY_STORAGE = 3;
		public const int FACILITY_ITF = 4;
		public const int FACILITY_WIN32 = 7;
		public const int FACILITY_WINDOWS = 8;
		public const int FACILITY_CONTROL = 10;
		public const int FACILITY_INTERNET = 12;
		public const int FACILITY_URT = 19;

		public const int S_OK = 0;
		public const int S_FALSE = 1;

		public const int E_ABORT = unchecked((int)0x80004004);
		public const int E_ELEMENT_NOT_FOUND = unchecked((int)0x8002802B);
		public const int E_CLASS_NOT_REGISTERED = unchecked((int)0x80040154);
		// ReSharper restore InconsistentNaming
	}
}
#endif
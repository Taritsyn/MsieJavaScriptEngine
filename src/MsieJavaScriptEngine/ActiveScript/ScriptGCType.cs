﻿#if NETFRAMEWORK
namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// The type of garbage collection to perform
	/// </summary>
	internal enum ScriptGCType
	{
		/// <summary>
		/// Do normal garbage collection
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Do exhaustive garbage collection
		/// </summary>
		Exhaustive = 1
	}
}
#endif
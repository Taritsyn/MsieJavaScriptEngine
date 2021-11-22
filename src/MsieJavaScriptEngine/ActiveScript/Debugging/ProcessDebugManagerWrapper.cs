#if NETFRAMEWORK
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Wrapper for process debug manager
	/// </summary>
	internal sealed class ProcessDebugManagerWrapper
	{
		/// <summary>
		/// Flag that the current process debug manager is a 64-bit
		/// </summary>
		private readonly bool _is64Bit;

		/// <summary>
		/// Instance of 32-bit process debug manager
		/// </summary>
		private readonly IProcessDebugManager32 _processDebugManager32;

		/// <summary>
		/// Instance of 64-bit process debug manager
		/// </summary>
		private readonly IProcessDebugManager64 _processDebugManager64;


		/// <summary>
		/// Constructs an instance of the wrapper for process debug manager
		/// </summary>
		/// <param name="processDebugManager">Instance of 32-bit process debug manager</param>
		public ProcessDebugManagerWrapper(IProcessDebugManager32 processDebugManager)
		{
			_is64Bit = false;
			_processDebugManager32 = processDebugManager;
		}

		/// <summary>
		/// Constructs an instance of the wrapper for process debug manager
		/// </summary>
		/// <param name="processDebugManager">Instance of 64-bit process debug manager</param>
		public ProcessDebugManagerWrapper(IProcessDebugManager64 processDebugManager)
		{
			_is64Bit = true;
			_processDebugManager64 = processDebugManager;
		}


		/// <summary>
		/// Creates a wrapper for the process debug manager. A return value indicates whether
		/// the creation succeeded.
		/// </summary>
		/// <param name="wrapper">Wrapper for process debug manager</param>
		/// <returns>true if the wrapper was created successfully; otherwise, false.</returns>
		public static bool TryCreate(out ProcessDebugManagerWrapper wrapper)
		{
			const string progId = "ProcessDebugManager";

			if (Utils.Is64BitProcess())
			{
				IProcessDebugManager64 processDebugManager64;
				if (ComHelpers.TryCreateComObject(progId, null, out processDebugManager64))
				{
					wrapper = new ProcessDebugManagerWrapper(processDebugManager64);
					return true;
				}
			}
			else
			{
				IProcessDebugManager32 processDebugManager32;
				if (ComHelpers.TryCreateComObject(progId, null, out processDebugManager32))
				{
					wrapper = new ProcessDebugManagerWrapper(processDebugManager32);
					return true;
				}
			}

			wrapper = null;
			return false;
		}

		/// <summary>
		/// Creates a wrapper for new debug application
		/// </summary>
		/// <param name="applicationWrapper">The wrapper for new debug application</param>
		public void CreateApplication(out DebugApplicationWrapper applicationWrapper)
		{
			if (_is64Bit)
			{
				IDebugApplication64 debugApplication64;
				_processDebugManager64.CreateApplication(out debugApplication64);
				applicationWrapper = new DebugApplicationWrapper(debugApplication64);
			}
			else
			{
				IDebugApplication32 debugApplication32;
				_processDebugManager32.CreateApplication(out debugApplication32);
				applicationWrapper = new DebugApplicationWrapper(debugApplication32);
			}
		}

		/// <summary>
		/// Adds a new debug application. A return value indicates whether the adding succeeded.
		/// </summary>
		/// <param name="applicationWrapper">Wrapper for debug application</param>
		/// <param name="cookie">An engine-defined cookie</param>
		/// <returns>true if the debug application was added successfully; otherwise, false.</returns>
		public bool TryAddApplication(DebugApplicationWrapper applicationWrapper, out uint cookie)
		{
			uint result;

			if (_is64Bit)
			{
				IDebugApplication64 application64 = applicationWrapper.DebugApplication64;
				result = _processDebugManager64.AddApplication(application64, out cookie);
			}
			else
			{
				IDebugApplication32 application32 = applicationWrapper.DebugApplication32;
				result = _processDebugManager32.AddApplication(application32, out cookie);
			}

			bool isSucceeded = ComHelpers.HResult.Succeeded(result);

			return isSucceeded;
		}

		/// <summary>
		/// Removes a debug application
		/// </summary>
		/// <param name="cookie">The cookie of the debug application to remove</param>
		public void RemoveApplication(uint cookie)
		{
			if (_is64Bit)
			{
				_processDebugManager64.RemoveApplication(cookie);
			}
			else
			{
				_processDebugManager32.RemoveApplication(cookie);
			}
		}
	}
}
#endif
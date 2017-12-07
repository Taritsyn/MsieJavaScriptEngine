#if !NETSTANDARD
namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Wrapper for debug application
	/// </summary>
	internal sealed class DebugApplicationWrapper
	{
		/// <summary>
		/// Flag that the current debug application is a 64-bit
		/// </summary>
		private readonly bool _is64Bit;

		/// <summary>
		/// Instance of 32-bit debug application
		/// </summary>
		private readonly IDebugApplication32 _debugApplication32;

		/// <summary>
		/// Instance of 64-bit debug application
		/// </summary>
		private readonly IDebugApplication64 _debugApplication64;

		/// <summary>
		/// Gets a instance of 32-bit debug application
		/// </summary>
		public IDebugApplication32 DebugApplication32
		{
			get { return _debugApplication32; }
		}

		/// <summary>
		/// Gets a instance of 64-bit debug application
		/// </summary>
		public IDebugApplication64 DebugApplication64
		{
			get { return _debugApplication64; }
		}


		/// <summary>
		/// Constructs an instance of the wrapper for debug application
		/// </summary>
		/// <param name="debugApplication">Instance of 32-bit debug application</param>
		public DebugApplicationWrapper(IDebugApplication32 debugApplication)
		{
			_is64Bit = false;
			_debugApplication32 = debugApplication;
		}

		/// <summary>
		/// Constructs an instance of the wrapper for debug application
		/// </summary>
		/// <param name="debugApplication">Instance of 64-bit debug application</param>
		public DebugApplicationWrapper(IDebugApplication64 debugApplication)
		{
			_is64Bit = true;
			_debugApplication64 = debugApplication;
		}


		/// <summary>
		/// Sets the name of the application
		/// </summary>
		/// <param name="name">The name of the application</param>
		public void SetName(string name)
		{
			if (_is64Bit)
			{
				_debugApplication64.SetName(name);
			}
			else
			{
				_debugApplication32.SetName(name);
			}
		}

		/// <summary>
		/// Returns the application node under which all nodes associated with the application are added
		/// </summary>
		/// <param name="node">The debug application node under which all nodes associated with
		/// the application are added</param>
		public void GetRootNode(out IDebugApplicationNode node)
		{
			if (_is64Bit)
			{
				_debugApplication64.GetRootNode(out node);
			}
			else
			{
				_debugApplication32.GetRootNode(out node);
			}
		}

		/// <summary>
		/// Creates a new application node that is associated with a specific document provider
		/// </summary>
		/// <param name="node">The application node associated with this document provider</param>
		public void CreateApplicationNode(out IDebugApplicationNode node)
		{
			if (_is64Bit)
			{
				_debugApplication64.CreateApplicationNode(out node);
			}
			else
			{
				_debugApplication32.CreateApplicationNode(out node);
			}
		}

		/// <summary>
		/// Returns the current debugger connected to the application
		/// </summary>
		/// <param name="debugger">The current debugger connected to the application</param>
		/// <returns>The method returns an HRESULT</returns>
		public uint GetDebugger(out IApplicationDebugger debugger)
		{
			uint result = _is64Bit ?
				_debugApplication64.GetDebugger(out debugger)
				:
				_debugApplication32.GetDebugger(out debugger)
				;

			return result;
		}

		/// <summary>
		/// Causes this application to release all references and enter an inactive state
		/// </summary>
		public void Close()
		{
			if (_is64Bit)
			{
				_debugApplication64.Close();
			}
			else
			{
				_debugApplication32.Close();
			}
		}
	}
}
#endif
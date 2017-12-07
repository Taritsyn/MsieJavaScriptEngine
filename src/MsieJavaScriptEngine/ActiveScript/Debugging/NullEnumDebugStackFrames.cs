#if !NETSTANDARD
using System;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	internal class NullEnumDebugStackFrames : IEnumDebugStackFrames
	{
		#region IEnumDebugStackFrames implementation

		public void Next(uint count, out DebugStackFrameDescriptor descriptor, out uint countFetched)
		{
			descriptor = default(DebugStackFrameDescriptor);
			countFetched = 0;
		}

		public void Skip(uint count)
		{ }

		public void Reset()
		{ }

		public void Clone(out IEnumDebugStackFrames enumFrames)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
#endif
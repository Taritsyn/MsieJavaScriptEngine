namespace MsieJavaScriptEngine.Helpers
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	/// COM helpers
	/// </summary>
	internal static class ComHelpers
	{
		public static IntPtr CreateInstanceByClsid<T>(string clsid)
		{
			IntPtr pInterface = CreateInstanceByClsid<T>(Guid.Parse(clsid));

			return pInterface;
		}

		public static IntPtr CreateInstanceByClsid<T>(Guid clsid)
		{
			IntPtr pInterface;
			Guid iid = typeof(T).GUID;
			HResult.Check(NativeMethods.CoCreateInstance(ref clsid, IntPtr.Zero, 1, ref iid, out pInterface));

			return pInterface;
		}

		public static IntPtr CreateInstanceByProgId<T>(string progId)
		{
			Guid clsid = ClsidFromProgId(progId);
			IntPtr pInterface = CreateInstanceByClsid<T>(clsid);
			
			return pInterface;
		}

		private static Guid ClsidFromProgId(string progId)
		{
			Guid clsid;

			if (!Guid.TryParseExact(progId, "B", out clsid))
			{
				HResult.Check(NativeMethods.CLSIDFromProgID(progId, out clsid));
			}

			return clsid;
		}

		public static IntPtr QueryInterface<T>(IntPtr pUnknown)
		{
			IntPtr pInterface;
			Guid iid = typeof(T).GUID;

			HResult.Check(Marshal.QueryInterface(pUnknown, ref iid, out pInterface));

			return pInterface;
		}

		public static void ReleaseAndEmpty(ref IntPtr pUnk)
		{
			if (pUnk != IntPtr.Zero)
			{
				Marshal.Release(pUnk);
				pUnk = IntPtr.Zero;
			}
		}

		public static void ReleaseComObject<T>(ref T obj, bool final = false) where T : class
		{
			if (obj != null && Marshal.IsComObject(obj))
			{
				if (final)
				{
					Marshal.FinalReleaseComObject(obj);
				}
				else
				{
					Marshal.ReleaseComObject(obj);
				}
			}

			obj = null;
		}

		#region Private methods

		private static int UnsignedAsSigned(uint value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		#endregion

		#region Nested type: NativeMethods

		private static class NativeMethods
		{
			[DllImport("ole32.dll", ExactSpelling = true)]
			public static extern uint CoCreateInstance(
				[In] ref Guid clsid,
				[In] IntPtr pOuter,
				[In] uint clsContext,
				[In] ref Guid iid,
				[Out] out IntPtr pInterface
			);

			[DllImport("ole32.dll", ExactSpelling = true)]
			public static extern uint CLSIDFromProgID(
				[In] [MarshalAs(UnmanagedType.LPWStr)] string progId,
				[Out] out Guid clsid
			);
		}

		#endregion

		#region Nested type: HResult

		public static class HResult
		{
			// ReSharper disable InconsistentNaming
			public const int SEVERITY_SUCCESS = 0;
			public const int SEVERITY_ERROR = 1;
			// ReSharper restore InconsistentNaming

			public static void Check(uint result)
			{
				Check(UnsignedAsSigned(result));
			}

			public static void Check(int result)
			{
				Marshal.ThrowExceptionForHR(result);
			}

			public static int GetSeverity(uint result)
			{
				return GetSeverity(UnsignedAsSigned(result));
			}

			public static int GetSeverity(int result)
			{
				return (result >> 31) & 0x1;
			}

			public static int GetFacility(uint result)
			{
				return GetFacility(UnsignedAsSigned(result));
			}

			public static int GetFacility(int result)
			{
				return (result >> 16) & 0x1FFF;
			}

			public static int GetCode(uint result)
			{
				return GetCode(UnsignedAsSigned(result));
			}

			public static int GetCode(int result)
			{
				return result & 0xFFFF;
			}

			public static int MakeResult(int severity, int facility, int code)
			{
				return UnsignedAsSigned((uint)(code & 0xFFFF) | ((uint)(facility & 0x1FFF) << 16) | ((uint)(severity & 0x1) << 31));
			}
		}

		#endregion
	}
}
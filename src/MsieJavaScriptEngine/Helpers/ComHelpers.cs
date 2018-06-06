#if !NETSTANDARD
using System;
#if NET45
using System.Reflection;
#endif
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.Constants;
#if NET40
using MsieJavaScriptEngine.Polyfills.System;
#endif

namespace MsieJavaScriptEngine.Helpers
{
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
			Guid iid = typeof(T).GetTypeInfo().GUID;
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

		public static bool TryCreateComObject<T>(string progId, string serverName, out T obj) where T : class
		{
			Type type;
			if (!TryGetComType(progId, serverName, out type))
			{
				obj = null;
				return false;
			}

			obj = Activator.CreateInstance(type) as T;

			return obj != null;
		}

		public static bool TryGetComType(string progId, string serverName, out Type type)
		{
			Guid clsid;
			type = Guid.TryParseExact(progId, "B", out clsid) ?
				Type.GetTypeFromCLSID(clsid, serverName)
				:
				Type.GetTypeFromProgID(progId, serverName)
				;

			return type != null;
		}

		public static IntPtr QueryInterface<T>(IntPtr pUnknown)
		{
			IntPtr pInterface;
			Guid iid = typeof(T).GetTypeInfo().GUID;

			HResult.Check(Marshal.QueryInterface(pUnknown, ref iid, out pInterface));

			return pInterface;
		}

		public static IntPtr QueryInterfaceNoThrow<T>(IntPtr pUnknown)
		{
			IntPtr pInterface;
			Guid iid = typeof(T).GetTypeInfo().GUID;
			int result = Marshal.QueryInterface(pUnknown, ref iid, out pInterface);

			return result == ComErrorCode.S_OK ? pInterface : IntPtr.Zero;
		}

		public static T GetMethodDelegate<T>(IntPtr pInterface, int methodIndex) where T : class
		{
			var pVTable = Marshal.ReadIntPtr(pInterface);
			var pMethod = Marshal.ReadIntPtr(pVTable + methodIndex * IntPtr.Size);

			return Marshal.GetDelegateForFunctionPointer(pMethod, typeof(T)) as T;
		}

		public static void ReleaseAndEmpty(ref IntPtr pUnk)
		{
			if (pUnk != IntPtr.Zero)
			{
				Marshal.Release(pUnk);
				pUnk = IntPtr.Zero;
			}
		}

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
			public static void Check(uint result)
			{
				Check(NumericHelpers.UnsignedAsSigned(result));
			}

			public static void Check(int result)
			{
				Marshal.ThrowExceptionForHR(result);
			}

			public static bool Succeeded(uint result)
			{
				return GetSeverity(result) == ComErrorCode.SEVERITY_SUCCESS;
			}

			public static int GetSeverity(uint result)
			{
				return GetSeverity(NumericHelpers.UnsignedAsSigned(result));
			}

			public static int GetSeverity(int result)
			{
				return (result >> 31) & 0x1;
			}

			public static int GetFacility(uint result)
			{
				return GetFacility(NumericHelpers.UnsignedAsSigned(result));
			}

			public static int GetFacility(int result)
			{
				return (result >> 16) & 0x1FFF;
			}

			public static int GetCode(uint result)
			{
				return GetCode(NumericHelpers.UnsignedAsSigned(result));
			}

			public static int GetCode(int result)
			{
				return result & 0xFFFF;
			}

			public static int MakeResult(int severity, int facility, int code)
			{
				return NumericHelpers.UnsignedAsSigned((uint)(code & 0xFFFF) | ((uint)(facility & 0x1FFF) << 16) | ((uint)(severity & 0x1) << 31));
			}
		}

		#endregion
	}
}
#endif
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect
{
	public class StructSerializer
	{
		public static byte[] Serialize<T>(T structure) where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));

			byte[] ret = new byte[size];
			IntPtr buff = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(structure, buff, false);
			Marshal.Copy(buff, ret, 0, size);
			Marshal.FreeHGlobal(buff);
			return ret;
		}

		public static T DeserializeStruct<T>(byte[] data) where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));

			IntPtr buff = Marshal.AllocHGlobal(size);
			Marshal.Copy(data, 0, buff, size);

			T ret = (T)Marshal.PtrToStructure(buff, typeof(T));
			Marshal.FreeHGlobal(buff);

			return ret;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect.Messages
{
	[StructLayout(LayoutKind.Explicit)]
	public struct DeviceValue
	{
		[FieldOffset(0)]
		public Int64 Hash;
		[FieldOffset(8)]
		public Int64 IValue;
		[FieldOffset(8)]
		public double DValue;
	}
}

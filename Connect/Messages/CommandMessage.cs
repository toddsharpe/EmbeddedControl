using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect.Messages
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CommandMessage
	{
		public MessageHeader Header;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Device;
		public long IValue;
		public double DValue;
	}
}

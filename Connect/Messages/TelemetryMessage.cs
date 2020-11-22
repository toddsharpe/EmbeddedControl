using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect.Messages
{
	public struct TelemetryEntry
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Device;
		public long IValue;
		public double DValue;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct TelemetryMessage
	{
		public MessageHeader Header;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = EntryLength)]
		public TelemetryEntry[] Entries;

		public const int EntryLength = 4;
	}
}

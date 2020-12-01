using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect.Messages
{
	[StructLayout(LayoutKind.Sequential)]
	public struct TelemetryMessage
	{
		public MessageHeader Header;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = EntryLength)]
		public DeviceValue[] Devices;

		public const int EntryLength = 4;
	}
}

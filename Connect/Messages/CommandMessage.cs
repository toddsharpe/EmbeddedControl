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
		public DeviceValue Device;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Connect.Messages
{
	public enum Result
	{
		Success,
		Failure
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct AckMessage
	{
		public MessageHeader Header;
		public Result Result;
	}
}

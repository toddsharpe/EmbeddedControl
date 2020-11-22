using System;
using System.Collections.Generic;
using System.Text;

namespace Connect.Messages
{
	public struct MessageHeader
	{
		public MessageType Type { get; set; }

		public MessageHeader(MessageType type)
		{
			Type = type;
		}
	}
}

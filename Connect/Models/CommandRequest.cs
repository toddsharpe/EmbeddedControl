using System;
using System.Collections.Generic;
using System.Text;

namespace Connect.Models
{
	public class CommandRequest : Request
	{
		public string Device { get; set; }
		public object Value { get; set; }
	}
}

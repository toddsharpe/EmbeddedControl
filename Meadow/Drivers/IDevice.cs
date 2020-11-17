using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Drivers
{
	interface IDevice
	{
		bool IsOpen { get; }
		event EventHandler DataReceived;
		int Read(byte[] buffer, int offset, int length);
		void Write(byte[] buffer, int offset, int count);
	}
}

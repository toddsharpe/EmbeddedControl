using Meadow.Drivers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.IO
{
	class DeviceStream : Stream
	{
		public override bool CanRead => _device.IsOpen;

		public override bool CanSeek => throw new NotImplementedException();

		public override bool CanWrite => _device.IsOpen;

		public override long Length => throw new NotImplementedException();

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private readonly IDevice _device;
		public DeviceStream(IDevice device)
		{
			_device = device;
		}

		public override void Flush()
		{
			Console.WriteLine("Flush");
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//Console.WriteLine("Read: {0} {1}", offset, count);
			return _device.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			Console.WriteLine("Seek {0}", offset);
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			Console.WriteLine("SetLength {0}", value);
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			//Console.WriteLine("Write: {0} {1}", offset, count);
			_device.Write(buffer, offset, count);
		}
	}
}

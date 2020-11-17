using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Connect
{
	public static class Extensions
	{
		public static Task WriteAsync(this Stream stream, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return stream.WriteAsync(bytes, 0, bytes.Length);
		}

		public static async Task<int> ReadInt32Async(this Stream stream)
		{
			byte[] buffer = new byte[sizeof(int)];
			await stream.ReadAsync(buffer, 0, buffer.Length);
			return BitConverter.ToInt32(buffer, 0);
		}
	}
}

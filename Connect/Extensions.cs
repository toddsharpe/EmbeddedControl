using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

		public static Int64 GetHash64(this string s)
		{
			using (SHA256 hash = new SHA256CryptoServiceProvider())
			{
				byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(s));
				Int64 hashCodeStart = BitConverter.ToInt64(bytes, 0);
				Int64 hashCodeMedium = BitConverter.ToInt64(bytes, 8);
				Int64 hashCodeEnd = BitConverter.ToInt64(bytes, 24);
				Int64 hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
				return hashCode;
			}
		}
	}
}

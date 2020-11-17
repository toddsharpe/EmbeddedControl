using Connect.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect
{
	public static class Transport
	{
		private static readonly byte[] Magic = new[] { (byte)'T', (byte)'S', (byte)0, (byte)0 };

		public static async Task WriteAsync(Stream stream, Message message)
		{
			string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
			byte[] bytes = Encoding.UTF8.GetBytes(json);

			await stream.WriteAsync(Magic, 0, Magic.Length);
			await stream.WriteAsync(json.Length);
			await stream.WriteAsync(bytes, 0, bytes.Length);
			await stream.FlushAsync();
		}

		public static async Task<T> Read<T>(Stream stream) where T : class
		{
			byte[] header = new byte[Magic.Length];
			await stream.ReadAsync(header, 0, header.Length);
			if (!header.SequenceEqual(Magic))
				return default;

			int length = await stream.ReadInt32Async();
			byte[] buffer = new byte[length];
			await stream.ReadAsync(buffer, 0, buffer.Length);
			string json = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
			Console.WriteLine(json);

			return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				SerializationBinder = new ModelSerializationBinder()
			});
		}
	}
}

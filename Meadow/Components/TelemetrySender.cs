using Connect;
using Connect.Config;
using Connect.Messages;
using Meadow.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Components
{
	class TelemetrySender : IComponent
	{
		private readonly Stream _stream;
		private readonly TelemetryConfig _config;
		private readonly List<TelemetryMessage> _messages;

		public int MessagesCount => _messages.Count;
		public int DeviceCount => _config.Count;

		public TelemetrySender(Stream stream, TelemetryConfig config, Layout layout)
		{
			_stream = stream;
			_config = config;
			_messages = new List<TelemetryMessage>();
			layout.Bind("telemetry", this);
		}

		public void Load()
		{
			//Create messages
			int messageCount = (_config.Count + TelemetryMessage.EntryLength - 1) / TelemetryMessage.EntryLength;
			Console.WriteLine("Message Count: " + messageCount);
			for (int i = 0; i < messageCount; i++)
			{
				_messages.Add(new TelemetryMessage
				{
					Header = new MessageHeader(MessageType.Telemetry),
					Devices = new DeviceValue[TelemetryMessage.EntryLength]
				});
			}

			using (SHA256 hash = new SHA256CryptoServiceProvider())
			{
				//Populate by names, with hashcodes
				for (int i = 0; i < _config.Count; i++)
				{
					TelemetryConfigEntry entry = _config[i];

					byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(entry.Name));
					Int64 hashCodeStart = BitConverter.ToInt64(bytes, 0);
					Int64 hashCodeMedium = BitConverter.ToInt64(bytes, 8);
					Int64 hashCodeEnd = BitConverter.ToInt64(bytes, 24);
					Int64 hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

					int messageIndex = i / TelemetryMessage.EntryLength;
					int entryIndex = i % TelemetryMessage.EntryLength;

					_messages[messageIndex].Devices[entryIndex].Hash = hashCode;
					Console.Write("Hashed {0}-{1} to {2}" + Environment.NewLine, i, entry.Name, hashCode);
				}
			}
		}

		public void Dispatch(DeviceLookup lookup)
		{
			//Update values
			DateTime now = DateTime.Now;
			for (int i = 0; i < _config.Count; i++)
			{
				int messageIndex = i / TelemetryMessage.EntryLength;
				int entryIndex = i % TelemetryMessage.EntryLength;

				DeviceValue entry = _messages[messageIndex].Devices[entryIndex];
				object value = lookup[entry.Hash].Get();
				if (lookup[entry.Hash].Property.PropertyType == typeof(Int32))
					_messages[messageIndex].Devices[entryIndex].IValue = (Int32)value;
				else if (lookup[entry.Hash].Property.PropertyType == typeof(float))
					_messages[messageIndex].Devices[entryIndex].DValue = (float)value;
				else
					Console.WriteLine("Unknown Type: {0}", lookup[entry.Hash].Property.PropertyType);
			}

			//Send packets
			foreach (TelemetryMessage message in _messages)
			{
				//var m = message;
				//m.Header.DateTime = now;
				
				//Send telemetry
				byte[] buffer = StructSerializer.Serialize(message);
				_stream.Write(buffer);
			}
		}
	}
}

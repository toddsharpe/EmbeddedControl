using Connect;
using Connect.Messages;
using Meadow.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Meadow.TelemetryConfig;

namespace Meadow.Components
{
	class TelemetrySender : IComponent
	{
		public string Name => "Telemetry";

		private readonly Stream _stream;
		private readonly TelemetryConfig _config;
		private readonly List<TelemetryMessage> _messages;
		public TelemetrySender(Stream stream, TelemetryConfig config)
		{
			_stream = stream;
			_config = config;
			_messages = new List<TelemetryMessage>();
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
					Entries = new TelemetryEntry[TelemetryMessage.EntryLength]
				});
			}

			//Populate by names
			for (int i = 0; i < _config.Count; i++)
			{
				TelemetryConfigEntry entry = _config[i];

				int messageIndex = i / TelemetryMessage.EntryLength;
				int entryIndex = i % TelemetryMessage.EntryLength;

				_messages[messageIndex].Entries[entryIndex].Device = entry.Name;
			}
		}

		public void Dispatch()
		{
			//Update values
			for (int i = 0; i < _config.Count; i++)
			{
				TelemetryConfigEntry configEntry = _config[i];

				int messageIndex = i / TelemetryMessage.EntryLength;
				int entryIndex = i % TelemetryMessage.EntryLength;

				_messages[messageIndex].Entries[entryIndex].DValue = 0;
				_messages[messageIndex].Entries[entryIndex].IValue = 0;

				if (configEntry.GetProperty.PropertyType == typeof(float))
					_messages[messageIndex].Entries[entryIndex].DValue = (float)configEntry.GetProperty.GetValue(configEntry.Component);
				else if (configEntry.GetProperty.PropertyType == typeof(int))
					_messages[messageIndex].Entries[entryIndex].IValue = (int)configEntry.GetProperty.GetValue(configEntry.Component);
				else
					throw new Exception("Invalid config type");
			}

			//Send packets
			foreach (TelemetryMessage message in _messages)
			{
				//Send telemetry
				byte[] buffer = StructSerializer.Serialize(message);
				_stream.Write(buffer);
			}
		}
	}
}

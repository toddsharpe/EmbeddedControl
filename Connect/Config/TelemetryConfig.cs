using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Connect.Config
{
	class TelemetryConfig : List<TelemetryConfigEntry>
	{
		public static readonly TelemetryConfig Meadow = new TelemetryConfig
		{
			new TelemetryConfigEntry("control.CycleCount", TypeCode.Int32),
			new TelemetryConfigEntry("control.DispatchTime", TypeCode.Int32),
			new TelemetryConfigEntry("control.TelemetrySendTime", TypeCode.Int32),
			new TelemetryConfigEntry("control.sensors.Temperature", TypeCode.Single),
			new TelemetryConfigEntry("control.telemetry.MessagesCount", TypeCode.Int32),
			new TelemetryConfigEntry("control.telemetry.DeviceCount", TypeCode.Int32),
		};

		public IEnumerable<Int64> GetHashes()
		{
			using (SHA256 hash = new SHA256CryptoServiceProvider())
			{
				foreach (TelemetryConfigEntry entry in this)
				{
					byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(entry.Name));
					Int64 hashCodeStart = BitConverter.ToInt64(bytes, 0);
					Int64 hashCodeMedium = BitConverter.ToInt64(bytes, 8);
					Int64 hashCodeEnd = BitConverter.ToInt64(bytes, 24);
					Int64 hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
					yield return hashCode;
				}
			}
		}
	}
}

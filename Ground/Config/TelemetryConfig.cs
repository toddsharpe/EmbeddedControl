using System;
using System.Collections.Generic;
using System.Text;

namespace Ground.Config
{
	public static class TelemetryConfig
	{
		public static readonly List<TelemetryConfigEntry> Meadow = new List<TelemetryConfigEntry>
		{
			new TelemetryConfigEntry("Control.CycleCount", TypeCode.Int32),
			new TelemetryConfigEntry("Control.DispatchTime", TypeCode.Int32),
			new TelemetryConfigEntry("Control.TelemetrySendTime", TypeCode.Int32),
			new TelemetryConfigEntry("Sensors.Temperature", TypeCode.Single),
		};
	}

	public class TelemetryConfigEntry
	{
		public string Device { get; set; }
		public TypeCode Type { get; set; }

		public TelemetryConfigEntry(string device, TypeCode type)
		{
			Device = device;
			Type = type;
		}
	}
}

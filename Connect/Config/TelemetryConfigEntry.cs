using System;
using System.Collections.Generic;
using System.Text;

namespace Connect.Config
{
	public class TelemetryConfigEntry
	{
		public string Name { get; }
		public TypeCode TypeCode { get; }

		public TelemetryConfigEntry(string name, TypeCode typeCode)
		{
			Name = name;
			TypeCode = typeCode;
		}
	}
}

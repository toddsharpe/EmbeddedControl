using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Framework;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Atmospheric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Components
{
	class SensorReader : IComponent
	{
		private readonly AnalogTemperature _temperature;

		public float Temperature { get; private set; }

		public SensorReader(F7Micro device, Layout layout)
		{
			_temperature = new AnalogTemperature(device, device.Pins.A00, AnalogTemperature.KnownSensorType.LM35);
			layout.Bind("sensors", this);
		}

		public void Load()
		{

		}

		public async void Dispatch(DeviceLookup lookup = null)
		{
			AtmosphericConditions temp = await _temperature.Read();
			Temperature = temp.Temperature.HasValue ? temp.Temperature.Value : 0;
		}
	}
}

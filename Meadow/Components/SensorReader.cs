using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Framework;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Components
{
	public class SensorReader : IComponent
	{
		private readonly AnalogTemperature _temperature;

		public string Name => "Sensors";
		public float Temperature { get; private set; }

		public SensorReader(F7Micro device)
		{
			_temperature = new AnalogTemperature(device, device.Pins.A00, AnalogTemperature.KnownSensorType.LM35);
		}

		public void Load()
		{
			
		}

		public async void Dispatch()
		{
			var temp = await _temperature.Read();
			Temperature = temp.Temperature.HasValue ? temp.Temperature.Value : 0;
		}
	}
}

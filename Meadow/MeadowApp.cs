using Meadow;
using Meadow.Devices;
using Meadow.Drivers;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Framework;
using Meadow.Gateway.WiFi;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
	public class MeadowApp : App<F7Micro, MeadowApp>, IAsyncApp
	{
		private readonly Hc06 _bluetooth;

		public MeadowApp()
		{
			_bluetooth = new Hc06(Device, Device.SerialPortNames.Com4);
		}

		public async Task Run()
		{
			await _bluetooth.Open();
			Console.WriteLine("Bluetooth baud rate: " + _bluetooth.BaudRate);
			string s = await _bluetooth.GetVersion();
			Console.WriteLine(s);

			_bluetooth.DataReceived += _bluetooth_DataReceived;
		}

		private void _bluetooth_DataReceived(object sender, EventArgs e)
		{
			byte[] data = _bluetooth.ReadAll();
			string s = Encoding.ASCII.GetString(data);
			Console.WriteLine("Received " + s);
		}
	}
}

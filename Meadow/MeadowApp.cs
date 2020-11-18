using Connect;
using Connect.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Drivers;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Framework;
using Meadow.Gateway.WiFi;
using Meadow.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
	public class MeadowApp : App<F7Micro, MeadowApp>
	{
		private readonly Hc06 _bluetooth;

		public MeadowApp()
		{
			_bluetooth = new Hc06(Device, Device.SerialPortNames.Com4);
		}

		public void Run()
		{
			_bluetooth.Open();
			Console.WriteLine("Bluetooth baud rate: " + _bluetooth.BaudRate);
			if (_bluetooth.BaudRate != 921600)
			{
				_bluetooth.SetBaud(921600);
				Console.WriteLine("Bluetooth baud rate: " + _bluetooth.BaudRate);
			}
			Console.WriteLine(_bluetooth.GetVersion());

			_bluetooth.DataReceived += _bluetooth_DataReceived;
		}

		private async void _bluetooth_DataReceived(object sender, EventArgs e)
		{
			DeviceStream stream = new DeviceStream(_bluetooth);
			Message message = await Transport.Read<Message>(stream);

			if (message == null)
			{
				Console.WriteLine("null");
			}
			else
			{
				Console.WriteLine(message.GetType().Name);

				if (message is CommandRequest command)
				{
					Console.WriteLine("command {0} = {1}", command.Device, command.Value);
				}
			}
		}

		private void PrintBytes(byte[] bytes)
		{
			int width = 16;

			Console.Write("\n---- ");
			for (int i = 0; i < width; i++)
			{
				Console.Write("{0:x2} ", i);
			}
			Console.WriteLine();

			for (int i = 0; i < bytes.Length; i++)
			{
				if (i != 0 && i % width == 0)
					Console.WriteLine();

				if (i % width == 0)
					Console.Write("{0:x2} - ", (i / 16) << 4);

				Console.Write("{0:x2} ", bytes[i]);
			}
			Console.WriteLine();
		}
	}
}

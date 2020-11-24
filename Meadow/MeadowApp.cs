using Connect;
using Connect.Config;
using Connect.Messages;
using Meadow;
using Meadow.Components;
using Meadow.Devices;
using Meadow.Drivers;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Framework;
using Meadow.Gateway.WiFi;
using Meadow.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
	class MeadowApp : App<F7Micro, MeadowApp>, IControlApp, IComponent
	{
		private readonly Hc06 _bluetooth;

		private int ControlPeriod = 1000 * 1;

		//Components
		private SensorReader _sensorReader;
		private TelemetrySender _telemetrySender;

		//Root values
		public int CycleCount { get; private set; }
		public int DispatchTime { get; private set; }
		public int TelemetrySendTime { get; private set; } //Previous cycle

		private Layout _layout;
		private DeviceLookup _lookup;

		public MeadowApp()
		{
			_bluetooth = new Hc06(Device, Device.SerialPortNames.Com4);

			CycleCount = 0;
			DispatchTime = 0;
			TelemetrySendTime = 0;

			_layout = new Layout("control", this);

			//Components
			_sensorReader = new SensorReader(Device, _layout);
			_telemetrySender = new TelemetrySender(new DeviceStream(_bluetooth), TelemetryConfig.Meadow, _layout);

			_lookup = _layout.GetLookup();

			//Telemetry Config
			//_telemetryConfig = new TelemetryConfig();
			//_telemetryConfig.Add<MeadowApp, int>(this, i => i.CycleCount);
			//_telemetryConfig.Add<MeadowApp, int>(this, i => i.DispatchTime);
			//_telemetryConfig.Add<MeadowApp, int>(this, i => i.TelemetrySendTime);
			//_telemetryConfig.Add<SensorReader, float>(_sensorReader, i => i.Temperature);
		}

		public void Load()
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

			_sensorReader.Load();
			_telemetrySender.Load();
		}

		public async void Run()
		{
			Console.WriteLine("ThreadID: " + Thread.CurrentThread.ManagedThreadId);
			
			Stopwatch stopwatch = new Stopwatch();

			while (true)
			{
				//Dispatch cycle
				stopwatch.Start();
				Dispatch();
				stopwatch.Stop();
				DispatchTime = (int)stopwatch.ElapsedMilliseconds;
				stopwatch.Reset();

				//Dispatch telem sender
				stopwatch.Start();
				_telemetrySender.Dispatch(_lookup);
				stopwatch.Stop();
				TelemetrySendTime = (int)stopwatch.ElapsedMilliseconds;
				stopwatch.Reset();

				//Sleep for next cycle
				int sleep = ControlPeriod - DispatchTime;
				if (sleep > 0)
					Thread.Sleep(sleep);
				CycleCount++;
			}
		}

		public void Dispatch(DeviceLookup lookup = null)
		{
			//Console.WriteLine("Dispatch - {0}", DateTime.Now);
			_sensorReader.Dispatch();
		}

		private void _bluetooth_DataReceived(object sender, EventArgs e)
		{
			//DeviceStream stream = new DeviceStream(_bluetooth);
			//Message message = await Transport.Read<Message>(stream);
			byte[] buffer = _bluetooth.ReadAll();
			MessageHeader header = StructSerializer.DeserializeStruct<MessageHeader>(buffer);
			if (header.Type == MessageType.Command)
			{
				CommandMessage command = StructSerializer.DeserializeStruct<CommandMessage>(buffer);
				Console.WriteLine("command {0} = {1},{2}", command.Device.Hash, command.Device.IValue, command.Device.DValue);

				//IComponent component = _components.SingleOrDefault(i => i.Name)
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

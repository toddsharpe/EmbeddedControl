using Ground.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.IO;
using Connect;
using Connect.Messages;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Ground.Pages
{
	class MainPageViewModel : BaseViewModel, ILoadable
	{
		private string _command;
		public string Command
		{
			get
			{
				return _command;
			}
			set
			{
				if (_command == value)
					return;

				_command = value;
				OnPropertyChanged();
			}
		}

		private bool _isLoaded;
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
			set
			{
				if (_isLoaded == value)
					return;

				_isLoaded = value;
				OnPropertyChanged();
			}
		}
		
		public RelayCommand SendCommand { get; }

		private readonly StreamSocket _stream;
		public MainPageViewModel()
		{
			SendCommand = new RelayCommand((state) => Send(state));
			_stream = new StreamSocket();
		}

		public async void Send(object state)
		{
			CommandMessage message = new CommandMessage { Header = new MessageHeader { Type = MessageType.Command }, Device = Command, IValue = 1 };
			byte[] bytes = StructSerializer.Serialize(message);

			DataWriter writer = new DataWriter(_stream.OutputStream);
			writer.WriteBytes(bytes);
			await writer.StoreAsync();
			//Stream s = _stream.OutputStream.AsStreamForWrite();
			//await Transport.WriteAsync(s, new CommandRequest { Device = Command, Value = 1 });
		}

		public async Task LoadAsync()
		{
			// Enumerate devices with the object push service
			DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
			var device = devices.Single();
			var _srv = await RfcommDeviceService.FromIdAsync(device.Id);
			await _stream.ConnectAsync(_srv.ConnectionHostName, _srv.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
			//var s = _stream.OutputStream.AsStreamForWrite();
			//Start listener
			Task.Run(ListenAsync);

			//DevicePicker

			//Paired bluetooth devices
			//DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
			//DeviceInformation device = PairedBluetoothDevices.Single(i => i.Name == "HC-06");

			IsLoaded = true;
		}

		private async Task ListenAsync()
		{
			uint headerSize = (uint)Marshal.SizeOf(typeof(MessageHeader));
			byte[] headerBuffer = new byte[headerSize];

			DataReader reader = new DataReader(_stream.InputStream);
			while (true)
			{
				await reader.LoadAsync(headerSize);
				reader.ReadBytes(headerBuffer);

				MessageHeader header = StructSerializer.DeserializeStruct<MessageHeader>(headerBuffer);

				if (header.Type == MessageType.Telemetry)
				{
					uint telemetrySize = (uint)Marshal.SizeOf(typeof(TelemetryMessage));
					byte[] telemetryBuffer = new byte[telemetrySize - headerSize];
					await reader.LoadAsync(telemetrySize - headerSize);
					reader.ReadBytes(telemetryBuffer);

					var z = new byte[headerBuffer.Length + telemetryBuffer.Length];
					headerBuffer.CopyTo(z, 0);
					telemetryBuffer.CopyTo(z, headerBuffer.Length);

					TelemetryMessage telemetry = StructSerializer.DeserializeStruct<TelemetryMessage>(z);

					Debug.WriteLine("Telemetry");
					for (int i = 0; i < 4; i++)
					{
						Debug.WriteLine("  {0} = {1},{2}", telemetry.Entries[i].Device,
							telemetry.Entries[i].IValue,
							telemetry.Entries[i].DValue);
					}
					Debug.WriteLine("");
				}
			}
		}

		private IBuffer GetBufferFromByteArray(byte[] package)
		{
			using (DataWriter dw = new DataWriter())
			{
				dw.WriteBytes(package);
				return dw.DetachBuffer();
			}
		}
	}
}

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
using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Charts;
using Ground.Models;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Connect.Config;

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

		public CurrentDeviceValues CurrentValues { get; }

		private Dictionary<string, ObservableCollection<Data>> _telemetry;

		public ChartSeriesCollection Series { get; }

		public RelayCommand SendCommand { get; }

		private readonly StreamSocket _stream;
		public MainPageViewModel()
		{
			CurrentValues = new CurrentDeviceValues(TelemetryConfig.Meadow);
			SendCommand = new RelayCommand((state) => Send(state));
			_stream = new StreamSocket();
			_telemetry = new Dictionary<string, ObservableCollection<Data>>();
			Series = new ChartSeriesCollection();
		}

		public async void Send(object state)
		{
			//CommandMessage message = new CommandMessage { Header = new MessageHeader { Type = MessageType.Command }, Device = Command, IValue = 1 };
			//byte[] bytes = StructSerializer.Serialize(message);

			//DataWriter writer = new DataWriter(_stream.OutputStream);
			//writer.WriteBytes(bytes);
			//await writer.StoreAsync();
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

			//AddToChart("Control.CycleCount");
			AddToChart("Sensors.Temperature");

			IsLoaded = true;
		}

		private void AddToChart(string device)
		{
			if (!_telemetry.ContainsKey(device))
				_telemetry.Add(device, new ObservableCollection<Data>());

			LineSeries series = new LineSeries { XBindingPath = "DateTime", YBindingPath = "Value", ItemsSource = _telemetry[device] };
			Series.Add(series);
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
					CurrentValues.Update(telemetry);

					Debug.WriteLine("Telemetry");
					for (int i = 0; i < TelemetryMessage.EntryLength; i++)
					{
						Debug.WriteLine("  {0} = {1},{2}", telemetry.Devices[i].Hash, telemetry.Devices[i].IValue, telemetry.Devices[i].DValue);
					}
					Debug.WriteLine("");

					//for (int i = 0; i < TelemetryMessage.EntryLength; i++)
					//{
					//	if (!_telemetry.ContainsKey(telemetry.Entries[i].Device))
					//		_telemetry.Add(telemetry.Entries[i].Device, new ObservableCollection<Data>());

					//	if (telemetry.Entries[i].IValue != 0)
					//	{
					//		TelemetryEntry capture = telemetry.Entries[i];
					//		ObservableCollection<Data> collection = _telemetry[capture.Device];
					//		Data data = new Data { DateTime = DateTime.Now, Value = capture.IValue };

					//		CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					//		{
					//			collection.Add(data);
					//		});
					//	}
					//	else if (telemetry.Entries[i].DValue != 0)
					//	{
					//		TelemetryEntry capture = telemetry.Entries[i];
					//		ObservableCollection<Data> collection = _telemetry[capture.Device];
					//		Data data = new Data { DateTime = DateTime.Now, Value = capture.DValue };

					//		CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					//		{
					//			collection.Add(data);
					//		});
					//	}
					//}
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

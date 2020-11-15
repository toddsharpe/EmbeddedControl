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

namespace Ground.Pages
{
	class MainPageViewModel : BaseViewModel, ILoadable
	{
		public RelayCommand SendCommand { get; }


		private readonly StreamSocket _stream;
		public MainPageViewModel()
		{
			SendCommand = new RelayCommand((state) => Send(state));
			_stream = new StreamSocket();
		}

		public async void Send(object state)
		{
			var writer = new DataWriter(_stream.OutputStream);
			writer.WriteString("Command");

		}

		public async Task LoadAsync()
		{

			// Enumerate devices with the object push service
			DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
			var device = devices.First();
			var _srv = await RfcommDeviceService.FromIdAsync(device.Id);
			await _stream.ConnectAsync(
				_srv.ConnectionHostName,
				_srv.ConnectionServiceName,
				SocketProtectionLevel
					.BluetoothEncryptionAllowNullAuthentication);
			var writer = new DataWriter(_stream.OutputStream);
			writer.WriteString("Command");
			var store = writer.StoreAsync().AsTask();

			await store;
			/*
			DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
			DeviceInformation device = PairedBluetoothDevices.Single(i => i.Name == "HC-06");
			var d = await BluetoothDevice.FromIdAsync(device.Id);

			var s = await d.GetRfcommServicesAsync();
			var _service = await RfcommDeviceService.FromIdAsync(device.Id);
			*/
			//var device = devices.First();

			//var _service = await RfcommDeviceService.FromIdAsync(
			//										device.Id);

			//var _socket = new StreamSocket();

			//await _socket.ConnectAsync(
			//		_service.ConnectionHostName,
			//		_service.ConnectionServiceName,
			//		SocketProtectionLevel.
			//		BluetoothEncryptionAllowNullAuthentication);

			//var writer = new DataWriter(_socket.OutputStream);

			//writer.WriteString("Command");

			//// Launch an async task to 
			////complete the write operation
			//var store = writer.StoreAsync().AsTask();
			//await store;
			//DevicePicker

			//Paired bluetooth devices
			//DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
			//DeviceInformation device = PairedBluetoothDevices.Single(i => i.Name == "HC-06");

			//var d = await BluetoothDevice.FromIdAsync(device.Id);

			//var services = await d.GetRfcommServicesAsync();
			//foreach (var s in services.Services)
			//{
			//	Console.WriteLine(s.ConnectionHostName + " " + s.ConnectionServiceName + " " + s.ServiceId);
			//}

			//StreamSocket streamSocket = new StreamSocket();
			//await streamSocket.ConnectAsync(d.HostName, "1");

			//IBuffer buffer = GetBufferFromByteArray(Encoding.ASCII.GetBytes("Connected"));
			//await streamSocket.OutputStream.WriteAsync(buffer);
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

using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Drivers
{
	//https://www.olimex.com/Products/Components/RF/BLUETOOTH-SERIAL-HC-06/resources/hc06.pdf
	class Hc06
	{
		private static readonly Dictionary<int, char> BaudLookup = new Dictionary<int, char>
		{
			{ 1200, '1' }, { 2400, '2' }, { 4800, '3' }, { 9600, '4' },
			{ 19200, '5' }, { 38400, '6' }, { 57600, '7' }, { 115200, '8' },
			{ 230400, '9' }, { 460800, 'A' }, { 921600, 'B' }, { 1382400, 'C' },
		};

		public int BaudRate => _port.BaudRate;
		public int BytesToRead => _port.BytesToRead;

		private readonly AutoResetEvent _signal;
		private readonly ISerialPort _port;
		private bool _internalOp;
		public event EventHandler DataReceived;

		public Hc06(IIODevice device, SerialPortName port, int baudRate = 9600)
		{
			_signal = new AutoResetEvent(false);
			_port = device.CreateSerialPort(port, baudRate, 8, Parity.None, StopBits.One);
			_port.DataReceived += (s, e) =>
			{
				if (_internalOp)
				{
					_signal.Set();
					_internalOp = false;
				}
				else
					DataReceived?.Invoke(this, new EventArgs());
			};
			_internalOp = false;
		}

		public async Task Open()
		{
			if (!await DetectBaudRate())
				throw new Exception("Open - DetectBaudRate Failed");
		}

		private Task<bool> DetectBaudRate()
		{
			return Task.Run<bool>(() =>
			{
				//Try with 9600 default value
				_port.Open();
				if (TrySendReceive())
					return true;
				_port.Close();

				//Try all known baud rates in descending order
				foreach (int key in BaudLookup.Keys.Reverse())
				{
					_port.BaudRate = key;
					_port.Open();
					if (TrySendReceive())
						return true;
					_port.Close();
				}
				return false;
			});
		}

		private bool TrySendReceive()
		{
			_internalOp = true;
			_port.Write(Encoding.ASCII.GetBytes("AT"));

			if (!_signal.WaitOne(1000))
			{
				_internalOp = false;
				return false;
			}

			byte[] buffer = new byte[_port.BytesToRead];
			_port.Read(buffer, 0, buffer.Length);

			string response = Encoding.ASCII.GetString(buffer);
			return response == "OK";
		}

		public async Task SetBaud(int baud)
		{
			if (!BaudLookup.ContainsKey(baud))
				throw new ArgumentException("Invalid baud rate", nameof(baud));

			string send = $"AT+BAUD{BaudLookup[baud]}";
			string back = $"OK{baud}";
			if (!await VerifyCommand(send, back))
				throw new Exception("SetBaud - OperationFailed");
		}

		public async Task SetName(string name)
		{
			if (name.Length > 20)
				throw new ArgumentException("Name limited to 20 characters", nameof(name));

			string send = $"AT+NAME{name}";
			string back = $"OK{name}";
			if (!await VerifyCommand(send, back))
				throw new Exception("SetName - OperationFailed");
		}

		public async Task SetPin(string pin)
		{
			int parsed;
			if (pin.Length > 4 || !int.TryParse(pin, out parsed))
				throw new ArgumentException("Invalid pin", nameof(pin));

			string send = $"AT+PIN{pin}";
			string back = "OKsetpin";
			if (!await VerifyCommand(send, back))
				throw new Exception("SetPin - OperationFailed");
		}

		public Task<string> GetVersion()
		{
			return SendCommand("AT+VERSION");
		}

		public void Write(byte[] buffer)
		{
			_port.Write(buffer);
		}

		public int Read(byte[] buffer, int offset, int length)
		{
			return _port.Read(buffer, offset, buffer.Length);
		}

		public byte[] ReadAll()
		{
			byte[] buffer = new byte[_port.BytesToRead];
			_port.Read(buffer, 0, buffer.Length);
			return buffer;
		}

		private Task<string> SendCommand(string send)
		{
			return Task.Run<string>(() =>
			{
				_internalOp = true;
				_port.Write(Encoding.ASCII.GetBytes(send));
				_signal.WaitOne();

				byte[] buffer = new byte[_port.BytesToRead];
				_port.Read(buffer, 0, buffer.Length);

				return Encoding.ASCII.GetString(buffer);
			});
		}

		private async Task<bool> VerifyCommand(string send, string verify)
		{
			string back = await SendCommand(send);
			return back == verify;
		}
	}
}

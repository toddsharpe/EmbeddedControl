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
	class Hc06 : IDevice
	{
		private static readonly Dictionary<int, char> BaudLookup = new Dictionary<int, char>
		{
			{ 1200, '1' }, { 2400, '2' }, { 4800, '3' }, { 9600, '4' },
			{ 19200, '5' }, { 38400, '6' }, { 57600, '7' }, { 115200, '8' },
			{ 230400, '9' }, { 460800, 'A' }, { 921600, 'B' }, { 1382400, 'C' },
		};

		public int BaudRate => _port.BaudRate;
		public int BytesToRead => _port.BytesToRead;

		public bool IsOpen => _port.IsOpen;

		private readonly AutoResetEvent _signal;
		private readonly ISerialPort _port;
		private bool _internalOp;
		public event EventHandler DataReceived;

		public Hc06(IIODevice device, SerialPortName port, int baudRate = 921600)
		{
			_signal = new AutoResetEvent(false);
			_port = device.CreateSerialPort(port, baudRate, 8, Parity.None, StopBits.One);
			_port.DataReceived += (s, e) =>
			{
				Console.WriteLine("DataReceived - {0}!", _port.BytesToRead);
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

		public void Open()
		{
			if (!DetectBaudRate())
				throw new Exception("Open - DetectBaudRate Failed");
		}

		private bool DetectBaudRate()
		{
			//Try with 9600 default value
			_port.Open();
			if (TrySendReceive())
				return true;
			_port.ClearReceiveBuffer();
			_port.Close();

			//Try all known baud rates in descending order
			foreach (int key in BaudLookup.Keys.Reverse())
			{
				_port.BaudRate = key;
				_port.Open();
				if (TrySendReceive())
					return true;
				_port.ClearReceiveBuffer();
				_port.Close();
			}
			return false;
		}

		private bool TrySendReceive()
		{
			//Console.WriteLine("Trying with {0}", _port.BaudRate);
			_internalOp = true;
			_port.Write(Encoding.ASCII.GetBytes("AT"));

			if (!_signal.WaitOne(2000))
			{
				_internalOp = false;
				//Console.WriteLine("timeout");
				return false;
			}

			byte[] buffer = new byte[_port.BytesToRead];
			_port.Read(buffer, 0, buffer.Length);

			string response = Encoding.ASCII.GetString(buffer);
			//Console.WriteLine(response);
			return response == "OK";
		}

		public void SetBaud(int baud)
		{
			if (!BaudLookup.ContainsKey(baud))
				throw new ArgumentException("Invalid baud rate", nameof(baud));

			string send = $"AT+BAUD{BaudLookup[baud]}";
			string back = $"OK{baud}";
			if (!VerifyCommand(send, back))
				throw new Exception("SetBaud - OperationFailed");

			//Update port rate
			_port.ClearReceiveBuffer();
			_port.Close();
			_port.BaudRate = baud;
			_port.Open();
		}

		public void SetName(string name)
		{
			if (name.Length > 20)
				throw new ArgumentException("Name limited to 20 characters", nameof(name));

			string send = $"AT+NAME{name}";
			string back = $"OK{name}";
			if (!VerifyCommand(send, back))
				throw new Exception("SetName - OperationFailed");
		}

		public void SetPin(string pin)
		{
			int parsed;
			if (pin.Length > 4 || !int.TryParse(pin, out parsed))
				throw new ArgumentException("Invalid pin", nameof(pin));

			string send = $"AT+PIN{pin}";
			string back = "OKsetpin";
			if (!VerifyCommand(send, back))
				throw new Exception("SetPin - OperationFailed");
		}

		public string GetVersion()
		{
			return SendCommand("AT+VERSION");
		}

		public void Write(string s)
		{
			Write(Encoding.ASCII.GetBytes(s));
		}
		
		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			_port.Write(buffer, offset, count);
		}

		public int Read(byte[] buffer, int offset, int length)
		{
			//Console.WriteLine("BytesToRead: {0}", _port.BytesToRead);
			return _port.Read(buffer, offset, Math.Min(_port.BytesToRead, buffer.Length));
		}

		public byte[] ReadAll()
		{
			byte[] buffer = new byte[_port.BytesToRead];
			_port.Read(buffer, 0, buffer.Length);
			return buffer;
		}

		private string SendCommand(string send)
		{
			_internalOp = true;
			_port.Write(Encoding.ASCII.GetBytes(send));
			_signal.WaitOne();

			byte[] buffer = new byte[_port.BytesToRead];
			_port.Read(buffer, 0, buffer.Length);

			return Encoding.ASCII.GetString(buffer);
		}

		private bool VerifyCommand(string send, string verify)
		{
			string back = SendCommand(send);
			//Console.WriteLine("VerifyCommand {0},{1} -> {2}", send, verify, back);
			return back == verify;
		}
	}
}

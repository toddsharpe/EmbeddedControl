using Connect;
using Connect.Config;
using Connect.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ground.Models
{
	class CurrentDeviceValues : ObservableCollection<DeviceValue>
	{
		public CurrentDeviceValues(TelemetryConfig config)
		{
			//Add devices
			foreach (var item in config)
			{
				this.Add(new DeviceValue { Hash = item.Name.GetHash64(), Device = item.Name, TypeCode = item.TypeCode });
			}
		}

		public void Update(TelemetryMessage message)
		{
			for (int i = 0; i < TelemetryMessage.EntryLength; i++)
			{
				Int64 hash = message.Devices[i].Hash;
				if (hash == 0)
					continue;

				DeviceValue found = this.SingleOrDefault(item => item.Hash == hash);
				if (found == null)
					continue;

				if (found.TypeCode == TypeCode.Int32)
					found.Value = message.Devices[i].IValue;
				else if (found.TypeCode == TypeCode.Single)
					found.Value = message.Devices[i].DValue;
				else
					continue;
			}
		}
	}
}

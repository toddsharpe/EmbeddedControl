using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Framework
{
	class DeviceLookup : Dictionary<Int64, Device>
	{
		public DeviceLookup()
		{
			
		}

		public void Add(string path, IComponent o, PropertyInfo property)
		{
			using (SHA256 hash = new SHA256CryptoServiceProvider())
			{
				byte[] bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(path));
				Int64 hashCodeStart = BitConverter.ToInt64(bytes, 0);
				Int64 hashCodeMedium = BitConverter.ToInt64(bytes, 8);
				Int64 hashCodeEnd = BitConverter.ToInt64(bytes, 24);
				Int64 hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

				this.Add(hashCode, new Device { Component = o, Property = property });
				Console.Write("Added {0}={1}" + Environment.NewLine, path, hashCode);
			}
		}

		//public T Get<T>(DeviceToken<T> token)
		//{
		//	return (T)this[token.Hash].Property.GetValue(this[token.Hash].Component);
		//}

		//public void Set<T>(DeviceToken<T> token, T value)
		//{
		//	_state[token.Hash].Property.SetValue(_state[token.Hash])

		//}
	}
}

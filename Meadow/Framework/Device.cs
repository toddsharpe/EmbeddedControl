using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Framework
{
	class Device
	{
		public IComponent Component { get; set; }
		public PropertyInfo Property { get; set; }

		public object Get()
		{
			return Property.GetValue(Component);
		}
	}
}

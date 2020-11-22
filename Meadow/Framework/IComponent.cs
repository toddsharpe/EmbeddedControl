using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Framework
{
	interface IComponent
	{
		string Name { get; }
		void Load();
		void Dispatch();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Framework
{
    class DeviceToken<T> : IDeviceToken
    {
        public Int64 Hash { get; }
        
        public DeviceToken()
        {

        }
    }
}

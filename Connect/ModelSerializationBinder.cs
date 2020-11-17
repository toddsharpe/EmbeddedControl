using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Connect
{
	//Shared projects are built in the context of the main project, therefore the assembly name is different and has to be ignored.
	class ModelSerializationBinder : ISerializationBinder
	{
		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = serializedType.FullName;
		}

		public Type BindToType(string assemblyName, string typeName)
		{
			return Type.GetType(typeName);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Framework
{
	class Layout
	{
		public Layout Parent { get; set; }
		public string Name { get; }
		public IComponent Component { get; }
		public List<Layout> Nodes { get; }

		public Layout(string name, IComponent component = null)
		{
			Parent = null;
			Name = name;
			Component = component;
			Nodes = new List<Layout>();
		}

		public Layout Get(string s)
		{
			Layout search = Nodes.SingleOrDefault(i => i.Name == s);
			if (search != null)
				return search;

			Layout layout = new Layout(s) { Parent = this };
			Nodes.Add(layout);
			return layout;
		}

		public void Bind(string s, IComponent component)
		{
			Layout search = Nodes.SingleOrDefault(i => i.Name == s);
			if (search != null)
				throw new Exception("Can't bind to existing layout node");

			Nodes.Add(new Layout(s, component) { Parent = this });
		}

		public DeviceLookup GetLookup()
		{
			DeviceLookup state = new DeviceLookup();

			Stack<Layout> stack = new Stack<Layout>();
			stack.Push(this);

			while (stack.Count != 0)
			{
				Layout current = stack.Pop();

				if (current.Component != null)
				{
					Type t = current.Component.GetType();
					foreach (PropertyInfo info in t.GetProperties())
					{
						state.Add(BuildDeviceName(current, info.Name), current.Component, info);
					}
				}

				foreach (Layout node in current.Nodes)
				{
					stack.Push(node);
				}
			}

			return state;
		}

		private static string BuildDeviceName(Layout current, string name)
		{
			Stack<Layout> layouts = new Stack<Layout>();
			Layout temp = current;
			while (temp != null)
			{
				layouts.Push(temp);
				temp = temp.Parent;
			}

			return layouts.Select(i => i.Name).Aggregate((i, j) => i + "." + j) + "." + name;
		}
	}
}

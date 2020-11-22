using Meadow.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
	class TelemetryConfig
	{
		public class TelemetryConfigEntry
		{
			public string Name;
			public IComponent Component;
			public PropertyInfo GetProperty;
		}

		private List<TelemetryConfigEntry> _entries;

		public int Count => _entries.Count;

		public TelemetryConfig()
		{
			_entries = new List<TelemetryConfigEntry>();
		}

		public void Add<T, P>(IComponent component, Expression<Func<T, P>> selector) where T : IComponent
		{
			MemberExpression memberExpression = selector.Body as MemberExpression;
			if (memberExpression == null)
				throw new Exception("Usage");

			PropertyInfo getProperty = component.GetType().GetProperty(memberExpression.Member.Name);
			if (getProperty.PropertyType != typeof(int) && getProperty.PropertyType != typeof(float))
				throw new Exception("Property type");


			TelemetryConfigEntry entry = new TelemetryConfigEntry
			{
				Name = component.Name + "." + memberExpression.Member.Name,
				Component = component,
				GetProperty = getProperty,
			};
			Console.WriteLine("Added telem: " + entry.Name + " as " + entry.GetProperty.PropertyType.FullName);

			_entries.Add(entry);
		}

		public TelemetryConfigEntry this[int key]
		{
			get => _entries[key];
		}
	}
}

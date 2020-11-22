using Meadow;
using Meadow.Framework;
using System;
using System.Threading;

namespace Meadow
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length > 0 && args[0] == "--exitOnDebug") return;

			Console.WriteLine("\nMeadow Control");
			IControlApp app = new MeadowApp();
			app.Load();
			app.Run();

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

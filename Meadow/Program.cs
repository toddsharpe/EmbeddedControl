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
			MeadowApp app = new MeadowApp();
			app.Run();

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

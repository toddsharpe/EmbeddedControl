using Meadow;
using Meadow.Framework;
using System;
using System.Threading;

namespace Meadow
{
	class Program
	{
		static IApp app;
		public static void Main(string[] args)
		{
			Console.WriteLine("\nMeadow Controll");
			app = new MeadowApp();

			if (app is IAsyncApp asyncApp)
				asyncApp.Run().Wait();

			Thread.Sleep(Timeout.Infinite);
		}
	}
}

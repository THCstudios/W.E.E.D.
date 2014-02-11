using System;
using System.IO;
using SharedMemory;

namespace Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SharedMemoryControl connection = new SharedMemoryControl();
			ConnectionInformation info = new ConnectionInformation();
			info.address = "10.91.52.125";
			info.port = 12345;
			info.noOfClients = 2;
			connection.Info = info;
			connection.State = SharedMemoryControl.ServerClientState.SERVER;
			connection.AddSource(File.ReadAllText("../../../initial_game.json"));
			Console.WriteLine("Starting SharedMemory(tm) Session");
			connection.Start();
			Console.WriteLine("Client(s) Connected!");
			Console.WriteLine("Buffer:");
			Console.WriteLine(connection.Manager.CacheInfo());
			Console.WriteLine(connection.Manager.Dump(FeMoUpdateStringFormatter.CONSOLE));
		}
	}
}

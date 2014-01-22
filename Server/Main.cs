using System;
using SharedMemory;

namespace Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			FeMoManager man = new FeMoManager();
			man.ReadFromFile("../../../Test/bin/Debug/err_shutdown.json", FeMoUpdateStringFormatter.JSON);
			ConnectionHandle con = ConnectionHandle.Connect (ConnectionType.WAIT, "127.0.0.1");
			FeMoPeer peer = new FeMoPeer(con);
			man.AddConnection(peer);
			man.SendUpdateString();
			con.Close();
		}
	}
}

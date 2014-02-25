using System;
using SharedMemory;

namespace Client
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			FeMoManager man = new FeMoManager ();
//			ConnectionHandle con = ConnectionHandle.Connect (ConnectionType.CONNECT, "10.91.2.123");
//			man.AddConnection (new FeMoPeer (con));
//			while (true) {
//				Console.WriteLine(man.CacheInfo());
//				Console.WriteLine(man.Dump(FeMoUpdateStringFormatter.CONSOLE));
//				if(man.ObjectCount() > 0)
//					break;
//				System.Threading.Thread.Sleep(1000);
//			}
//			Console.WriteLine("[TEST] Object: 0, Field: TestInt, Expected Value: 26");
//			FeMoObject fmo = man.Get(0);
//			if(fmo.GetInt("TestInt") == 26)
//				Console.WriteLine("[ OK ] Test Completed!");
//			else
//				Console.WriteLine("[FAIL] Expected: 26, Found: " + fmo.GetInt("TestInt"));
//			Console.WriteLine("[TEST] Object: 2 -> Object: 0, Field: TestString, Expected Value: Test");
//			fmo = man.Get(2);
//			if(fmo.GetObject("TestObject").GetString("TestString") == "Test")
//				Console.WriteLine("[ OK ] Test Completed!");
//			else
//				Console.WriteLine("[FAIL] Expected: Test, Found: " + fmo.GetInt("TestString"));
//			Environment.Exit(0);
			Global.AddJob (new InitTask ());
			SharedMemoryControl connection = new SharedMemoryControl ();
			ConnectionInformation info = new ConnectionInformation ();
			info.address = "10.91.4.57";
			info.port = 12345;
			info.noOfClients = 0;
			connection.Info = info;
			connection.State = SharedMemoryControl.ServerClientState.CLIENT;
			Console.WriteLine ("Connecting to Server");
			connection.Start ();
			FeMoManager man = connection.Manager;
//			Console.WriteLine("[TEST] Object: 0, Field: TestInt, Expected Value: 26");
//			FeMoObject fmo = man.Get(0);
//			if(fmo.GetInt("TestInt") == 26)
//				Console.WriteLine("[ OK ] Test Completed!");
//			else
//				Console.WriteLine("[FAIL] Expected: 26, Found: " + fmo.GetInt("TestInt"));
//			Console.WriteLine("[TEST] Object: 2 -> Object: 0, Field: TestString, Expected Value: Test");
//			fmo = man.Get(2);
//			if(fmo.GetObject("TestObject").GetString("TestString") == "Test")
//				Console.WriteLine("[ OK ] Test Completed!");
//			else
//				Console.WriteLine("[FAIL] Expected: Test, Found: " + fmo.GetInt("TestString"));
//			Console.WriteLine (man.Dump (FeMoUpdateStringFormatter.CONSOLE));
//			try {
//				Global.DebugObject(man.Get(5));
//				FeMoList list = new FeMoList (connection.Manager.Get (5));
//				FeMoObject obj = list.GetIndex (1);
//				Global.DebugObject (obj);
//				FeMoObject tile_4 = new FeMoObject();
//				tile_4.Name = "Tile_4";
//				tile_4.Id = 104;
//				tile_4.AddInt("Terrain", 0);
//				tile_4.AddInt("X", 2);
//				tile_4.AddInt("Y", 0);
//				tile_4.AddInt("Levitation", 0);
//				man.CacheObject(tile_4);
//				list.AddObject(tile_4);
//				connection.Manager.SendUpdateString();
//				Global.DebugObject(man.Get(5));
//				Console.WriteLine(man.CacheInfo());
//				Console.WriteLine(man.Dump(Global.GetDefaultOutputFormatter()));
//				list.RemoveObject(4);
//				connection.Manager.SendUpdateString();
//				Console.WriteLine(man.CacheInfo());
//				Console.WriteLine(man.Dump(Global.GetDefaultOutputFormatter()));
//				Global.DebugObject(man.Get(5));
//			} catch (Exception e) {
//				Global.fail(e.ToString());
//				Global.DebugObject(connection.Manager.Get(5));
//				Console.WriteLine(man.CacheInfo());
//				Console.WriteLine(man.Dump(Global.GetDefaultOutputFormatter()));
//			}
			/*connection.Close();
			Environment.Exit(0);*/
		}

		private class InitTask : Target {

			public InitTask() {
				Id = 0;
				Name = "Init Task";
			}

			public override TargetState run ()
			{
				Console.WriteLine("Init");
				return TargetState.DONE;
			}
		}
	}
}

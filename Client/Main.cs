using System;
using SharedMemory;

namespace Client
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			FeMoManager man = new FeMoManager ();
			ConnectionHandle con = ConnectionHandle.Connect (ConnectionType.CONNECT, "10.91.2.123");
			man.AddConnection (new FeMoPeer (con));
			while (true) {
				Console.WriteLine(man.CacheInfo());
				Console.WriteLine(man.Dump(FeMoUpdateStringFormatter.CONSOLE));
				if(man.ObjectCount() > 0)
					break;
				System.Threading.Thread.Sleep(1000);
			}
			Console.WriteLine("[TEST] Object: 0, Field: TestInt, Expected Value: 26");
			FeMoObject fmo = man.Get(0);
			if(fmo.GetInt("TestInt") == 26)
				Console.WriteLine("[ OK ] Test Completed!");
			else
				Console.WriteLine("[FAIL] Expected: 26, Found: " + fmo.GetInt("TestInt"));
			Console.WriteLine("[TEST] Object: 2 -> Object: 0, Field: TestString, Expected Value: Test");
			fmo = man.Get(2);
			if(fmo.GetObject("TestObject").GetString("TestString") == "Test")
				Console.WriteLine("[ OK ] Test Completed!");
			else
				Console.WriteLine("[FAIL] Expected: Test, Found: " + fmo.GetInt("TestString"));
			Environment.Exit(0);
		}
	}
}

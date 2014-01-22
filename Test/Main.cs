using System;
using SharedMemory;
namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			FeMoManager man = new FeMoManager();
			FeMoObject fmo = new FeMoObject();
			fmo.Name = "TestObject";
			fmo.Id = 0;
			fmo.AddString("TestString", "Test");
			fmo.AddInt("TestInt", 26);
			man.CacheObject(fmo);
			FeMoObject fmo_ = new FeMoObject();
			fmo_.Name = "TestObject";
			fmo_.Id = 1;
			fmo_.AddString("String", "Yupidooh");
			fmo_.AddInt("Int", 13);
			man.CacheObject(fmo_);
			fmo_ = new FeMoObject();
			fmo_.Name = "TestLinkObject";
			fmo_.Id = 2;
			fmo_.AddObject("TestObject", fmo);
			man.CacheObject(fmo_);
			Console.WriteLine(man.CacheInfo());
			Console.WriteLine(man.Dump(FeMoUpdateStringFormatter.CONSOLE));
			man.FileDump("err_shutdown.json", FeMoUpdateStringFormatter.JSON);
		}
	}
}

using System;
using System.IO;
using SharedMemory;

namespace StartupTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TaskManager tm = new TaskManager();
			InitTarget init = new InitTarget(tm);
			tm.pushTarget(0, init);
			tm.RunAll();
			SharedMemoryControl connection = new SharedMemoryControl();
			ConnectionInformation info = new ConnectionInformation();
			info.address = "10.23.11.158";
			info.port = 12345;
			info.noOfClients = 2;
			connection.Info = info;
			connection.State = SharedMemoryControl.ServerClientState.SERVER;
			connection.AddSource(File.ReadAllText("initial_game.json"));
			Console.WriteLine("Starting SharedMemory(tm) Session");
			connection.Start();
			Console.WriteLine("Client(s) Connected!");
			Console.WriteLine("Buffer:");
			Console.WriteLine(connection.Manager.CacheInfo());
			Console.WriteLine(connection.Manager.Dump(FeMoUpdateStringFormatter.CONSOLE));
		}
	}

	public class InitTarget : Target {
		
		private TaskManager tm;
		
		public InitTarget (TaskManager tm)
		{
			Id = 0;
			Name = "Init Task";
			this.tm = tm;
		}
		
		public override TargetState run() {
			TestTarget test = new TestTarget();
			OtherTarget other = new OtherTarget();
			DownloadJob dl = new DownloadJob(301, "http://www.google.com/index.html", "index.html");
			DownloadJob dj = new DownloadJob(302, "http://localhost/initial_game.json", "initial_game.json");
			test.predesecors.Add(this);
			dl.predesecors.Add(this);
			dj.predesecors.Add(this);
			other.predesecors.Add(dl);
			if(!File.Exists("initial_game.json"))
				other.predesecors.Add(dj);
			tm.pushTarget(100, test);
			tm.pushTarget(200, other);
			tm.pushTarget(301, dl);
			if(!File.Exists("initial_game.json"))
				tm.pushTarget(302, dj);
			return TargetState.DONE;
		}
	}
	
	public class TestTarget : Target {
		public TestTarget() {
			Id = 100;
			Name = "Some Task";
		}
		
		public override TargetState run() {
			System.Threading.Thread.Sleep(300);
			return TargetState.DONE;
		}
	}
	
	public class OtherTarget : Target {
		public OtherTarget() {
			Id = 200;
			Name = "Some Other Task";
		}
		
		public override TargetState run ()
		{
			System.Threading.Thread.Sleep(500);
			return TargetState.DONE;
		}
	}
}

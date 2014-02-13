using System;
using System.IO;
using SharedMemory;

namespace StartupTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			InitTarget init = new InitTarget(Global.ConnectionStartup);
			Global.AddJob(init);
			Global.RunJobs();
			SharedMemoryControl connection = new SharedMemoryControl();
			ConnectionInformation info = new ConnectionInformation();
			info.address = "0.0.0.0";
			info.port = 12345;
			info.noOfClients = 1;
			connection.Info = info;
			connection.State = SharedMemoryControl.ServerClientState.SERVER;
			connection.AddSource(File.ReadAllText("initial_game.json"));
			Console.WriteLine("Starting SharedMemory(tm) Session");
			connection.Start();
			Console.WriteLine("Client(s) Connected!");
			Console.WriteLine("Buffer:");
			Console.WriteLine(connection.Manager.CacheInfo());
			Console.WriteLine(connection.Manager.Dump(FeMoUpdateStringFormatter.CONSOLE));
			System.Threading.Thread.Sleep(1000);
			connection.Manager.BroadcastCommand("test");
			connection.Manager.BroadcastCommand("boing");
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
			//TestTarget test = new TestTarget();
			//OtherTarget other = new OtherTarget();
			DownloadJob dj = new DownloadJob(302, "http://localhost/initial_game.json", "initial_game.json");
			//test.predesecors.Add(this);
			dj.predesecors.Add(this);
			//other.predesecors.Add(dj);
			//tm.pushTarget(100, test);
			//tm.pushTarget(200, other);
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

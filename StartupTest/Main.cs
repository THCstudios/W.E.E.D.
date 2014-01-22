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
			DownloadJob dj = new DownloadJob(302, "http://dl.bukkit.org/latest-beta/craftbukkit-beta.jar", "craftbukkit-beta.jar");
			test.predesecors.Add(this);
			dl.predesecors.Add(this);
			dj.predesecors.Add(this);
			other.predesecors.Add(dl);
			if(!File.Exists("craftbukkit-beta.jar"))
				other.predesecors.Add(dj);
			tm.pushTarget(100, test);
			tm.pushTarget(200, other);
			tm.pushTarget(301, dl);
			if(!File.Exists("craftbukkit-beta.jar"))
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

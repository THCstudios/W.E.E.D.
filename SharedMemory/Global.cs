
using System;

namespace SharedMemory
{
	public abstract class Global
	{
		private static TaskManager connectionStartup = new TaskManager();

		public static TaskManager ConnectionStartup {
			get {
				return connectionStartup;
			}
			set {
				connectionStartup = value;
			}
		}

		private static FeMoUpdateStringFormatter databaseFormatter = FeMoUpdateStringFormatter.SQLITE;

		public static FeMoUpdateStringFormatter GetDefaultFormatter() {
			return FeMoUpdateStringFormatter.JSON;
		}

		public static FeMoUpdateStringFormatter GetDefaultOutputFormatter() {
			return FeMoUpdateStringFormatter.CONSOLE;
		}

		public static void DebugObject(FeMoObject obj) {
			Console.WriteLine("[DEBUG] OBJECT");
			Console.WriteLine("Debug Stack for " + obj.Name + " (" + obj.Id + ")");
			Console.WriteLine(obj.Manager.CacheInfo());
			Console.WriteLine(obj);
		}

		public static void AddJob (Target job)
		{
			connectionStartup.pushTarget(job.Id, job);
		}

		public static void RunJobs ()
		{
			connectionStartup.RunAll();
		}

		public static FeMoUpdateStringFormatter GetDefaultDatabaseFormatter() {
			return databaseFormatter;
		}

		public static void SwitchMySQL () {
			databaseFormatter = FeMoUpdateStringFormatter.MYSQL;
		}

		public static void SwitchSQLITE () {
			databaseFormatter = FeMoUpdateStringFormatter.SQLITE;
		}

		public static void log(String msg) {
			Console.WriteLine("[INFO] " + msg);
		}

		public static void femo(String msg) {
			Console.WriteLine("[FEMO] " + msg);
		}

		public static void success(String msg) {
			Console.WriteLine("[ OK ] " + msg);
		}

		public static void fail(String msg) {
			Console.WriteLine("[FAIL] " + msg);
		}

		public static void warn (String msg)
		{
			Console.WriteLine ("[WARN] " + msg);
		}

		public static void AddCommandsJob(CommandHandler handler) {
			CommandJob job = new CommandJob(handler);
			job.Id = 101;
			job.Name = "Add Commands";
			AddJob(job);
		}

		private class CommandJob : Target {

			private CommandHandler handler;

			public CommandJob(CommandHandler handler) {
				this.handler = handler;
			}

			public override TargetState run ()
			{
				handler.PutMethod("test", delegate(string[] args) {
					Global.log("Test");
					return 0;
				});
				handler.PutMethod("boing", delegate(string[] args) {
					Global.warn("Boing is too warm!!!");
					Global.warn("Shutting down!!!");
					return 0;
				});
				return TargetState.DONE;
			}			
		}
	}
}

using System;

namespace SharedMemory
{
	public abstract class Global
	{

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
	}
}
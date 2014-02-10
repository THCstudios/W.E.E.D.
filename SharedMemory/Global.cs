
using System;

namespace SharedMemory
{
	public abstract class Global
	{
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
	}
}
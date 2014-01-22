
using System;

namespace SharedMemory
{
	public abstract class Global
	{
		public static FeMoUpdateStringFormatter GetDefaultFormatter() {
			return FeMoUpdateStringFormatter.JSON;
		}
	}
}


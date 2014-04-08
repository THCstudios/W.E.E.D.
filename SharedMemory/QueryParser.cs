using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedMemory {
	class QueryParser {
		public static FeMoObject Parse(String cmd) {
			String[] split = cmd.Split(' ');
			FeMoObject f = new FeMoObject();
			if (split[0] == "GET") {
				int id = Convert.ToInt32(split[1]);
			} else if (split[0] == "SET") {

			} else if (split[0] == "DEL") {
				int id = Convert.ToInt32(split[1]);

			} else if (split[0] == "INS") {

			} else if (split[0] == "EXP") {

			} else if (split[0] == "INP") {

			} else if (split[0] == "DEF") {

			} else if (split[0] == "ALT") {

			}
		}
	}
}

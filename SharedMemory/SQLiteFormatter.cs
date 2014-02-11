using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedMemory {
	class SQLiteFormatter :SQLExporter{
		public SQLiteFormatter() {
			base.Deci = "decimal";
			base.Stri = "string";
		}
	}
}

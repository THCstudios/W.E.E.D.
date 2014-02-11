using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedMemory {
	class MySQLFormatter :SQLExporter{
		public MySQLFormatter() {
			base.Deci = "double";
			base.Stri = "varchar(200)";
		}
	}
}

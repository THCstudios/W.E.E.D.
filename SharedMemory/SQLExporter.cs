using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedMemory {
	abstract class SQLExporter : FeMoUpdateStringFormatter{
		String stri, deci;
		long tempid;

		protected String Stri {
			get { return stri; }
			set { stri = value; }
		}

		protected String Deci {
			get { return deci; }
			set { deci = value; }
		}

		public override string FormatObject(FeMoObject obj) {
			tempid = obj.Id;
			return "insert into o_objects values ('" + obj.Id + "','" + obj.Name + "');\n";
		}

		public override string FormatEntry(FeMoEntry entry) {
			return "insert into e_entries values ('" + entry.Name + "','" + entry.Value + "','"+tempid+"');\n";
		}

		public override string OpenEntryList() {
			return "create table if not exists e_entries(e_name " + stri + " primary key,e_value " + stri + ",e_o_id " + deci + ",foreign key(e_o_id) references o_objects(o_id) on update cascade on delete cascade);\n";
		}

		public override string CloseEntryList() {
			return "";
		}

		public override string EntrySeparator() {
			return "";
		}

		public override string OpenObjectList() {
			return "drop table if exists o_objects;\ncreate table o_objects(o_id "+deci+" primary key,o_name "+stri+");\n";
		}
		public override string CloseObjectList() {
			return "";
		}

		public override string ObjectSeparator() {
			return "";
		}
	}
}

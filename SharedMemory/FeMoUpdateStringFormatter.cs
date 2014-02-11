using System;
using System.Data.Linq;
using System.Linq;

namespace SharedMemory
{
	public abstract class FeMoUpdateStringFormatter
	{
		public static FeMoUpdateStringFormatter CONSOLE = new ConsoleFormatter();
		public static FeMoUpdateStringFormatter JSON = new JSONFormatter();
		public static FeMoUpdateStringFormatter XML = new XMLFormatter();
		public static FeMoUpdateStringFormatter MYSQL = new MySQLFormatter();
		public static FeMoUpdateStringFormatter SQLITE = new SQLiteFormatter();


		public abstract String FormatObject(FeMoObject obj);
		public abstract String FormatEntry(FeMoEntry entry);
		public abstract String OpenEntryList();
		public abstract String CloseEntryList();
		public abstract String EntrySeparator();

		public abstract String OpenObjectList();
		public abstract String CloseObjectList();
		public abstract String ObjectSeparator();

		public virtual FeMoObject[] Parse(String objString) {
			throw new NotImplementedException("String Formatter " + this.GetType().FullName + " is not capable of reading Elements");
		}

		public String Format(FeMoObject obj) {
			String ret = FormatObject(obj);
			ret += OpenEntryList();
			var erg = from e in obj.Update where e.Changed || !obj.Current.Contains(e) select e;
			for(int i = 0; i < erg.ToList().Count(); i++) {
				if(i != 0)
					ret += EntrySeparator();
				ret += erg.ToList()[i].UpdateString(this);
			}
			ret += CloseEntryList();
			return ret;
		}

		public String Dump(FeMoObject obj) {
			String ret = FormatObject(obj);
			ret += OpenEntryList();
			var erg = from e in obj.Update select e;
			for(int i = 0; i < erg.ToList().Count(); i++) {
				if(i != 0)
					ret += EntrySeparator();
				ret += this.FormatEntry((erg.ToList()[i]));
			}
			ret += CloseEntryList();
			return ret;
		}
	}
}


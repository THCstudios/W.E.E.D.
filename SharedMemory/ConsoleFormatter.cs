using System;

namespace SharedMemory
{
	public class ConsoleFormatter : FeMoUpdateStringFormatter
	{
		public override string FormatObject (FeMoObject obj)
		{
			return String.Format("{0,-10} {1,-30} {2,5} Entries:", obj.Id, obj.Name, obj.Update.Count);
		}

		public override string CloseEntryList ()
		{
			return "";
		}

		public override string EntrySeparator ()
		{
			return "\n";
		}

		public override string OpenEntryList ()
		{
			return "\n";
		}

		public override string FormatEntry (FeMoEntry entry)
		{
			return String.Format("\t{0, 10}: {1,-20} => {2,-30}", entry.Type, entry.Name, entry.Value);
		}

		public override string CloseObjectList ()
		{
			return "\nEND";
		}

		public override string ObjectSeparator ()
		{
			return "\n";
		}

		public override string OpenObjectList ()
		{
			return "BEGIN\n";
		}
	}
}


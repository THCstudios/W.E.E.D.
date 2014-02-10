using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharedMemory {
	class XMLFormatter : FeMoUpdateStringFormatter{
		public override string FormatEntry(FeMoEntry entry) {
			return "\t\t\t<Entry name=\""+entry.Name + "\" value=\"" + entry.Value + "\" type=\"" + entry.Type +"\" ></Entry>";
		}

		public override string CloseEntryList() {
			return "\n\t\t</Entries>\n\t</Object>";
		}

		public override string OpenEntryList() {
			return "\t\t<Entries>\n";
		}

		public override string EntrySeparator() {
			return "\n";
		}

		public override string FormatObject(FeMoObject obj) {
			return "\t<Object id=\""+ obj.Id + "\" name=\"" + obj.Name + "\" >\n";
		}

		public override string ObjectSeparator() {
			return "\n";
		}

		public override string OpenObjectList() {
			return "<Objects>\n";
		}

		public override string CloseObjectList() {
			return "\n</Objects>";
		}

		public override FeMoObject[] Parse(String objString) {
			XElement xe = XElement.Parse(objString);
			List<FeMoObject> fmos = new List<FeMoObject>();
			var res = from a in xe.Descendants("Object")
					  select new {
						  Object = a,
						  Entries = (from b in a.Descendants("Entry")
									 select b)
					  };

			foreach (var r in res) {
				FeMoObject fme = new FeMoObject();
				fme.Id = long.Parse(r.Object.Attribute("id").Value);
				fme.Name = r.Object.Attribute("name").Value;
				foreach (var i in r.Entries) {
					FeMoEntry e = new FeMoEntry();
					e.Name = i.Attribute("name").Value;
					e.Value = i.Attribute("value").Value;
					fme.AddEntry(e);
				}
				fmos.Add(fme);
			}
			return fmos.ToArray();
		}
	}
}

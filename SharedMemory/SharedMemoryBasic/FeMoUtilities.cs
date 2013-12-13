using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharedMemory {
	public class FeMoUtilities {
		public static List<FeMoObject> ReadObject(String femo) {
			String[] lines = Regex.Split(femo, "\n");
			//String[] lines = femo.Split("\n".ToCharArray());
			List<FeMoObject> list = new List<FeMoObject>();
			foreach (String str in lines) {
				int i = str.IndexOf("body:=[");
				String head = str.Substring(0, i - 3);
				String body = str.Substring(i + 7);

				//Head Splitting
				i = head.IndexOf("head:=");
				head = head.Substring(i);
				i = head.IndexOf("name:=");
				head = head.Substring(i);


				FeMoObject feMoOb = new FeMoObject();

				//String[] headS = head.Split("||".ToCharArray());
				String[] headS = Regex.Split(head, "||");
				foreach (String s in headS) {
						if(s.StartsWith("name"))
							feMoOb.Name = s.Substring(s.IndexOf(":=") + 2);
						else if( s.StartsWith("vers"))
							feMoOb.Version = float.Parse(s.Substring(s.IndexOf(":=") + 2));
						else if( s.StartsWith("id"))
							feMoOb.ID = long.Parse(s.Substring(s.IndexOf(":=") + 2));

				}

				//Body splitting
				body = head.Substring(i);

				String[] bodyS = Regex.Split(body, "||");
				foreach (String s in bodyS) {
					String[] forsplit = Regex.Split(s, ":=");
					feMoOb.Add(forsplit[0], forsplit[1]);
				}
				list.Add(feMoOb);
			}
			return list;
		}

		public static XElement XMLDump(List<FeMoObject> femo) {
			XElement xe = new XElement("FeMoObjects",
				              from a in femo
				          select
				              new XElement("FeMo",
					              new XAttribute("Name", a.Name),
					              new XAttribute("Version", a.Version),
					              new XAttribute("ID", a.ID),
					              new XElement("Body",
						              from b in a
						              select new XElement("Object",
							                  new XElement("Key", b.Key),
							                  new XElement("Value", b.Value)))));
			return xe;
		}
	}
}

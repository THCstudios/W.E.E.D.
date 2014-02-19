using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace SharedMemory
{
	public class JSONFormatter : FeMoUpdateStringFormatter
	{
		public override string FormatEntry (FeMoEntry entry)
		{
			return "{\"Name\":\"" + entry.Name + "\", \"Value\":\"" + entry.Value + "\", \"Type\":\"" + entry.Type + "\"}";
		}

		public override string CloseEntryList ()
		{
			return "]}";
		}

		public override string OpenEntryList ()
		{
			return "\"update\":[";
		}

		public override string EntrySeparator ()
		{
			return ", ";
		}

		public override string FormatObject (FeMoObject obj)
		{
			return "{\"Id\":\"" + obj.Id + "\", \"Name\":\"" + obj.Name + "\",";
		}

		public override string ObjectSeparator ()
		{
			return EntrySeparator();
		}

		public override string OpenObjectList ()
		{
			return "{\"updates\":[";
		}

		public override string CloseObjectList ()
		{
			return "]}";
		}

		public override FeMoObject[] Parse (String objString)
		{
			String json = objString.Trim ().Replace (" ", "").Replace ("\t", "").Replace ("\n", "").Replace("\r", "");
			if (json.StartsWith ("{") && json.EndsWith ("}")) {
				List<FeMoObject> fmos = new List<FeMoObject>();
				json = json.Substring(1, json.Length - 2);
				String[] objs = MakeObjectList(json);
				foreach (String obj in objs) {
					FeMoObject fmo = ReadObject(obj);
					fmos.Add(fmo);
				}
				return fmos.ToArray();
			} else {
				throw new Exception("Not valid JSON");
			}
		}

		private String[] MakeObjectList(String json) {
			String work = json.Substring(json.IndexOf("["));
			work = work.Substring(0, work.LastIndexOf("]"));
			List<String> objs = new List<string>();
			int depth = 0;
			String ele = "";
			for(int i = 0; i < work.Length; i++) {
				if(work[i] == '{') {
					depth++;
					if(depth == 1) {
						ele = "";
					} else {
						ele += work[i];
					}
				} else if (work[i] == '}') {
					depth--;
					if(depth <= 0) {
						depth = 0;
						objs.Add(ele);
					}
					else {
						ele += work[i];
					}
				} else {
					ele += work[i];
				}
			}
			return objs.ToArray();
		}

		private FeMoObject ReadObject (String obj)
		{
			FeMoObject fmo = new FeMoObject();
			String head = obj.Split ('[') [0].Trim ().Replace ("\"", "");
			String body = obj.Substring (obj.IndexOf ("["), obj.LastIndexOf ("]") - obj.IndexOf ("[")).Trim ();
			String[] heads = Regex.Split (head, "[:,]");
			if (heads.Length < 4) {
				throw new Exception("Not valid JSON! Head too short: " + heads.Length + " from head: \"" + head + "\"");
			}
			fmo.Id = long.Parse(heads[1]);
			fmo.Name = heads[3];
			String[] entries = Regex.Split(body, "[{}]");
			foreach (String entry in entries) {
				if(entry == "," || entry == "[" || entry == "") {
					continue;
				}
				FeMoEntry fme = new FeMoEntry();
				String t = entry.Replace ("\"", "");
				String[] ts = Regex.Split (t, "[:,]");
				if (ts.Length < 6) {
					throw new Exception("Not valid JSON! Entry too short: " + ts.Length + " from entry: \"" + t + "\"");
				}
				fme.Name = ts[1];
				fme.Value = ts[3];
				fme.Type = typeParser(ts[5]);
				fmo.AddEntry(fme);
			}
			return fmo;
		}

		private Type typeParser (String type)
		{
			switch (type) {
			case "BOOL":
				return Type.BOOL;
			case "DECIMAL":
				return Type.DECIMAL;
			case "STRING":
				return Type.STRING;
			case "INT":
				return Type.INT;
			case "OBJECT":
				return Type.OBJECT;
			default:
				throw new Exception("Unknown Entry Type: " + type);
			}
		}
	}
}


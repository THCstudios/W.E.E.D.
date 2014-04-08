using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLiteWrapper;
using System.Data;

namespace SharedMemory{
	public class TypenAPI : FeMoObject {
		public TypenAPI(FeMoObject femo) {
			int value;
			try {
				value = base.GetInt("EntityType");
			} catch (Exception e) {
				value = femo.GetInt("EntityType");
			}

			Id = femo.Id;
			Manager = femo.Manager;
			Convert.ToInt32(femo.GetInt("EntityType"));
			Console.WriteLine(value);
			SQLiteBase db = new SQLiteBase(@"F:\SYNC\Git\W.E.E.D\SharedMemory\bin\Debug\PlayerDB.sqlite");
			DataTable table = db.ExecuteQuery("SELECT * FROM def where id = " + value + ";");
			db.CloseDatabase();

			for (var i = 0; i <= table.Rows.Count - 1; i++) {
				FeMoEntry entry = new FeMoEntry();
				for (var j = 1; j <= table.Columns.Count - 1; j++) {
					var cell = table.Rows[i][j];
					Console.WriteLine(cell);
					if (j == 1) entry.Name = (String)cell;
					else if (j == 2) entry.Value = (String)cell;
					else if (j == 3 && Global.CastEnum((String)cell) != Type.UNKNOWN) entry.Type = Global.CastEnum((String)cell);
				}
				base.AddEntry(entry);
			}
			Manager.SendUpdateString();
			Global.DebugObject(this);
		}

		public TypenAPI(FeMoObject femo, String source) {
			int value;
			try {
				value = base.GetInt("EntityType");
			} catch (Exception e) {
				value = femo.GetInt("EntityType");
			}

			Id = femo.Id;
			Manager = femo.Manager;
			Convert.ToInt32(femo.GetInt("EntityType"));
			Console.WriteLine(value);
			SQLiteBase db = new SQLiteBase(source);
			DataTable table = db.ExecuteQuery("SELECT * FROM def where id = " + value + ";");
			db.CloseDatabase();

			for (var i = 0; i <= table.Rows.Count - 1; i++) {
				FeMoEntry entry = new FeMoEntry();
				for (var j = 1; j <= table.Columns.Count - 1; j++) {
					var cell = table.Rows[i][j];
					Console.WriteLine(cell);
					if (j == 1) entry.Name = (String)cell;
					else if (j == 2) entry.Value = (String)cell;
					else if (j == 3 && Global.CastEnum((String)cell) != Type.UNKNOWN) entry.Type = Global.CastEnum((String)cell);
				}
				base.AddEntry(entry);
			}
			Manager.SendUpdateString();
			Global.DebugObject(this);
		}
	}
}

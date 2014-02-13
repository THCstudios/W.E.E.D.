using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMemory {

	public delegate void ElementRemovingHandler(FeMoObject obj);
	public delegate void ElementRemovedHandler(FeMoObject obj);

	public class FeMoObject : IComparable{
		public enum FeMoInserState { works, not };
		public enum FeMoRemoveState { works, not };
		private ICollection<FeMoEntry> update;

		public event ElementRemovedHandler ElementRemoved;
		public event ElementRemovingHandler ElementRemoving;

		public virtual void OnElementRemoved (FeMoObject fmo)
		{
			if(ElementRemoved != null)
				ElementRemoved(fmo);
		}

		public virtual void OnElementRemoving (FeMoObject fmo)
		{
			if(ElementRemoving != null)
				ElementRemoving(fmo);
		}

		public void Merge (FeMoObject fmo)
		{
			if(fmo.id != id)
				return;
			if(fmo.Name != Name)
				Name = fmo.Name;
			foreach (FeMoEntry fme in fmo.Update) {
				if(FetchEntry(fme.Name) == null) {
					Current.Add(fme);
				} else 
				if(FetchEntry(fme.Name).Value != fme.Value) {
					FetchEntry(fme.Name).Value = fme.Value;
				}
				if(FetchForUpdate(fme.Name) == null) {
					AddEntry(fme);
				} else 
				if(FetchForUpdate(fme.Name).Value != fme.Value) {
					FetchForUpdate(fme.Name).Value = fme.Value;
				}
			}
		}

		public FeMoObject () {
			update = new SortedSet<FeMoEntry>();
			current = new SortedSet<FeMoEntry>();
		}

		internal ICollection<FeMoEntry> Update {
			get { return update; }
		}
		private ICollection<FeMoEntry> current;

		internal ICollection<FeMoEntry> Current {
			get { return current; }
			set { current = value; }
		}
		private long id;

		public long Id {
			set { this.id = value;}
			get { return id; }
		}
		private String name;

		public String Name {
			get { return name; }
			set { name = value; }
		}
		private FeMoManager manager;

		public FeMoManager Manager {
			get { return manager; }
			set { manager = value; }
		}
		private FeMoCreator creator;

		public FeMoCreator Creator {
			get { return creator; }
			set {creator = value; }
		}
		private FeMoPeer owner;

		public FeMoPeer Owner {
			get { return owner; }
			set { owner = value;}
		}
		private bool valid;

		public bool Valid {
			get {return valid;}
		}

		public FeMoInserState AddEntry(FeMoEntry entry) {
			try {
				Update.Add(entry);
				return FeMoInserState.works;
			} catch (Exception e) {
				return FeMoInserState.not;
			}
		}

		public int GetInt (String name)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.INT select e;
			if (erg.Count() == 1) {
				return Int32.Parse(erg.ToList().First().Value);
			}
			throw new Exception("Query \"GET INT " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public void AddInt (String name, int value)
		{
			FeMoEntry fme = new FeMoEntry();
			fme.Name = name;
			fme.Value = value + "";
			fme.Type = Type.INT;
			AddEntry(fme);
		}

		public void SetInt (String name, int value)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.INT select e;
			if (erg.Count() == 1) {
				erg.First().Value = "" + value;
			} else 
				throw new Exception("Query \"SET INT " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public Double GetDecimal (String name)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.DECIMAL select e;
			if (erg.Count() == 1) {
				return Double.Parse(erg.ToList().First().Value);
			} 
			throw new Exception("Query \"GET DECIMAL " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public void AddDecimal (String name, double value)
		{
			FeMoEntry fme = new FeMoEntry();
			fme.Name = name;
			fme.Value = value + "";
			fme.Type = Type.DECIMAL;
			AddEntry(fme);
		}

		public void SetDecimal (String name, double value)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.DECIMAL select e;
			if (erg.Count() == 1) {
				erg.First().Value = "" + value;
			} else
			throw new Exception("Query \"GST DECIMAL " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public String GetString (String name)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.STRING select e;
			if (erg.Count() == 1) {
				return erg.ToList().First().Value;
			} 
			throw new Exception("Query \"GET STRING " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}
		
		public void AddString (String name, String value)
		{
			FeMoEntry fme = new FeMoEntry();
			fme.Name = name;
			fme.Value = value;
			fme.Type = Type.STRING;
			AddEntry(fme);
		}
		
		public void SetString (String name, String value)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.STRING select e;
			if (erg.Count() == 1) {
				erg.First().Value = value;
			} else
			throw new Exception("Query \"SET STRING " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public FeMoObject GetObject (String name)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.OBJECT select e;
			if (erg.Count() == 1) {
				String obj = erg.ToList().First().Value;
				return manager.Get(long.Parse(obj));
			} 
			throw new Exception("Query \"GET OBJECT " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public void AddObject (String name, FeMoObject value)
		{
			FeMoEntry fme = new FeMoEntry();
			fme.Name = name;
			fme.Value = value.Id + "";
			fme.Type = Type.OBJECT;
			AddEntry(fme);
		}
		
		public void SetObject (String name, FeMoObject value)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.OBJECT select e;
			if (erg.Count() == 1) {
				erg.First().Value = value.Id + "";
			} else
			throw new Exception("Query \"SET OBJECT " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public bool GetBool (String name)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.BOOL select e;
			if (erg.Count() == 1) {
				return erg.ToList().First().Value == "T";
			} 
			throw new Exception("Query \"GET BOOL " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public void AddBool (String name, bool value)
		{
			FeMoEntry fme = new FeMoEntry();
			fme.Name = name;
			fme.Value = value ? "T" : "F";
			fme.Type = Type.BOOL;
			AddEntry(fme);
		}
		
		public void SetBool (String name, bool value)
		{
			var erg = from e in Current where e.Name == name && e.Type == Type.BOOL select e;
			if (erg.Count() == 1) {
				erg.First().Value = value ? "T" : "F";
			} else
			throw new Exception("Query \"SET BOOL " + name + " IN CURRENT\" returned illegal amount of Elements: " + erg.Count());
		}

		public FeMoRemoveState RemoveEntry(FeMoEntry entry) {
			try {
				Update.Remove(entry);
				return FeMoRemoveState.works;
			} catch (Exception e) {
				return FeMoRemoveState.not;
			}
		}

		public FeMoRemoveState RemoveEntry(String name) {
			try {
				Update.Remove(FetchEntry(name));
				return FeMoRemoveState.works;
			} catch (Exception e) {
				return FeMoRemoveState.not;
			}
		}

		public FeMoEntry FetchEntry(String name) {
			foreach (FeMoEntry f in Current) {
				if(f.Name == name){
					return f;
				}
			}
			return null;
		}

		private FeMoEntry FetchForUpdate (String name)
		{
			foreach (FeMoEntry f in Update) {
				if(f.Name == name){
					return f;
				}
			}
			return null;
		}

		public String UpdateString(FeMoUpdateStringFormatter formatter) {
			String ret = formatter.Format(this);
			Revalidate();
			return ret;
		}

		public override string ToString ()
		{
			FeMoUpdateStringFormatter formatter = Global.GetDefaultOutputFormatter();
			String ret = formatter.FormatObject (this);
			foreach (FeMoEntry e in Current) {
				ret += formatter.EntrySeparator();
				ret += formatter.FormatEntry(e);
			}
			return ret;
		}
			
		public void Invalidate() {
			valid = false;
		}

		public void Revalidate() {
			Current.Clear();
			foreach(FeMoEntry e in this.Update) {
				current.Add(e);
			}
			valid = true;
		}

		private void MarkValidation(bool v) {
			if (v) {
				Revalidate();
			} else {
				Invalidate();
			}
		}

		int IComparable.CompareTo(Object o) {
			if(!(o is FeMoObject))
				return 0;
			return (int)(((FeMoObject)o).id - id);
		}
	}
}

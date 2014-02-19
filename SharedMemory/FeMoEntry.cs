using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMemory {

	public enum Type {
		INT, DECIMAL, STRING, OBJECT, BOOL,UNKNOWN
	}

	public class FeMoEntry : IComparable {
		private String name;

		public String Name {
			get { return name; }
			set { name = value; Invalidate();}
		}
		private String value;

		public String Value {
			get { return this.value; }
			set { this.value = value; Invalidate();}
		}
		private bool changed;

		public bool Changed {
			get { return changed; }
			set {this.changed = value;}
		}

		private Type type;

		public Type Type {
			get { return type; }
			set { this.type = value;}
		}

		public String UpdateString(FeMoUpdateStringFormatter formatter) {
			String ret = formatter.FormatEntry(this);
			Changed = false;
			return ret;
		}

		public void Invalidate() {
			Changed = true;
		}

		public override bool Equals (object obj)
		{
			if (!(obj is FeMoEntry))
				return false;
			FeMoEntry fme = (FeMoEntry) obj;
			return fme.Value.Equals(Value) && fme.Name.Equals(Name);
		}

		int IComparable.CompareTo(object o) {
			if(!(o is FeMoEntry))
				return 0;
			return ((FeMoEntry)o).Name.CompareTo(Name);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections;

namespace SharedMemory
{
	public abstract class Target : IComparable
	{
		private long id;
		private String name;
		public List<Target> predesecors = new List<Target>();

		public abstract TargetState run();

		public long Id {
			get { return id;}
			set { id = value;}
		}

		public String Name {
			get { return name;}
			set { name = value;}
		}

		int IComparable.CompareTo (Object o)
		{
			if (o is Target) {
				Target t = (Target) o;
				return (int)(t.id - id);
			} else
				return 0;
		}
	}

	public enum TargetState {
		WAIT, FAIL, DONE
	}
}


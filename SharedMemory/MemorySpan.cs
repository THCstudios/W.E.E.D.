using System;

namespace SharedMemory
{
	public class MemorySpan
	{

		private char owner;

		public char Owner {
			get {
				return owner;
			}
			set {
				owner = value;
			}
		}

		private long low, top;

		private FeMoManager manager;

		private long current;

		public long Top {
			get {
				return top;
			}
			set {
				top = value;
			}
		}

		public long Low {
			get {
				return low;
			}
			set {
				low = value;
			}
		}

		public MemorySpan (long low, long top, FeMoManager manager, char owner)
		{
			this.low = low;
			this.top = top;
			this.manager = manager;
			this.owner = owner;
			current = low;
		}

		public long GetNextId ()
		{
			if (manager.LocalId == owner || owner == 'Z') {
				while(manager.Get(current) != null) {
					current++;
					if(current > top)
						current = low;
				}
				return current;
			}
			return -1;
		}
	}
}


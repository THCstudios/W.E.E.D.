using System;
using System.Collections.Generic;

namespace SharedMemory
{
	public class VirtualMemory : Dictionary<long, FeMoObject>
	{
		/*
		 * Current Memory instance
		 */
		private static VirtualMemory instance;
		/*
		 * 
		 */
		private MemorySpan[] memoryTable = new MemorySpan[26];

		private long amount;

		public static void Initialize (long amount)
		{
			instance = new VirtualMemory();
			instance.amount = amount;
			instance.allocMem();
		}

		public static void Dump ()
		{
			if(instance == null)
				throw new MemoryNotInitializedException("Dump not possible, no memory!");
			Console.WriteLine(instance.ToString());
		}

		public static void VarDump ()
		{
			Console.WriteLine("Used Space: " + instance.Keys.Count);
			Console.WriteLine("Free Space: " + (instance.amount - instance.Keys.Count));
			foreach(FeMoObject fmo in instance.Values) {
				Console.WriteLine(fmo.ToString());
			}
		}

		public static void WholeDump ()
		{
			Dump ();
			VarDump();
		}

		public static bool InMemory (long id)
		{
			try {
				return instance[id] != null;
			} catch (KeyNotFoundException) {
				return false;
			}
		}

		public static void Push (FeMoObject fmo)
		{
			if(fmo.ID > instance.amount)
				throw new MemoryAddressException("Out of Memory Range");
			if(InMemory(fmo.ID))
				throw new MemoryAddressException("Memory already in use");
			if(!InMemory(fmo.ID))
				instance.Add(fmo.ID, fmo);
		}

		public static FeMoObject Pull (long id)
		{
			if(!InMemory(id))
				throw new MemoryWrongIdException("No Element at this id found");
			if(InMemory(id))
				return instance[id];
			throw new Exception("Illegal Point of Operation");
		}

		private void allocMem() {
			long objs = (long)Math.Floor((double)(amount / 26));
			for(int i = 0; i < 25; ++i) {
				memoryTable[i] = new MemorySpan(objs * i, objs * (i + 1) - 1, (char)(i + 65));
			}
			memoryTable[25] = new MemorySpan(objs * 25, amount, 'Z');
		}

		public override string ToString ()
		{
			String ret = "Memory Table:\n";
			foreach (MemorySpan ms in memoryTable) {
				ret += ms.ID + " : " + ms.Begin + " - " + ms.End + "\n";
			}
			return ret;
		}
	}

	public class MemorySpan {

		private long begin;
		private long end;
		private char id;

		public char ID {
			get {
				return id;
			}
		}

		public long Begin {
			get {
				return begin;
			}
		}

		public long End {
			get {
				return end;
			}
		}

		public MemorySpan(long begin, long end, char id) {
			if(begin > end)
				throw new MemoryAllocationError("Illegal Memory Range (begin > end)");
			if(begin == end)
				throw new MemoryAllocationError("Illegal Memory Range (begin = end)");
			if(end - begin <= 50)
				throw new MemoryAllocationError("Illegal Memory Range (too few elements)");
			this.begin = begin;
			this.end = end;
			this.id = id;
		}
	}
}


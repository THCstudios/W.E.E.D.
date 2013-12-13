using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace SharedMemory
{
	public class VirtualMemory : Dictionary<long, FeMoObject>
	{
		/*
		 * Current Memory instance
		 */
		private static VirtualMemory instance;
		/*
		 * List of Memoryspans
		 */
		private MemorySpan[] memoryTable;

		/*
		 * The amount of reserved objects
		 */
		private long amount;
		/*
		 * Method to allocate the needed objects
		 */
		public static void Initialize (long amount, int parts)
		{
			instance = new VirtualMemory();
			instance.amount = amount;
			instance.allocMem(parts);
		}
		/*
		 * Destroys the Virtual Memory
		 */
		public static void Destroy ()
		{
			instance = null;
		}
		/*
		 * Returns a list containing the memory table
		 */
		public static List<MemorySpan> GetMemoryList ()
		{
			return instance.memoryTable.ToList<MemorySpan>();
		}
		/*
		 * Returns the fitting Memory Span
		 */
		public static MemorySpan GetMemorySpan (char id)
		{
			var x = from m in instance.memoryTable where m.ID == id select m;
			foreach (var y in x) {
				return (MemorySpan) y;
			}
			throw new MemoryAddressException ("Invalid Key");
		}
		/*
		 * Returns first free id of the given MemorySpan
		 */
		public static long GetFirstIdOf (MemorySpan span)
		{
			for (long i = span.Begin; i <= span.End; ++i) {
				if(!InMemory(i))
					return i;
			}
			throw new MemoryAddressException("Out of Range");
		}
		/*
		 * Dumps the MemorySpan list to the Console
		 */
		public static void Dump ()
		{
			if(instance == null)
				throw new MemoryNotInitializedException("Dump not possible, no memory!");
			Console.WriteLine(instance.ToString());
		}
		/*
		 * Dumps the whole memory to the Console
		 */
		public static void VarDump ()
		{
			Console.WriteLine("Used Space: " + instance.Keys.Count);
			Console.WriteLine("Free Space: " + (instance.amount - instance.Keys.Count));
			foreach(FeMoObject fmo in instance.Values) {
				Console.WriteLine(fmo.ToString());
			}
		}
		/*
		 * Performs a WholeDump to the Console
		 */
		public static void WholeDump ()
		{
			Dump ();
			VarDump();
		}
		/*
		 * Checks if an object is currently in memory
		 */
		public static bool InMemory (long id)
		{
			try {
				return instance[id] != null;
			} catch (KeyNotFoundException) {
				return false;
			}
		}
		/*
		 * Pushes an object into the Memory
		 */
		public static void Push (FeMoObject fmo)
		{
			if(fmo.ID > instance.amount)
				throw new MemoryAddressException("Out of Memory Range");
			if(InMemory(fmo.ID))
				throw new MemoryAddressException("Memory already in use");
			if(!InMemory(fmo.ID))
				instance.Add(fmo.ID, fmo);
		}
		/*
		 * Retrieves an object from the Memory
		 */
		public static FeMoObject Pull (long id)
		{
			if(!InMemory(id))
				throw new MemoryWrongIdException("No Element at this id found");
			if(InMemory(id))
				return instance[id];
			throw new Exception("Illegal Point of Operation");
		}
		/*
		 * Creates the Memory Table
		 */
		private void allocMem(int parts) {
			memoryTable = new MemorySpan[parts + 1];
			long objs = (long)Math.Floor((double)((double)amount / (double)(parts + 1)));
			for(int i = 0; i < parts; ++i) {
				memoryTable[i] = new MemorySpan(objs * i, objs * (i + 1) - 1, (char)(i + 65));
			}
			memoryTable[parts] = new MemorySpan(objs * parts, amount, 'Z');
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
		/*
		 * The begin of the memory span
		 */
		private long begin;
		/*
		 * The end of the memory span
		 */
		private long end;
		/*
		 * The id of the memory span
		 */
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


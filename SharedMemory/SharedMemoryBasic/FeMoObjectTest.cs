using System;
using 

namespace SharedMemory
{
	[TestFixture ()]
	public class FeMoObjectTest
	{
		[Test ()]
		public void TestCase ()
		{
			Assert.Throws<MemoryNotInitializedException> (VirtualMemory.Dump);
			Assert.Throws<MemoryAllocationError> (throw_1);
			Assert.Throws<MemoryAllocationError> (throw_2);
			Assert.Throws<MemoryAllocationError> (throw_3);
			Assert.DoesNotThrow (throw_4);
			VirtualMemory.Initialize (100000);
			VirtualMemory.Dump ();
			Assert.Throws<MemoryNotInitializedException>(VirtualMemory.Dump);
			Assert.Throws<MemoryAllocationError>(throw_1);
			Assert.Throws<MemoryAllocationError>(throw_2);
			Assert.Throws<MemoryAllocationError>(throw_3);
			Assert.DoesNotThrow(throw_4);
			VirtualMemory.Initialize(100000, 25);
			VirtualMemory.Dump();
			VirtualMemory.Destroy();
		}

		[Test ()]
		public void VirtualMemoryTest ()
		{
			FeMoObject fmo = new FeMoObject ();
			fmo.ID = 12;
			fmo.Add ("test", "test");
			fmo.Name = "Test";
			VirtualMemory.Initialize (10000);
			VirtualMemory.Push (fmo);
			VirtualMemory.VarDump ();
			FeMoObject fmo_ = new FeMoObject ();
			fmo_.ID = 13;
			fmo_.Name = "TestContainer";
			VirtualMemory.Push (fmo_);
			VirtualMemory.VarDump ();
			fmo_.AddObject ("lol", fmo);
			VirtualMemory.VarDump ();
			VirtualMemory.Initialize(10000, 2);
			VirtualMemory.Push(fmo);
			VirtualMemory.WholeDump();
			FeMoObject fmo_ = new FeMoObject();
			fmo_.ID = 13;
			fmo_.Name = "TestContainer";
			VirtualMemory.Push(fmo_);
			VirtualMemory.VarDump();
			fmo_.AddObject("lol", fmo);
			VirtualMemory.VarDump();
			FeMoObject f_ = fmo_.GetObject("lol");
			Console.WriteLine(f_.ToString());
			Console.WriteLine(f_.GetString("test"));
			VirtualMemory.Destroy();
		}

		[Test()]
		public void MemorySpanTest ()
		{
			VirtualMemory.Initialize(10000, 5);
			VirtualMemory.Dump();
			FeMoObject fmo = new FeMoObject();
			fmo.ID = VirtualMemory.GetFirstIdOf(VirtualMemory.GetMemorySpan('B'));
			fmo.Name = "Test";
			fmo.AddString("test", "test");
			VirtualMemory.Push(fmo);
			FeMoObject fmo_ = new FeMoObject();
			fmo_.ID = VirtualMemory.GetFirstIdOf(VirtualMemory.GetMemorySpan('C'));
			fmo_.Name = "ContainerTest";
			fmo_.AddObject("lol", fmo);
			VirtualMemory.Push(fmo_);
			VirtualMemory.VarDump();
			VirtualMemory.Destroy();
		}

		private void throw_1 ()
		{
			new MemorySpan (40, 30, 'A');
		}

		private void throw_2 ()
		{
			new MemorySpan (40, 40, 'B');
		}

		private void throw_3 ()
		{
			new MemorySpan (40, 50, 'C');
		}

		private void throw_4 ()
		{
			new MemorySpan (0, 10000, 'D');
		}
	}
}

using System;
using NUnit.Framework;

namespace SharedMemory
{
	[TestFixture()]
	public class ConsoleFormatterTest
	{
		[Test()]
		public void TestCase ()
		{
			FeMoObject fmo = new FeMoObject();
			FeMoEntry fme = new FeMoEntry();
			fme.Name = "TestString";
			fme.Value = "Test";
			fmo.AddEntry(fme);
			Console.WriteLine(fmo.UpdateString(FeMoUpdateStringFormatter.CONSOLE));
		}
	}
}


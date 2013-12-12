
using System;

namespace SharedMemory {
	public class MemoryAllocationError : System.Exception {

		public MemoryAllocationError(String msg) : base(msg) {

		}
	}

	public class MemoryNotInitializedException : System.Exception {
		public MemoryNotInitializedException (String msg) : base(msg)
		{

		}
	}

	public class MemoryAddressException : System.Exception {
		public MemoryAddressException (String msg) : base(msg)
		{

		}
	}

	public class MemoryWrongIdException : System.Exception {
		public MemoryWrongIdException (String msg) : base(msg)
		{

		}
	}
}

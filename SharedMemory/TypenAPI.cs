using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedMemory {
	class TypenAPI {
		long id;

		public long Id {
			get { return id; }
			set { id = value; }
		}



		FeMoManager manager;

		public FeMoManager Manager {
			get { return manager; }
			set { manager = value; }
		}

		public TypenAPI(FeMoObject femo) {
			if (femo.FetchEntry("EntityType") != null) {
				Id = femo.Id;
				Manager = femo.Manager;
			} else {
				throw new Exception("No EntityType");
			}


		}
	}
}

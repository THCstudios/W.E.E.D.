using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SharedMemory
{
	public delegate void ReadObjectHandler(FeMoObject fmo);

	public class FeMoCreator
	{
		private FeMoManager manager;
		private FeMoPeer connection;
		private IList<FeMoObject> cache = new List<FeMoObject>();

		public event ReadObjectHandler ReadObject;

		protected virtual void OnReadObject (FeMoObject fmo)
		{
			if(ReadObject != null)
				ReadObject(fmo);
		}


		public FeMoManager Manager {
			get {
				return manager;
			}
			set {
				manager = value;
			}
		}

		public FeMoPeer Connection {
			get {
				return connection;
			}
			set {
				connection = value;
			}
		}

		public FeMoCreator (FeMoManager manager, FeMoPeer connection) {
			connection.ReceivedObject += HandleObjectReceived;
			ReadObject += manager.HandleOnObjectRead;
			Connection = connection;
			Manager = manager;
		}

		private void HandleObjectReceived(String objString) {
			FeMoUpdateStringFormatter formatter = Global.GetDefaultFormatter();
			FeMoObject[] fmos = formatter.Parse(objString);
			foreach (FeMoObject fmo in fmos) {
				fmo.Creator = this;
				fmo.Manager = Manager;
				fmo.Owner = Connection;
				fmo.Revalidate();
				OnReadObject(fmo);				
			}
		}

		public void RemoveObject(long id) {
			FeMoObject obj = (from o in cache where o.Id == id select o).ToList().First();
			cache.Remove(obj);
		}

		public void RemoveObjectClean(long id) {
			FeMoObject obj = (from o in cache where o.Id == id select o).ToList().First();
			obj.OnElementRemoving(obj);
			cache.Remove(obj);
			obj.OnElementRemoved(obj);
		}
	}
}


using System;

namespace SharedMemory
{
	public class FeMoList
	{

		private long id;
		private FeMoManager man;

		public FeMoList (FeMoObject fmo)
		{
			id = fmo.Id;
			man = fmo.Manager;
			int count;
			try {
				count = fmo.GetInt ("Count");
				this.Count = count;
			} catch (Exception e) {
				Global.warn("Creating new List " + fmo.Name);
				this.Count = 0;
				fmo.AddInt("Count", 0);
			}
		}

		private int Count {
			get {
				FeMoObject o = man.Get(id);
				return o.GetInt("Count");
			}
			set {
				FeMoObject o = man.Get(id);
				o.SetInt("Count", value);
			}
		}

		public FeMoObject GetIndex(int index) {
			if(index >= Count)
				throw new IndexOutOfRangeException("Index " + index + " not in List " + man.Get(id).Name);
			FeMoObject o = man.Get(id);
			return o.GetObject(o.Name + "_" + index);
		}

		public void AddObject(FeMoObject obj) {
			FeMoObject o = man.Get(id);
			o.Invalidate();
			o.AddObject(o.Name + "_" + Count, obj);
			Count++;
		}

		public void RemoveObject (int index) {
			FeMoObject o = man.Get(id);
			try {
				FeMoObject o_ = GetIndex(index);
				for(int i = index; i < Count - 1; i++) {
					o.SetObject(o.Name + "_" + i, o.GetObject(o.Name + "_" + (i +1)));
				}
				Count--;
				o.RemoveEntry(o.Name + "_" + Count);
				o.Invalidate();
			} catch (Exception any) {
				if(any is IndexOutOfRangeException)
					throw new IndexOutOfRangeException("Couldn't remove object at index " + index + "! Not in List.", any);
				throw any;
			}
		}
	}
}


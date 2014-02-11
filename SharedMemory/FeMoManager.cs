using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SharedMemory
{
	/*
	 * TODO: 
	 * 		-Addressbereiche und zuweisungen der Objekte 
	 * 		-Entries mit Typ
	 * 		-Anstatt FeMoObject.AddEntry() FeMoObject.AddInt(), FeMoObject.SetInt() ...
	 */
	public class FeMoManager
	{
		private SortedDictionary<long, FeMoObject> cache = new SortedDictionary<long, FeMoObject>();
		private List<FeMoCreator> creators = new List<FeMoCreator>();
		private List<FeMoPeer> peers = new List<FeMoPeer>();

		public void AddConnection (FeMoPeer peer)
		{
			peers.Add(peer);
			creators.Add(new FeMoCreator(this,peer));
		}

		public long CacheObject (FeMoObject obj)
		{
			if (!cache.ContainsValue (obj)) {
				cache.Add (obj.Id, obj);
				obj.Manager = this;
			}
			else
				cache[obj.Id] = obj;
			return obj.Id;
		}

		public FeMoObject UncacheObject(long id) {
			if(!cache.ContainsKey(id))
				return null;
			FeMoObject tmp = cache[id];
			cache.Remove(id);
			return tmp;
		}

		public int ObjectCount() {
			return cache.Count;
		}

		String GenereateUpdateString (FeMoUpdateStringFormatter formatter)
		{
			List<FeMoObject> objs = (from o in cache.Values where !o.Valid select o).ToList ();
			String ret = formatter.OpenObjectList();
			for (int i = 0; i < objs.Count(); ++i) {
				if(i != 0)
					ret += formatter.ObjectSeparator();
				ret += ((FeMoObject)objs[i]).UpdateString(formatter);
			}
			ret += formatter.CloseObjectList();
			return ret;
		}

		public String Dump(FeMoUpdateStringFormatter formatter) {
			List<FeMoObject> objs = (from o in cache.Values select o).ToList ();
			String ret = formatter.OpenObjectList();
			for (int i = 0; i < objs.Count(); ++i) {
				if(i != 0)
					ret += formatter.ObjectSeparator();
				ret += formatter.Dump(objs[i]);
			}
			ret += formatter.CloseObjectList();
			return ret;
		}

		public void SendUpdateString ()
		{
			String up = GenereateUpdateString(Global.GetDefaultFormatter());
			foreach (FeMoPeer peer in peers) {
				peer.SendObject(up);
			}
		}

		public int Updates() {
			return (from o in cache.Values where !o.Valid select o).Count();
		}

		public bool CheckForUpdates() {
			return Updates() > 0;
		}

		public FeMoObject Get (long id)
		{
			if(cache.ContainsKey(id))
				return cache[id];
			throw new Exception("Query: \"GET OBJECT id=" + id + " IN CACHE\" failed. No such Object!");
		}


		public String CacheInfo() {
			return String.Format("Objects: {0, 10} Updates: {1,5} AddressRanges: {2, 3}", cache.Count, this.Updates(), 0);
		}

		public void HandleOnObjectRead (FeMoObject fmo)
		{
			if (cache.ContainsKey (fmo.Id)) {
				cache [fmo.Id].Merge (fmo);
			} else {
				cache.Add(fmo.Id, fmo);
			}
		}

		public void BroadcastCommand (String msg)
		{
			foreach (FeMoPeer p in peers) {
				p.SendCommand(msg);
			}
		}

		public void Close ()
		{
			foreach (FeMoPeer p in peers) {
				try {
					p.Close();
				} catch {
					Global.femo ("Connection already shutdown");
				}
			}
		}

		public void FileDump (String file, FeMoUpdateStringFormatter formatter)
		{
			File.WriteAllText(file, Dump(formatter));
		}

		public void ReadFromFile (String file, FeMoUpdateStringFormatter formatter) {
			FeMoObject[] objs = formatter.Parse(File.ReadAllText(file));
			foreach (FeMoObject obj in objs) {
				CacheObject(obj);
			}
		}

		public void ReadFromString (String value, FeMoUpdateStringFormatter formatter) {
			FeMoObject[] objs = formatter.Parse(value);
			foreach (FeMoObject obj in objs) {
				CacheObject(obj);
			}
		}
	}
}


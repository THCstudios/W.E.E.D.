using System;
using System.Collections.Generic;

<<<<<<< HEAD
namespace SharedMemory {
  public class FeMoObject : Dictionary<String, String> {
    private string name;
    private float version = 0.1f;
    private long id;

    public Int64 ID {
      set {
        id = value;
      }
      get {
        return id;
      }
    }
=======
namespace SharedMemory
{
	public class FeMoObject : Dictionary<String, String> {
		/*
		 * Stores the name of the object
		 */
		private string name;
		/*
		 * Stores the version of the object
		 * Needed for compatibility checks
		 */
		private float version = 0.1f;
		/*
		 * Stores the memory address of the object
		 */
		private long id;
>>>>>>> e1ab3218b281cf2674b72592bacbd62d6f84ae10

    public String Name {
      set {
        name = value;
      }
      get {
        return name;
      }
    }

    public new void Add(String key, String value) {
      String key_ = getTypePrefix(value) + key;
      String value_ = optimizeValue(key, value);
      base.Add(key_, value_);
    }

<<<<<<< HEAD
    public void AddInt(String key, Int64 value) {
      Add(key, value + "");
    }

    public void AddFloat(String key, float value) {
      Add(key, value + "");
    }

    public void AddDouble(String key, double value) {
      Add(key, value + "");
    }

    public void AddObject(String key, FeMoObject value) {
      Add(key, value.ToString());
    }

    public void AddString(String key, String value) {
      Add(key, value);
    }

    private String optimizeValue(String key, String value) {
      String key_ = getTypePrefix(value);
      if (key_.StartsWith("o_")) {
        String[] p = value.Split("[".ToCharArray());
        String[] parts = p[2].Split("||".ToCharArray());
        String[] p_ = parts[4].Split("=".ToCharArray());
        String id = p_[1].Split("]".ToCharArray())[0];
        Int64 i_;
        if (!Int64.TryParse(id, out i_))
          throw new MemoryWrongIdException("Parse Error");
        Int64 id_ = Int64.Parse(id);
        if (VirtualMemory.InMemory(id_))
          return "fmo_" + id_;
        else
          return "mem_err";
      }
      return value;
    }

    private String getTypePrefix(String value) {
      float f = 0.0f;
      double d = 0.0d;
      Int64 i = 0;
      if (Int64.TryParse(value, out i))
        return "i_";
      if (float.TryParse(value, out f))
        return "f_";
      if (Double.TryParse(value, out d))
        return "d_";
      if (value.Contains("fmo:=["))
        return "o_";
      return "s_";
    }

    public override string ToString() {
      string head = "fmo:=[head:=[name:=" + name + "||vers:=" + version + "||id=" + id + "]";
      head += "||body:=[";
      foreach (String key in Keys) {
        head += key + ":=" + this[key] + "||";
      }
      if (Keys.Count > 0)
        head = head.Substring(0, head.Length - 2);
      head += "]]";
      return head;
    }
  }
=======
		public float Version {
			get {
				return version;
			}
			set {
				version = value;
			}
		}

		/*
		 * Plain add method, as long as you know the type please use the other methods
		 * Should only be used by the FeMoReader
		 */
		public new void Add (String key, String value)
		{
			String key_ = getTypePrefix(value) + key;
			String value_ = optimizeValue(key, value);
			base.Add(key_, value_);
		}
		/*
		 * Plain get method, as long as you know the type please use the other methods
		 * Should only be used by FeMoDumper
		 */
		public String Get (String key)
		{
			foreach(String k in Keys) {
				if(k.Substring(2) == key) {
					return this[k];
				}
			}
			throw new MemoryIllegalKeyException("Key not existing");
		}
		/*
		 * Convinience method for int adding, please use this instead of Add(String, String)
		 */
		public void AddInt (String key, Int64 value)
		{
			Add (key, value + "");
		}
		/*
		 * Convinience method for int pulling, please use this instead of Get(String) 
		 */
		public Int64 GetInt (String key)
		{
			String val = Get (key);
			Int64 i;
			if(Int64.TryParse(val, out i))
				return Int64.Parse(val);
			throw new MemoryIllegalKeyException("No readable Int");
		}
		/*
		 * Convinience method for float adding, please use this instead of Add(String, String)
		 */
		public void AddFloat (String key, float value)
		{
			Add (key, value + "");
		}
		/*
		 * Convinience method for float pulling, please use this instead of Get(String) 
		 */
		public float GetFloat (String key) {
			String val = Get (key);
			float f;
			if(float.TryParse(key, out f))
				return float.Parse(val);
			throw new MemoryIllegalKeyException("No readable Float");
		}
		/*
		 * Convinience method for double adding, please use this instead of Add(String, String)
		 */
		public void AddDouble (String key, double value)
		{
			Add (key, value + "");
		}
		/*
		 * Convinience method for double pulling, please use this instead of Get(String) 
		 */
		public double GetDouble (String key) {
			String val = Get (key);
			double d;
			if(double.TryParse(key, out d))
				return double.Parse(val);
			throw new MemoryIllegalKeyException("No readable Double");
		}
		/*
		 * Convinience method for Object adding, please use this instead of Add(String, String)
		 */
		public void AddObject (String key, FeMoObject value)
		{
			Add (key, value.ToString());
		}
		/*
		 * Convinience method for Object pulling, please use this instead of Get(String) 
		 */
		public FeMoObject GetObject (String key)
		{
			String val = Get (key);
			if(val == "mem_err")
				throw new MemoryAddressException("Illegal Memory Address");
			if(!val.StartsWith("fmo_"))
				throw new MemoryWrongFormatException("Memory Address not valid");
			long l;
			if(!long.TryParse(val.Substring(4), out l))
			   throw new MemoryWrongFormatException("Memory Address not valid");
			long id = long.Parse(val.Substring(4));
			if(!VirtualMemory.InMemory(id))
				throw new MemoryAddressException("Illegal Memory Address");
			return VirtualMemory.Pull(id);
		}
		/*
		 * Convinience method for String adding, please use this instead of Add(String, String)
		 */
		public void AddString (String key, String value)
		{
			Add (key, value);
		}
		/*
		 * Convinience method for String pulling, please use this instead of Get(String) 
		 */
		public String GetString (String key)
		{
			return Get (key);
		}
		/*
		 * Method to optimize the value
		 */
		private String optimizeValue (String key, String value)
		{
			String key_ = getTypePrefix (value);
			if (key_.StartsWith("o_")) {
				String[] p = value.Split("[".ToCharArray());
				String[] parts = p[2].Split("||".ToCharArray());
				String[] p_ = parts[4].Split("=".ToCharArray());
				String id = p_[1].Split("]".ToCharArray())[0];
				Int64 i_;
				if(!Int64.TryParse(id, out i_))
					throw new MemoryWrongIdException("Parse Error");
				Int64 id_ = Int64.Parse(id);
				if(VirtualMemory.InMemory(id_))
					return "fmo_" + id_;
				else
					return "mem_err";
			}
			return value;
		}
		/*
		 * Method to find the type prefix for the key
		 */
		private String getTypePrefix(String value) {
			float f = 0.0f;
			double d = 0.0d;
			Int64 i = 0;
			if(Int64.TryParse(value, out i))
				return "i_";
			if(float.TryParse(value, out f))
				return "f_";
			if(Double.TryParse(value, out d))
				return "d_";
			if(value.Contains("fmo:=["))
				return "o_";
			return "s_";
		}

		public override string ToString ()
		{
			string head =  "fmo:=[head:=[name:=" + name + "||vers:=" + version + "||id=" + id + "]";
			head += "||body:=[";
			foreach(String key in Keys) {
				head += key + ":=" + this[key] + "||";
			}
			if(Keys.Count > 0)
				head = head.Substring(0, head.Length - 2);
			head += "]]";
			return head;
		}
	}
>>>>>>> e1ab3218b281cf2674b72592bacbd62d6f84ae10
}


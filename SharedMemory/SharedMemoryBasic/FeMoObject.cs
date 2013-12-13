using System;
using System.Collections.Generic;

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
}


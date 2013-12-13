using System;
using System.Collections.Generic;

namespace SharedMemory {
  public class FeMoReader {
    public static List<FeMoObject> ReadObject(String femo) {
      String[] lines = femo.Split("\n");
      List<FeMoObject> list = new List<FeMoObject>();
      foreach (String str in lines) {
        int i = str.IndexOf("body:=[");
        String head = str.Substring(0, i - 3);
        String body = str.Substring(i + 7);

        //Head Splitting
        i = head.IndexOf("head:=");
        head = head.Substring(i);
        i = head.IndexOf("name:=");
        head = head.Substring(i);


        FeMoObject feMoOb = new FeMoObject();

        String[] headS = head.Split("||");
        foreach (String s in headS) {
          switch (s) {
            case s.StartsWith("name"):
              feMoOb.Name = s.Substring(s.IndexOf(":=") + 2);
              break;
            case s.StartsWith("vers"):
              feMoOb.Version = s.Substring(s.IndexOf(":=") + 2);
              break;
            case s.StartsWith("id"):
              feMoOb.ID = s.Substring(s.IndexOf(":=") + 2);
              break;
          }
        }

        //Body splitting
        body = head.Substring(i);

        String[] bodyS = head.Split("||");
        foreach (String s in bodyS) {
          String[] forsplit = s.Split(":=");
          feMoOb.Add(forsplit[0], forsplit[1]);
        }
        list.Add(feMoOb);
      }
      return list;
    }
  }
}

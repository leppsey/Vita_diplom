// Decompiled with JetBrains decompiler
// Type: PmToIfc.Program
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace PmToIfc
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      eMode mode = eMode.Ifc4;
      bool smode = false;
      string seed = ".\\IfcSeed.Xml";
      for (int index = 0; index < args.Length; ++index)
      {
        string str1 = args[index];
        string str2 = str1.Substring(0, 1);
        if (str2 == "-" || str2 == "/")
          stringList2.Add(str1);
        else
          stringList1.Add(str1);
      }
      foreach (string str3 in stringList2)
      {
        string str4 = str3.Substring(1, 1);
        int num = str3.IndexOf("=");
        if (num > 1)
          str3.Substring(num + 1);
        switch (str4)
        {
          case "M":
          case "m":
            string str5 = str3.Substring(2);
            if (str5 == "2X3" || str5 == "2x3")
            {
              mode = eMode.Ifc2x3;
              continue;
            }
            continue;
          case "S":
          case "s":
            seed = str3.Substring(2);
            smode = true;
            continue;
          default:
            continue;
        }
      }
      Def.DefInitialize(smode, seed);
      foreach (string path1 in stringList1)
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(path1);
        string path2 = Path.GetDirectoryName(path1) + "\\" + withoutExtension + ".ifc";
        Console.WriteLine("PmToIfc Started:{0} Mode={1}", (object) path2, (object) mode);
        try
        {
          FileStream fileStream = File.Open(path1, (FileMode) 3);
          DmToIfc.Create(new BinaryReader((Stream) fileStream), path2, mode);
          ((Stream) fileStream).Close();
        }
        catch (FileNotFoundException ex)
        {
          Console.WriteLine("File does not exist. Run Export Ifc first.");
        }
      }
    }
  }
}

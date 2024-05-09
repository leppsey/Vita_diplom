// Decompiled with JetBrains decompiler
// Type: PmToIfc.Def
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

#nullable disable
namespace PmToIfc
{
  public static class Def
  {
    public const double e = 0.0001;
    public static double MtoMM = 1000.0;
    public static bool StyleMode = false;
    public static Dictionary<ePmType, IfcColor> Colors = new Dictionary<ePmType, IfcColor>();

    public static void DefInitialize(bool smode, string seed)
    {
      Def.Colors.Add(ePmType.Generic, new IfcColor()
      {
        R = 105,
        G = 105,
        B = 105,
        A = 0
      });
      Def.Colors.Add(ePmType.Equipment, new IfcColor()
      {
        R = 30,
        G = 144,
        B = (int) byte.MaxValue,
        A = 0
      });
      Def.Colors.Add(ePmType.Shape, new IfcColor()
      {
        R = 30,
        G = 144,
        B = (int) byte.MaxValue,
        A = 0
      });
      Def.Colors.Add(ePmType.Nozzle, new IfcColor()
      {
        R = 75,
        G = 0,
        B = 130,
        A = 0
      });
      Def.Colors.Add(ePmType.PipeComponent, new IfcColor()
      {
        R = 75,
        G = 0,
        B = 130,
        A = 0
      });
      Def.Colors.Add(ePmType.PipeSupport, new IfcColor()
      {
        R = 75,
        G = 0,
        B = 130,
        A = 0
      });
      Def.Colors.Add(ePmType.Member, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.Column, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.Beam, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.Slab, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.Wall, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.CwayComponent, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      Def.Colors.Add(ePmType.DuctComponent, new IfcColor()
      {
        R = 178,
        G = 34,
        B = 34,
        A = 0
      });
      if (!smode)
        return;
      Def.XSeedStyle(seed);
    }

    public static void XSeedStyle(string xfile)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      try
      {
        XPathDocument xpathDocument = new XPathDocument(xfile);
        if (xpathDocument == null)
          return;
        Console.WriteLine("***IFC Seed:{0}", (object) xfile);
        XPathNavigator navigator = xpathDocument.CreateNavigator();
        Def.Colors.Clear();
        XPathNodeIterator xpathNodeIterator = navigator.Select("//IfcStyle");
        XPathNavigator current = xpathNodeIterator.Current;
        while (xpathNodeIterator.MoveNext())
        {
          ePmType pmType = Def.ParsePmType(current.GetAttribute("PmType", string.Empty));
          int num1 = Def.GetInt(current, "R");
          int num2 = Def.GetInt(current, "G");
          int num3 = Def.GetInt(current, "B");
          int num4 = Def.GetInt(current, "A");
          Def.Colors.Add(pmType, new IfcColor()
          {
            R = num1,
            G = num2,
            B = num3,
            A = num4
          });
        }
        Def.StyleMode = true;
      }
      catch
      {
        Console.WriteLine("IFC Seed:{0} Failed", (object) xfile);
        Def.StyleMode = false;
      }
    }

    public static int GetInt(XPathNavigator node, string name)
    {
      int result;
      return !int.TryParse(node.GetAttribute(name, string.Empty), out result) ? 0 : result;
    }

    public static ePmType ParsePmType(string name)
    {
      ePmType pmType = ePmType.Generic;
      if (string.IsNullOrEmpty(name))
        return pmType;
      try
      {
        pmType = (ePmType) Enum.Parse(typeof (ePmType), name);
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine("'{0}' is not a member of the PmType enumeration.", (object) name);
      }
      return pmType;
    }
  }
}

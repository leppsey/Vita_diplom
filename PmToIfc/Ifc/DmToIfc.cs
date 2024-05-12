// Decompiled with JetBrains decompiler
// Type: PmToIfc.DmToIfc
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.IO;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.IO;

#nullable disable
namespace PmToIfc
{
  internal class DmToIfc
  {
    public static object Storey;
    public static object Site;

    public static void Create(BinaryReader reader, string path, eMode mode = eMode.Ifc4)
    {
      IfcStore model = (IfcStore) null;
      switch (mode)
      {
        case eMode.Ifc4:
          model = InitIFCProject4.CreateModel("PmToIFC4");
          DmToIfc.Site = (object) InitIFCProject4.CreateSite(model, "Site");
          IfcStyles4.CreateStyles(model);
          break;
        case eMode.Ifc2x3:
          model = InitIFCProject2x3.CreateModel("PmToIFC2X3");
          DmToIfc.Site = (object) InitIFCProject2x3.CreateSite(model, "Site");
          IfcStyles2x3.CreateStyles(model);
          break;
      }
      if (model == (IModel) null)
        return;
      Console.WriteLine("*Read Pmb Version:{0}", (object) reader.ReadInt32());
      DmToIfc.ReadPmb(reader, model, mode);
      model.SaveAs(path, new StorageType?(StorageType.Ifc), (ReportProgressDelegate) null);
    }

    public static void ReadPmb(BinaryReader reader, IfcStore model, eMode mode)
    {
      using (ITransaction transaction = model.BeginTransaction(nameof (ReadPmb)))
      {
        Import.Read(reader).TraverseDown(model, mode);
        transaction.Commit();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: PmToIfc.IfcMaterials4
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

#nullable disable
namespace PmToIfc
{
  internal class IfcMaterials4
  {
    public static Dictionary<int, IfcRelAssociatesMaterial> Materials;

    public static void CreateMaterials(IfcStore model)
    {
      IfcMaterials4.Materials = new Dictionary<int, IfcRelAssociatesMaterial>();
      using (ITransaction transaction = model.BeginTransaction(nameof (CreateMaterials)))
      {
        for (int key = 0; key < 5; ++key)
        {
          IfcMaterial ifcMaterial = model.Instances.New<IfcMaterial>();
          ifcMaterial.Name = (IfcLabel) ("STEEL/C245" + key.ToString());
          IfcRelAssociatesMaterial associatesMaterial = model.Instances.New<IfcRelAssociatesMaterial>();
          associatesMaterial.RelatingMaterial = (IfcMaterialSelect) ifcMaterial;
          IfcMaterials4.Materials.Add(key, associatesMaterial);
        }
        transaction.Commit();
      }
    }
  }
}

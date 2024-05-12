// Decompiled with JetBrains decompiler
// Type: PmToIfc.Properties4
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

#nullable disable
namespace PmToIfc
{
  internal class Properties4
  {
    public static void Set(IfcStore model, IfcProduct element, Metadata md)
    {
      model.Instances.New<IfcRelDefinesByProperties>((Action<IfcRelDefinesByProperties>) (rel =>
      {
        ((ICollection<IfcObjectDefinition>) rel.RelatedObjects).Add((IfcObjectDefinition) element);
        rel.RelatingPropertyDefinition = (IfcPropertySetDefinitionSelect) model.Instances.New<IfcPropertySet>((Action<IfcPropertySet>) (pset =>
        {
          pset.Name = new IfcLabel?((IfcLabel) "Metadata");
          IfcPropertySingleValue[] propertySingleValueArray = new IfcPropertySingleValue[md.Count];
          int num3 = 0;
          foreach (string key2 in md.Keys)
          {
            string key = key2;
            Metadata.Entry entry = md[key];
            switch (entry.DataType)
            {
              case MetaDataType.Bool:
                int num4 = (bool) entry.Data ? 1 : 0;
                continue;
              case MetaDataType.Int32:
                int ivalue = (int) entry.Data;
                propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>((Action<IfcPropertySingleValue>) (p =>
                {
                  p.Name = (IfcIdentifier) key;
                  p.NominalValue = (IfcValue) new IfcInteger((long) ivalue);
                }));
                continue;
              case MetaDataType.UInt64:
                ulong uvalue = (ulong) entry.Data;
                propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>((Action<IfcPropertySingleValue>) (p =>
                {
                  p.Name = (IfcIdentifier) key;
                  p.NominalValue = (IfcValue) new IfcInteger((long) uvalue);
                }));
                continue;
              case MetaDataType.Float:
                float fvalue = (float) entry.Data;
                propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>((Action<IfcPropertySingleValue>) (p =>
                {
                  p.Name = (IfcIdentifier) key;
                  p.NominalValue = (IfcValue) new IfcLengthMeasure((double) fvalue);
                }));
                continue;
              case MetaDataType.Double:
                double dvalue = (double) entry.Data;
                propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>((Action<IfcPropertySingleValue>) (p =>
                {
                  p.Name = (IfcIdentifier) key;
                  p.NominalValue = (IfcValue) new IfcLengthMeasure(dvalue);
                }));
                continue;
              case MetaDataType.String:
                string sdata = entry.Data as string;
                propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>((Action<IfcPropertySingleValue>) (p =>
                {
                  p.Name = (IfcIdentifier) key;
                  p.NominalValue = (IfcValue) new IfcText(sdata);
                }));
                continue;
              case MetaDataType.Vector3D:
                Vector3 data = (Vector3) entry.Data;
                continue;
              default:
                continue;
            }
          }
          pset.HasProperties.AddRange((IEnumerable<IfcProperty>) propertySingleValueArray);
        }));
      }));
    }
  }
}

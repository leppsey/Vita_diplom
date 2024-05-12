// Decompiled with JetBrains decompiler
// Type: PmToIfc.Properties2x3
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PropertyResource;

#nullable disable
namespace PmToIfc
{
    internal class Properties2x3
    {
        public static void Set(IfcStore model, IfcProduct element, Metadata md)
        {
            if ((PersistEntity)element == (object)null)
                return;
            model.Instances.New<IfcRelDefinesByProperties>((Action<IfcRelDefinesByProperties>)(rel =>
            {
                ((ICollection<IfcObject>)rel.RelatedObjects).Add((IfcObject)element);
                rel.RelatingPropertyDefinition = (IfcPropertySetDefinition)model.Instances.New<IfcPropertySet>(
                    (Action<IfcPropertySet>)(pset =>
                    {
                        pset.Name = new IfcLabel?((IfcLabel)"Metadata");
                        var arrayCount = md.Values.Count(entry =>
                        {
                            MetaDataType[] types = new[]
                            {
                                MetaDataType.Int32, MetaDataType.UInt64, MetaDataType.Float, MetaDataType.Double,
                                MetaDataType.String
                            };
                            return types.Contains(entry.DataType);
                        });
                        IfcPropertySingleValue[] propertySingleValueArray = new IfcPropertySingleValue[arrayCount];
                        int num3 = 0;
                        foreach (string key2 in md.Keys)
                        {
                            string key = key2;
                            Metadata.Entry entry = md[key];
                            switch (entry.DataType)
                            {
                                case MetaDataType.Bool:
                                    int num4 = (bool)entry.Data ? 1 : 0;
                                    continue;
                                case MetaDataType.Int32:
                                    int ivalue = (int)entry.Data;
                                    propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>(
                                        (Action<IfcPropertySingleValue>)(p =>
                                        {
                                            p.Name = (IfcIdentifier)key;
                                            p.NominalValue = (IfcValue)new IfcInteger((long)ivalue);
                                        }));
                                    continue;
                                case MetaDataType.UInt64:
                                    ulong uvalue = (ulong)entry.Data;
                                    propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>(
                                        (Action<IfcPropertySingleValue>)(p =>
                                        {
                                            p.Name = (IfcIdentifier)key;
                                            p.NominalValue = (IfcValue)new IfcInteger((long)uvalue);
                                        }));
                                    continue;
                                case MetaDataType.Float:
                                    float fvalue = (float)entry.Data;
                                    propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>(
                                        (Action<IfcPropertySingleValue>)(p =>
                                        {
                                            p.Name = (IfcIdentifier)key;
                                            p.NominalValue = (IfcValue)new IfcLengthMeasure((double)fvalue);
                                        }));
                                    continue;
                                case MetaDataType.Double:
                                    double dvalue = (double)entry.Data;
                                    propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>(
                                        (Action<IfcPropertySingleValue>)(p =>
                                        {
                                            p.Name = (IfcIdentifier)key;
                                            p.NominalValue = (IfcValue)new IfcLengthMeasure(dvalue);
                                        }));
                                    continue;
                                case MetaDataType.String:
                                    string sdata = entry.Data as string;
                                    propertySingleValueArray[num3++] = model.Instances.New<IfcPropertySingleValue>(
                                        (Action<IfcPropertySingleValue>)(p =>
                                        {
                                            p.Name = (IfcIdentifier)key;
                                            p.NominalValue = (IfcValue)new IfcText(sdata);
                                        }));
                                    continue;
                                case MetaDataType.Vector3D:
                                    Vector3 data = (Vector3)entry.Data;
                                    continue;
                                default:
                                    continue;
                            }
                        }

                        pset.HasProperties.AddRange((IEnumerable<IfcProperty>)propertySingleValueArray);
                    }));
            }));
        }
    }
}
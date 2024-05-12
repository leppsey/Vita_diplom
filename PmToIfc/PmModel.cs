// Decompiled with JetBrains decompiler
// Type: PmToIfc.PmModel
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using Xbim.Ifc;

#nullable disable
namespace PmToIfc
{
  public class PmModel
  {
    public uint ID { get; set; }

    public ePmType PmType { get; set; }

    public int PmClass { get; set; }

    public string Name { get; set; }

    public Metadata Metadata { get; set; }

    public Matrix ModelMatrix { get; set; }

    public NodeType Type { get; set; }

    public IfcColor Color { get; set; }

    public List<PmModel> Children { get; set; }

    public List<int> Indices { get; set; }

    public List<Vector3> Points { get; set; }

    public List<Vector3> Normals { get; set; }

    public void TraverseDown(IfcStore model, eMode mode)
    {
      foreach (PmModel child in this.Children)
      {
        if (child.Children != null)
        {
          int count = child.Children.Count;
        }
        switch (child.Type)
        {
          case NodeType.Group:
          case NodeType.Model:
            child.TraverseDown(model, mode);
            continue;
          case NodeType.Mesh:
            Console.WriteLine("Item={0}", (object) child.Name);
            switch (mode)
            {
              case eMode.Ifc4:
                CreateIfc4.ModelToIfcElement(child, model);
                continue;
              case eMode.Ifc2x3:
                CreateIfc2x3.ModelToIfcElement(child, model);
                continue;
              default:
                continue;
            }
          default:
            continue;
        }
      }
    }
  }
}

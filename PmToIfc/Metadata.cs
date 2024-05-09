// Decompiled with JetBrains decompiler
// Type: PmToIfc.Metadata
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.Generic;

#nullable disable
namespace PmToIfc
{
  public class Metadata : Dictionary<string, Metadata.Entry>
  {
    public Metadata()
    {
    }

    public Metadata(int capacity)
      : base(capacity)
    {
    }

    public new struct Entry
    {
      public Entry(MetaDataType dataType, object data)
      {
        this.DataType = dataType;
        this.Data = data;
      }

      public MetaDataType DataType { get; }

      public object Data { get; }
    }
  }
}

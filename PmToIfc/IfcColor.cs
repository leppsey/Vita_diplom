// Decompiled with JetBrains decompiler
// Type: PmToIfc.IfcColor
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

#nullable disable
namespace PmToIfc
{
  public class IfcColor
  {
    public int R { get; set; }

    public int G { get; set; }

    public int B { get; set; }

    public int A { get; set; }

    public IfcColor()
    {
    }

    public IfcColor(int r, int g, int b, int a)
    {
      this.R = r;
      this.G = g;
      this.B = b;
      this.A = a;
    }

    public IfcColor(uint rgba)
    {
      this.A = (int) byte.MaxValue - ((int) (rgba >> 24) & (int) byte.MaxValue);
      this.B = (int) (rgba >> 16) & (int) byte.MaxValue;
      this.G = (int) (rgba >> 8) & (int) byte.MaxValue;
      this.R = (int) rgba & (int) byte.MaxValue;
    }
  }
}

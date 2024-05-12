// Decompiled with JetBrains decompiler
// Type: PmToIfc.Matrix
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

#nullable disable
namespace PmToIfc
{
  public struct Matrix
  {
    public static readonly int SizeInBytes;
    public static readonly Matrix Zero;
    public static readonly Matrix Identity;
    public float M44;
    public float M43;
    public float M42;
    public float M41;
    public float M34;
    public float M33;
    public float M32;
    public float M31;
    public float M23;
    public float M22;
    public float M21;
    public float M14;
    public float M13;
    public float M12;
    public float M11;
    public float M24;

    public Matrix(
      float M11,
      float M12,
      float M13,
      float M14,
      float M21,
      float M22,
      float M23,
      float M24,
      float M31,
      float M32,
      float M33,
      float M34,
      float M41,
      float M42,
      float M43,
      float M44)
    {
      this.M11 = M11;
      this.M11 = M11;
      this.M12 = M12;
      this.M13 = M13;
      this.M14 = M14;
      this.M21 = M21;
      this.M22 = M22;
      this.M23 = M23;
      this.M24 = M24;
      this.M31 = M31;
      this.M32 = M32;
      this.M33 = M33;
      this.M34 = M34;
      this.M41 = M41;
      this.M42 = M42;
      this.M43 = M43;
      this.M44 = M44;
    }

    public Vector3 Transform(Vector3 p)
    {
      double num1 = (double) p.X * (double) this.M11 + (double) p.Y * (double) this.M21 + (double) p.Z * (double) this.M31 + (double) this.M41;
      float num2 = (float) ((double) p.X * (double) this.M12 + (double) p.Y * (double) this.M22 + (double) p.Z * (double) this.M32) + this.M42;
      float num3 = (float) ((double) p.X * (double) this.M13 + (double) p.Y * (double) this.M23 + (double) p.Z * (double) this.M33) + this.M43;
      float num4 = (float) (1.0 / ((double) p.X * (double) this.M14 + (double) p.Y * (double) this.M24 + (double) p.Z * (double) this.M34 + (double) this.M44));
      double num5 = (double) num4;
      return new Vector3((float) (num1 * num5), num2 * num4, num3 * num4);
    }
  }
}

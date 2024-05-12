// Decompiled with JetBrains decompiler
// Type: PmToIfc.Import
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace PmToIfc
{
  public static class Import
  {
    public static PmModel Read(BinaryReader reader)
    {
      if (reader.BaseStream.Position == reader.BaseStream.Length)
        return (PmModel) null;
      reader.ReadInt32();
      return Import.ReadGroupNode(reader);
    }

    private static PmModel ReadGroupNode(BinaryReader reader)
    {
      int num = reader.ReadInt32();
      PmModel pmModel1 = new PmModel();
      pmModel1.Children = new List<PmModel>();
      pmModel1.Name = reader.ReadString();
      reader.ReadInt32();
      reader.ReadInt32();
      int mcount = reader.ReadInt32();
      pmModel1.Metadata = Import.ReadMetadata(reader, mcount);
      Matrix mx = Import.ReadModelMatrix(reader);
      pmModel1.ModelMatrix = mx;
      pmModel1.Type = NodeType.Group;
      for (int index = 0; index < num; ++index)
      {
        switch ((NodeType) reader.ReadInt32())
        {
          case NodeType.Group:
            PmModel pmModel2 = Import.ReadGroupNode(reader);
            pmModel1.Children.Add(pmModel2);
            break;
          case NodeType.Mesh:
            PmModel pmModel3 = Import.ReadMeshNode(reader, mx);
            pmModel3.Metadata = pmModel1.Metadata;
            pmModel3.ID = (uint) index;
            pmModel1.Children.Add(pmModel3);
            break;
          case NodeType.Line:
            PmModel pmModel4 = Import.ReadLineNode(reader);
            pmModel1.Children.Add(pmModel4);
            break;
          case NodeType.Point:
            PmModel pmModel5 = Import.ReadPointNode(reader);
            pmModel1.Children.Add(pmModel5);
            break;
          case NodeType.Model:
            PmModel pmModel6 = Import.ReadModelNode(reader);
            pmModel6.ID = (uint) index;
            pmModel1.Children.Add(pmModel6);
            break;
        }
      }
      return pmModel1;
    }

    private static PmModel ReadModelNode(BinaryReader reader)
    {
      int num1 = reader.ReadInt32();
      PmModel pmModel1 = new PmModel();
      pmModel1.Children = new List<PmModel>();
      pmModel1.Name = reader.ReadString();
      pmModel1.Type = NodeType.Model;
      int num2 = reader.ReadInt32();
      int num3 = reader.ReadInt32();
      pmModel1.PmType = (ePmType) num2;
      pmModel1.PmClass = num3;
      int mcount = reader.ReadInt32();
      pmModel1.Metadata = Import.ReadMetadata(reader, mcount);
      Matrix mx = Import.ReadModelMatrix(reader);
      pmModel1.ModelMatrix = mx;
      for (int index = 0; index < num1; ++index)
      {
        switch ((NodeType) reader.ReadInt32())
        {
          case NodeType.Group:
            PmModel pmModel2 = Import.ReadGroupNode(reader);
            pmModel1.Children.Add(pmModel2);
            break;
          case NodeType.Mesh:
            PmModel pmModel3 = Import.ReadMeshNode(reader, mx);
            pmModel3.Name = pmModel1.Name;
            pmModel3.PmType = pmModel1.PmType;
            pmModel3.Metadata = pmModel1.Metadata;
            pmModel1.Children.Add(pmModel3);
            break;
          case NodeType.Line:
            PmModel pmModel4 = Import.ReadLineNode(reader);
            pmModel1.Children.Add(pmModel4);
            break;
          case NodeType.Point:
            PmModel pmModel5 = Import.ReadPointNode(reader);
            pmModel1.Children.Add(pmModel5);
            break;
          case NodeType.Model:
            PmModel pmModel6 = Import.ReadModelNode(reader);
            pmModel1.Children.Add(pmModel6);
            break;
        }
      }
      return pmModel1;
    }

    private static PmModel ReadMeshNode(BinaryReader reader, Matrix mx)
    {
      PmModel pmModel = new PmModel();
      pmModel.Name = reader.ReadString();
      pmModel.Type = NodeType.Mesh;
      int capacity1 = reader.ReadInt32();
      int capacity2 = reader.ReadInt32();
      reader.ReadInt32();
      string name = pmModel.Name;
      if (!(name == "Model") && !(name == "Insulation"))
      {
        int num1 = name == "Operation" ? 1 : 0;
      }
      List<Vector3> vector3List1 = new List<Vector3>(capacity1);
      List<Vector3> vector3List2 = new List<Vector3>(capacity1);
      List<int> intList = new List<int>(capacity2);
      pmModel.Color = Import.ReadMeshMaterial(reader);
      pmModel.ModelMatrix = Import.ReadModelMatrix(reader);
      for (int index = 0; index < capacity1; ++index)
      {
        double x1 = (double) reader.ReadSingle();
        float num2 = reader.ReadSingle();
        float num3 = reader.ReadSingle();
        double y1 = (double) num2;
        double z1 = (double) num3;
        Vector3 p1 = new Vector3((float) x1, (float) y1, (float) z1);
        Vector3 vector3_1 = mx.Transform(p1);
        vector3List1.Add(vector3_1);
        double x2 = (double) reader.ReadSingle();
        float num4 = reader.ReadSingle();
        float num5 = reader.ReadSingle();
        double y2 = (double) num4;
        double z2 = (double) num5;
        Vector3 p2 = new Vector3((float) x2, (float) y2, (float) z2);
        Vector3 vector3_2 = mx.Transform(p2);
        vector3List2.Add(vector3_2);
      }
      for (int index = 0; index < capacity2; ++index)
        intList.Add(reader.ReadInt32());
      pmModel.Points = vector3List1;
      pmModel.Normals = vector3List2;
      pmModel.Indices = intList;
      return pmModel;
    }

    private static PmModel ReadLineNode(BinaryReader reader)
    {
      PmModel pmModel = new PmModel();
      pmModel.Name = reader.ReadString();
      pmModel.Type = NodeType.Line;
      string name = pmModel.Name;
      if (!(name == "Outlines"))
      {
        int num1 = name == "Centrelines" ? 1 : 0;
      }
      int capacity1 = reader.ReadInt32();
      int capacity2 = reader.ReadInt32();
      List<Vector3> vector3List = new List<Vector3>(capacity1);
      List<int> intList = new List<int>(capacity2);
      Import.ReadLineMaterial(reader);
      pmModel.ModelMatrix = Import.ReadModelMatrix(reader);
      for (int index = 0; index < capacity1; ++index)
      {
        double x = (double) reader.ReadSingle();
        float num2 = reader.ReadSingle();
        float num3 = reader.ReadSingle();
        double y = (double) num2;
        double z = (double) num3;
        Vector3 vector3 = new Vector3((float) x, (float) y, (float) z);
        vector3List.Add(vector3);
      }
      for (int index = 0; index < capacity2; ++index)
        intList.Add(reader.ReadInt32());
      pmModel.Points = vector3List;
      pmModel.Indices = intList;
      return pmModel;
    }

    private static PmModel ReadPointNode(BinaryReader reader)
    {
      PmModel pmModel = new PmModel();
      pmModel.Name = reader.ReadString();
      pmModel.Type = NodeType.Point;
      int capacity = reader.ReadInt32();
      List<Vector3> vector3List = new List<Vector3>(capacity);
      Import.ReadPointMaterial(reader);
      pmModel.ModelMatrix = Import.ReadModelMatrix(reader);
      for (int index = 0; index < capacity; ++index)
      {
        double x = (double) reader.ReadSingle();
        float num1 = reader.ReadSingle();
        float num2 = reader.ReadSingle();
        double num3 = (double) reader.ReadSingle();
        double y = (double) num1;
        double z = (double) num2;
        Vector3 vector3 = new Vector3((float) x, (float) y, (float) z);
      }
      pmModel.Points = vector3List;
      return pmModel;
    }

    private static IfcColor ReadMeshMaterial(BinaryReader reader)
    {
      float[] numArray = new float[8];
      for (int index = 0; index < 8; ++index)
        numArray[index] = reader.ReadSingle();
      return new IfcColor((uint) numArray[1]);
    }

    private static void ReadLineMaterial(BinaryReader reader)
    {
      float[] numArray = new float[8];
      for (int index = 0; index < 8; ++index)
        numArray[index] = reader.ReadSingle();
    }

    private static void ReadPointMaterial(BinaryReader reader)
    {
      float[] numArray = new float[8];
      for (int index = 0; index < 8; ++index)
        numArray[index] = reader.ReadSingle();
    }

    private static Metadata ReadMetadata(BinaryReader reader, int mcount)
    {
      Metadata metadata = new Metadata();
      if (mcount == 0)
        return metadata;
      for (int index = 0; index < mcount; ++index)
      {
        string key = reader.ReadString();
        switch (reader.ReadInt32())
        {
          case 0:
            bool data1 = reader.ReadBoolean();
            metadata.Add(key, new Metadata.Entry(MetaDataType.Bool, (object) data1));
            break;
          case 1:
            int data2 = reader.ReadInt32();
            metadata.Add(key, new Metadata.Entry(MetaDataType.Int32, (object) data2));
            break;
          case 2:
            ulong data3 = reader.ReadUInt64();
            metadata.Add(key, new Metadata.Entry(MetaDataType.UInt64, (object) data3));
            break;
          case 3:
            float data4 = reader.ReadSingle();
            metadata.Add(key, new Metadata.Entry(MetaDataType.Float, (object) data4));
            break;
          case 4:
            double data5 = reader.ReadDouble();
            metadata.Add(key, new Metadata.Entry(MetaDataType.Double, (object) data5));
            break;
          case 5:
            string data6 = reader.ReadString();
            metadata.Add(key, new Metadata.Entry(MetaDataType.String, (object) data6));
            break;
          case 6:
            double x = (double) reader.ReadSingle();
            float num1 = reader.ReadSingle();
            float num2 = reader.ReadSingle();
            double y = (double) num1;
            double z = (double) num2;
            Vector3 data7 = new Vector3((float) x, (float) y, (float) z);
            metadata.Add(key, new Metadata.Entry(MetaDataType.Vector3D, (object) data7));
            break;
        }
      }
      return metadata;
    }

    private static Matrix ReadModelMatrix(BinaryReader reader)
    {
      double M11 = (double) reader.ReadSingle();
      float num1 = reader.ReadSingle();
      float num2 = reader.ReadSingle();
      float num3 = reader.ReadSingle();
      float num4 = reader.ReadSingle();
      float num5 = reader.ReadSingle();
      float num6 = reader.ReadSingle();
      float num7 = reader.ReadSingle();
      float num8 = reader.ReadSingle();
      float num9 = reader.ReadSingle();
      float num10 = reader.ReadSingle();
      float num11 = reader.ReadSingle();
      float num12 = reader.ReadSingle();
      float num13 = reader.ReadSingle();
      float num14 = reader.ReadSingle();
      float num15 = reader.ReadSingle();
      double M12 = (double) num1;
      double M13 = (double) num2;
      double M14 = (double) num3;
      double M21 = (double) num4;
      double M22 = (double) num5;
      double M23 = (double) num6;
      double M24 = (double) num7;
      double M31 = (double) num8;
      double M32 = (double) num9;
      double M33 = (double) num10;
      double M34 = (double) num11;
      double M41 = (double) num12;
      double M42 = (double) num13;
      double M43 = (double) num14;
      double M44 = (double) num15;
      return new Matrix((float) M11, (float) M12, (float) M13, (float) M14, (float) M21, (float) M22, (float) M23, (float) M24, (float) M31, (float) M32, (float) M33, (float) M34, (float) M41, (float) M42, (float) M43, (float) M44);
    }
  }
}

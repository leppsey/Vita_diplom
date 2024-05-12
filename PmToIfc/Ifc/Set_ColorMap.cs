// Decompiled with JetBrains decompiler
// Type: PlantLinker.Set_ColorMap
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.ObjectModel;
using Xbim.Ifc;

#nullable disable
namespace PlantLinker
{
  internal class Set_ColorMap
  {
    public static XbimColourMap SetPColourMap()
    {
      XbimColourMap xbimColourMap = new XbimColourMap();
      ((Collection<XbimColour>) xbimColourMap).Clear();
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("Default", 0.98, 0.92, 0.74));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcWall", 0.98, 0.92, 0.74));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcWallStandardCase", 0.98, 0.92, 0.74));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcRoof", 0.28, 0.24, 0.55));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcBeam", 0.0, 0.0, 0.55));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcBuildingElementProxy", 0.95, 0.94, 0.74));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcColumn", 0.0, 0.0, 0.55));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcSlab", 0.47, 0.53, 0.6));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcWindow", 0.68, 0.85, 0.9, 0.5));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcCurtainWall", 0.68, 0.85, 0.9, 0.4));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcPlate", 0.68, 0.85, 0.9, 0.4));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcDoor", 0.97, 0.19, 0.0));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcSpace", 0.68, 0.85, 0.9, 0.4));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcMember", 0.34, 0.34, 0.34));
      ((Collection<XbimColour>) xbimColourMap).Add(new XbimColour("IfcGrid", 1.0, 0.9, 0.0));
      return xbimColourMap;
    }
  }
}

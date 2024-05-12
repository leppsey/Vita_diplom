// Decompiled with JetBrains decompiler
// Type: PmToIfc.IfcStyles4
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;

#nullable disable
namespace PmToIfc
{
  internal class IfcStyles4
  {
    public static Dictionary<ePmType, IfcPresentationStyleAssignment> Styles;

    public static void CreateStyles(IfcStore model)
    {
      IfcStyles4.Styles = new Dictionary<ePmType, IfcPresentationStyleAssignment>();
      using (ITransaction transaction = model.BeginTransaction(nameof (CreateStyles)))
      {
        Dictionary<ePmType, IfcColor> colors = Def.Colors;
        foreach (ePmType key in colors.Keys)
        {
          IfcColor ifcColor = colors[key];
          IfcColourRgb icolor = model.Instances.New<IfcColourRgb>();
          icolor.Red = (IfcNormalisedRatioMeasure) ((double) ifcColor.R / (double) byte.MaxValue);
          icolor.Green = (IfcNormalisedRatioMeasure) ((double) ifcColor.G / (double) byte.MaxValue);
          icolor.Blue = (IfcNormalisedRatioMeasure) ((double) ifcColor.B / (double) byte.MaxValue);
          IfcPresentationStyleAssignment faceSetStyle = IfcStyles4.CreateFaceSetStyle(model, icolor, (double) ifcColor.A);
          IfcStyles4.Styles.Add(key, faceSetStyle);
        }
        transaction.Commit();
      }
    }

    public static IfcPresentationStyleAssignment CreateFaceSetStyle(
      IfcStore model,
      IfcColourRgb icolor,
      double a = 0.0)
    {
      IfcSurfaceStyleRendering surfaceStyleRendering = model.Instances.New<IfcSurfaceStyleRendering>();
      surfaceStyleRendering.SurfaceColour = icolor;
      surfaceStyleRendering.Transparency = new IfcNormalisedRatioMeasure?((IfcNormalisedRatioMeasure) a);
      surfaceStyleRendering.SpecularColour = (IfcColourOrFactor) icolor;
      IfcSurfaceStyle ifcSurfaceStyle = model.Instances.New<IfcSurfaceStyle>();
      ifcSurfaceStyle.Side = IfcSurfaceSide.BOTH;
      ((ICollection<IfcSurfaceStyleElementSelect>) ifcSurfaceStyle.Styles).Add((IfcSurfaceStyleElementSelect) surfaceStyleRendering);
      IfcPresentationStyleAssignment faceSetStyle = model.Instances.New<IfcPresentationStyleAssignment>();
      ((ICollection<IfcPresentationStyleSelect>) faceSetStyle.Styles).Add((IfcPresentationStyleSelect) ifcSurfaceStyle);
      return faceSetStyle;
    }
  }
}

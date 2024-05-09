// Decompiled with JetBrains decompiler
// Type: PmToIfc.CreateIfc2x3
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationAppearanceResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.SharedBldgServiceElements;
using Xbim.Ifc2x3.TopologyResource;

#nullable disable
namespace PmToIfc
{
  internal class CreateIfc2x3
  {
    public static void ModelToIfcElement(PmModel dm, IfcStore model)
    {
      IfcSite site = DmToIfc.Site as IfcSite;
      IfcShapeRepresentation shapeRepresentation = CreateIfc2x3.ModelIfcShape(dm, model);
      IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>();
      ((ICollection<IfcRepresentation>) productDefinitionShape.Representations).Add((IfcRepresentation) shapeRepresentation);
      dm.ID.ToString();
      IfcProduct element;
      switch (dm.PmType)
      {
        case ePmType.Equipment:
        case ePmType.Shape:
        case ePmType.Nozzle:
          element = (IfcProduct) model.Instances.New<IfcEquipmentElement>();
          break;
        case ePmType.Member:
        case ePmType.Column:
        case ePmType.Beam:
          element = (IfcProduct) model.Instances.New<IfcMember>();
          break;
        case ePmType.Slab:
          element = (IfcProduct) model.Instances.New<IfcSlab>();
          break;
        case ePmType.Wall:
          element = (IfcProduct) model.Instances.New<IfcWall>();
          break;
        case ePmType.PipeComponent:
          element = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        case ePmType.CwayComponent:
          element = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        case ePmType.DuctComponent:
          element = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        default:
          element = (IfcProduct) model.Instances.New<IfcDistributionElement>();
          break;
      }
      element.Name = new IfcLabel?((IfcLabel) dm.Name);
      element.Description = new IfcText?((IfcText) "From Plantlinker");
      element.Representation = (IfcProductRepresentation) productDefinitionShape;
      element.ObjectPlacement = (IfcObjectPlacement) model.Instances.New<IfcLocalPlacement>();
      Metadata metadata = dm.Metadata;
      if (metadata != null && metadata.Count > 0)
        Properties2x3.Set(model, element, metadata);
      site.AddElement(element);
    }

    public static IfcShapeRepresentation ModelIfcShape(PmModel dm, IfcStore model)
    {
      IfcConnectedFaceSet connectedFaceSet = model.Instances.New<IfcConnectedFaceSet>();
      List<IfcCartesianPoint> points = new List<IfcCartesianPoint>();
      foreach (Vector3 point in dm.Points)
      {
        IfcCartesianPoint ifcCartesianPoint = model.Instances.New<IfcCartesianPoint>();
        double num1 = (double) point.X * 1000.0;
        double num2 = (double) point.Y * 1000.0;
        double num3 = (double) point.Z * 1000.0;
        ifcCartesianPoint.Coordinates.AddRange((IEnumerable<IfcLengthMeasure>) new IfcLengthMeasure[3]
        {
          (IfcLengthMeasure) num1,
          (IfcLengthMeasure) num2,
          (IfcLengthMeasure) num3
        });
        points.Add(ifcCartesianPoint);
      }
      foreach (int[] source in CreateIfc2x3.IndicesToList(dm.Indices))
      {
        IfcPolyLoop ifcPolyLoop = model.Instances.New<IfcPolyLoop>();
        ifcPolyLoop.Polygon.AddRange(((IEnumerable<int>) source).Select<int, IfcCartesianPoint>((Func<int, IfcCartesianPoint>) (k => points[k])));
        IfcFaceOuterBound ifcFaceOuterBound = model.Instances.New<IfcFaceOuterBound>();
        ifcFaceOuterBound.Bound = (IfcLoop) ifcPolyLoop;
        IfcFace ifcFace = model.Instances.New<IfcFace>();
        ((ICollection<IfcFaceBound>) ifcFace.Bounds).Add((IfcFaceBound) ifcFaceOuterBound);
        ((ICollection<IfcFace>) connectedFaceSet.CfsFaces).Add(ifcFace);
      }
      IfcFaceBasedSurfaceModel basedSurfaceModel = model.Instances.New<IfcFaceBasedSurfaceModel>();
      ((ICollection<IfcConnectedFaceSet>) basedSurfaceModel.FbsmFaces).Add(connectedFaceSet);
      IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>();
      shapeRepresentation.ContextOfItems = (IfcRepresentationContext) model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault<IfcGeometricRepresentationContext>();
      shapeRepresentation.RepresentationType = new IfcLabel?((IfcLabel) "Tessellation");
      shapeRepresentation.RepresentationIdentifier = new IfcLabel?((IfcLabel) "Body");
      ((ICollection<IfcRepresentationItem>) shapeRepresentation.Items).Add((IfcRepresentationItem) basedSurfaceModel);
      IfcPresentationStyleAssignment presentationStyleAssignment;
      if (Def.StyleMode)
      {
        ePmType key = dm.PmType;
        if (!IfcStyles2x3.Styles.ContainsKey(key))
          key = ePmType.Generic;
        presentationStyleAssignment = IfcStyles2x3.Styles[key];
      }
      else
      {
        IfcColor color = dm.Color;
        IfcColourRgb c = model.Instances.New<IfcColourRgb>();
        c.Red = (IfcNormalisedRatioMeasure) ((double) color.R / (double) byte.MaxValue);
        c.Green = (IfcNormalisedRatioMeasure) ((double) color.G / (double) byte.MaxValue);
        c.Blue = (IfcNormalisedRatioMeasure) ((double) color.B / (double) byte.MaxValue);
        double a = (double) color.A / (double) byte.MaxValue;
        presentationStyleAssignment = CreateIfc2x3.CreateFaceSetStyle(model, c, a);
      }
      IfcStyledItem ifcStyledItem = model.Instances.New<IfcStyledItem>();
      ifcStyledItem.Name = new IfcLabel?((IfcLabel) ("Style" + dm.PmType.ToString()));
      ifcStyledItem.Item = (IfcRepresentationItem) basedSurfaceModel;
      ((ICollection<IfcPresentationStyleAssignment>) ifcStyledItem.Styles).Add(presentationStyleAssignment);
      return shapeRepresentation;
    }

    public static IfcPresentationStyleAssignment CreateFaceSetStyle(
      IfcStore model,
      IfcColourRgb c,
      double a = 0.0)
    {
      IfcSurfaceStyleRendering surfaceStyleRendering = model.Instances.New<IfcSurfaceStyleRendering>();
      surfaceStyleRendering.SurfaceColour = c;
      surfaceStyleRendering.Transparency = new IfcNormalisedRatioMeasure?((IfcNormalisedRatioMeasure) a);
      surfaceStyleRendering.SpecularColour = (IfcColourOrFactor) c;
      IfcSurfaceStyle ifcSurfaceStyle = model.Instances.New<IfcSurfaceStyle>();
      ifcSurfaceStyle.Side = IfcSurfaceSide.BOTH;
      ((ICollection<IfcSurfaceStyleElementSelect>) ifcSurfaceStyle.Styles).Add((IfcSurfaceStyleElementSelect) surfaceStyleRendering);
      IfcPresentationStyleAssignment faceSetStyle = model.Instances.New<IfcPresentationStyleAssignment>();
      ((ICollection<IfcPresentationStyleSelect>) faceSetStyle.Styles).Add((IfcPresentationStyleSelect) ifcSurfaceStyle);
      return faceSetStyle;
    }

    public static List<int[]> IndicesToList(List<int> indeces)
    {
      int count = indeces.Count;
      List<int[]> list = new List<int[]>();
      for (int index = 0; index < count; index += 3)
      {
        int indece1 = indeces[index];
        int indece2 = indeces[index + 1];
        int indece3 = indeces[index + 2];
        list.Add(new int[3]{ indece1, indece2, indece3 });
      }
      return list;
    }
  }
}

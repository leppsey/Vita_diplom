// Decompiled with JetBrains decompiler
// Type: PmToIfc.CreateIfc4
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;
using Xbim.Ifc4.PresentationOrganizationResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.SharedBldgServiceElements;
using Xbim.Ifc4.UtilityResource;

#nullable disable
namespace PmToIfc
{
  internal class CreateIfc4
  {
    public static void ModelToIfcElement(PmModel dm, IfcStore model)
    {
      IfcSite site = DmToIfc.Site as IfcSite;
      string val = dm.ID.ToString();
      IfcProduct ifcProduct;
      switch (dm.PmType)
      {
        case ePmType.Equipment:
        case ePmType.Shape:
        case ePmType.Nozzle:
          ifcProduct = (IfcProduct) model.Instances.New<IfcDistributionElement>();
          break;
        case ePmType.Member:
        case ePmType.Column:
        case ePmType.Beam:
          ifcProduct = (IfcProduct) model.Instances.New<IfcMember>();
          break;
        case ePmType.Slab:
          ifcProduct = (IfcProduct) model.Instances.New<IfcSlab>();
          break;
        case ePmType.Wall:
          ifcProduct = (IfcProduct) model.Instances.New<IfcWall>();
          break;
        case ePmType.PipeComponent:
          ifcProduct = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        case ePmType.CwayComponent:
          ifcProduct = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        case ePmType.DuctComponent:
          ifcProduct = (IfcProduct) model.Instances.New<IfcFlowFitting>();
          break;
        default:
          ifcProduct = (IfcProduct) model.Instances.New<IfcDistributionElement>();
          break;
      }
      ifcProduct.Name = new IfcLabel?((IfcLabel) dm.Name);
      ifcProduct.Description = new IfcText?((IfcText) "From Plantlinker");
      ifcProduct.ObjectPlacement = (IfcObjectPlacement) model.Instances.New<IfcLocalPlacement>();
      ifcProduct.GlobalId = new IfcGloballyUniqueId(val);
      Metadata metadata = dm.Metadata;
      if (metadata != null && metadata.Count > 0)
        Properties4.Set(model, ifcProduct, metadata);
      IfcTriangulatedFaceSet faceSet = model.Instances.New<IfcTriangulatedFaceSet>();
      IfcCartesianPointList3D cps = model.Instances.New<IfcCartesianPointList3D>();
      IfcCartesianPointList3D coordinates = faceSet.Coordinates;
      CreateIfc4.FillInIndeces(faceSet, dm.Indices);
      CreateIfc4.FillInNormals(faceSet, dm.Normals);
      CreateIfc4.FillInPointsList(cps, dm.Points);
      faceSet.Coordinates = cps;
      IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>();
      IfcGeometricRepresentationContext representationContext = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault<IfcGeometricRepresentationContext>();
      shapeRepresentation.ContextOfItems = (IfcRepresentationContext) representationContext;
      shapeRepresentation.RepresentationType = new IfcLabel?((IfcLabel) "Tessellation");
      shapeRepresentation.RepresentationIdentifier = new IfcLabel?((IfcLabel) "Body");
      ((ICollection<IfcRepresentationItem>) shapeRepresentation.Items).Add((IfcRepresentationItem) faceSet);
      IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>();
      ((ICollection<IfcRepresentation>) productDefinitionShape.Representations).Add((IfcRepresentation) shapeRepresentation);
      ifcProduct.Representation = (IfcProductRepresentation) productDefinitionShape;
      IfcLocalPlacement ifcLocalPlacement = model.Instances.New<IfcLocalPlacement>();
      ifcProduct.ObjectPlacement = (IfcObjectPlacement) ifcLocalPlacement;
      IfcPresentationLayerAssignment presentationLayerAssignment = model.Instances.New<IfcPresentationLayerAssignment>();
      presentationLayerAssignment.Name = (IfcLabel) "";
      ((ICollection<IfcLayeredItem>) presentationLayerAssignment.AssignedItems).Add((IfcLayeredItem) shapeRepresentation);
      IfcPresentationStyleAssignment presentationStyleAssignment;
      if (Def.StyleMode)
      {
        ePmType key = dm.PmType;
        if (!IfcStyles2x3.Styles.ContainsKey(key))
          key = ePmType.Generic;
        presentationStyleAssignment = IfcStyles4.Styles[key];
      }
      else
      {
        IfcColor color = dm.Color;
        IfcColourRgb icolor = model.Instances.New<IfcColourRgb>();
        icolor.Red = (IfcNormalisedRatioMeasure) ((double) color.R / (double) byte.MaxValue);
        icolor.Green = (IfcNormalisedRatioMeasure) ((double) color.G / (double) byte.MaxValue);
        icolor.Blue = (IfcNormalisedRatioMeasure) ((double) color.B / (double) byte.MaxValue);
        double a = (double) color.A / (double) byte.MaxValue;
        presentationStyleAssignment = CreateIfc4.CreateFaceSetStyle(model, icolor, a);
      }
      IfcStyledItem ifcStyledItem = model.Instances.New<IfcStyledItem>();
      ifcStyledItem.Name = new IfcLabel?((IfcLabel) ("Style" + dm.PmType.ToString()));
      ifcStyledItem.Item = (IfcRepresentationItem) faceSet;
      ((ICollection<IfcStyleAssignmentSelect>) ifcStyledItem.Styles).Add((IfcStyleAssignmentSelect) presentationStyleAssignment);
      if (!((PersistEntity) site != (object) null))
        return;
      site.AddElement(ifcProduct);
    }

    public static int FillInIndeces(IfcTriangulatedFaceSet faceSet, List<int> indeces)
    {
      int count = indeces.Count;
      List<IfcPositiveInteger> ifcPositiveIntegerList = new List<IfcPositiveInteger>();
      int num = 0;
      for (int index = 0; index < count; index += 3)
      {
        ifcPositiveIntegerList.Add((IfcPositiveInteger) (long) (indeces[index] + 1));
        ifcPositiveIntegerList.Add((IfcPositiveInteger) (long) (indeces[index + 1] + 1));
        ifcPositiveIntegerList.Add((IfcPositiveInteger) (long) (indeces[index + 2] + 1));
        faceSet.CoordIndex.GetAt(num++).AddRange((IEnumerable<IfcPositiveInteger>) ifcPositiveIntegerList);
        ifcPositiveIntegerList.Clear();
      }
      return num;
    }

    public static int FillInNormals(IfcTriangulatedFaceSet faceSet, List<Vector3> normals)
    {
      int num = 0;
      foreach (Vector3 normal in normals)
        faceSet.Normals.GetAt(num++).AddRange((IEnumerable<IfcParameterValue>) new List<IfcParameterValue>()
        {
          (IfcParameterValue) (double) normal.X,
          (IfcParameterValue) (double) normal.Y,
          (IfcParameterValue) (double) normal.Z
        });
      return num;
    }

    public static int FillInPointsList(IfcCartesianPointList3D cps, List<Vector3> points)
    {
      int num1 = 0;
      foreach (Vector3 point in points)
      {
        double num2 = (double) point.X * 1000.0;
        double num3 = (double) point.Y * 1000.0;
        double num4 = (double) point.Z * 1000.0;
        cps.CoordList.GetAt(num1++).AddRange((IEnumerable<IfcLengthMeasure>) new IfcLengthMeasure[3]
        {
          (IfcLengthMeasure) num2,
          (IfcLengthMeasure) num3,
          (IfcLengthMeasure) num4
        });
      }
      return num1;
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

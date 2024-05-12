// Decompiled with JetBrains decompiler
// Type: PmToIfc.InitIFCProject4
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.IO;

#nullable disable
namespace PmToIfc
{
  internal class InitIFCProject4
  {
    public static IfcStore CreateModel(string projectName)
    {
      IfcStore model = IfcStore.Create(new XbimEditorCredentials()
      {
        ApplicationDevelopersName = "Plantlinker",
        ApplicationFullName = "Plantlinker",
        ApplicationIdentifier = "PmToIFC.exe",
        ApplicationVersion = "2.0",
        EditorsFamilyName = "Plantlinker",
        EditorsGivenName = "Pm",
        EditorsOrganisationName = "Plantlinker"
      }, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
      using (ITransaction transaction = model.BeginTransaction(nameof (CreateModel)))
      {
        IfcProject ifcProject = model.Instances.New<IfcProject>();
        ifcProject.Initialize(ProjectUnits.SIUnitsUK);
        ifcProject.Name = new IfcLabel?((IfcLabel) projectName);
        transaction.Commit();
      }
      return model;
    }

    public static IfcBuilding CreateBuilding(IfcStore model, string name)
    {
      using (ITransaction transaction = model.BeginTransaction(nameof (CreateBuilding)))
      {
        IfcBuilding building = model.Instances.New<IfcBuilding>();
        building.Name = new IfcLabel?((IfcLabel) name);
        building.CompositionType = new IfcElementCompositionEnum?(IfcElementCompositionEnum.ELEMENT);
        IfcLocalPlacement ifcLocalPlacement = model.Instances.New<IfcLocalPlacement>();
        building.ObjectPlacement = (IfcObjectPlacement) ifcLocalPlacement;
        IfcAxis2Placement3D axis2Placement3D = model.Instances.New<IfcAxis2Placement3D>();
        ifcLocalPlacement.RelativePlacement = (IfcAxis2Placement) axis2Placement3D;
        axis2Placement3D.Location = model.Instances.New<IfcCartesianPoint>((Action<IfcCartesianPoint>) (p => p.SetXYZ(0.0, 0.0, 0.0)));
        IfcProject ifcProject = model.Instances.OfType<IfcProject>().FirstOrDefault<IfcProject>();
        if ((PersistEntity) ifcProject != (object) null)
          ifcProject.AddBuilding(building);
        transaction.Commit();
        return building;
      }
    }

    public static IfcSite CreateSite(IfcStore model, string name)
    {
      using (ITransaction transaction = model.BeginTransaction("tr"))
      {
        IfcSite site = model.Instances.New<IfcSite>();
        site.Name = new IfcLabel?((IfcLabel) name);
        site.CompositionType = new IfcElementCompositionEnum?(IfcElementCompositionEnum.ELEMENT);
        IfcLocalPlacement ifcLocalPlacement = model.Instances.New<IfcLocalPlacement>();
        site.ObjectPlacement = (IfcObjectPlacement) ifcLocalPlacement;
        IfcAxis2Placement3D axis2Placement3D = model.Instances.New<IfcAxis2Placement3D>();
        ifcLocalPlacement.RelativePlacement = (IfcAxis2Placement) axis2Placement3D;
        axis2Placement3D.Location = model.Instances.New<IfcCartesianPoint>((Action<IfcCartesianPoint>) (p => p.SetXYZ(0.0, 0.0, 0.0)));
        IfcProject ifcProject = model.Instances.OfType<IfcProject>().FirstOrDefault<IfcProject>();
        if ((PersistEntity) ifcProject != (object) null)
          ifcProject.AddSite(site);
        transaction.Commit();
        return site;
      }
    }

    public static IfcBuildingStorey CreateStorey(
      IfcBuilding building,
      string name,
      double elv,
      string desc = "")
    {
      IModel model = building.Model;
      using (ITransaction transaction = model.BeginTransaction(nameof (CreateStorey)))
      {
        IfcBuildingStorey child = model.Instances.New<IfcBuildingStorey>();
        child.Name = new IfcLabel?((IfcLabel) name);
        child.Elevation = new IfcLengthMeasure?((IfcLengthMeasure) elv);
        child.Description = new IfcText?((IfcText) desc);
        building.AddToSpatialDecomposition((IfcSpatialStructureElement) child);
        transaction.Commit();
        return child;
      }
    }
  }
}

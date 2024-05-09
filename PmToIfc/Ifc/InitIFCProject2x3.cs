// Decompiled with JetBrains decompiler
// Type: PmToIfc.InitIFCProject2x3
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

using System;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO;

#nullable disable
namespace PmToIfc
{
  internal class InitIFCProject2x3
  {
    public static IfcStore CreateModel(string projectName)
    {
      IfcStore model = IfcStore.Create(new XbimEditorCredentials()
      {
        ApplicationDevelopersName = "Plantlinker.com",
        ApplicationFullName = "Plantlinker",
        ApplicationIdentifier = "PmToIFC.exe",
        ApplicationVersion = "1.0",
        EditorsFamilyName = "Team",
        EditorsGivenName = "",
        EditorsOrganisationName = "Plantlinker.com"
      }, XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel);
      using (ITransaction transaction = model.BeginTransaction("tr"))
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
      using (ITransaction transaction = model.BeginTransaction("tr"))
      {
        IfcBuilding building = model.Instances.New<IfcBuilding>();
        building.Name = new IfcLabel?((IfcLabel) name);
        building.CompositionType = IfcElementCompositionEnum.ELEMENT;
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
        site.CompositionType = IfcElementCompositionEnum.ELEMENT;
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
  }
}

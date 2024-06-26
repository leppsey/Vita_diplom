using System;
using System.Collections.Generic;
using System.Linq;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;

namespace Isomerization.Domain;

public class DatabaseInitializer
{
    public static void Init(IsomerizationContext context)
    {
        #region UserRoles

        var researcherRole = new UserRole()
        {
            Name = UserRoles.Researcher,
        };        
        context.Add(researcherRole);
        
        var adminRole = new UserRole()
        {
            Name = UserRoles.Admin,
        };
        context.Add(adminRole);
        
        #endregion

        #region Users

        var defaultUsers = new List<User>()
        {
            new()
            {
                UserRole = researcherRole,
                Login = "r",
                Password = "r",
                Name = "Исследователь",
            },
            new()
            {
                UserRole = adminRole,
                Login = "a",
                Password = "a",
                Name = "Администратор",
            },
        };
        context.AddRange(defaultUsers);
        
        #endregion

        #region Catalysts

        var defaultCatalysts = new List<Catalyst>()
        {
            new()
            {
                Type = "Гетерогенный",
                Name = "Pt/Al2O3 (Платина на алюмосиликате)",
                Activity = 110,
                LoadingRate = 0.5,
                OperatingTime = 5000,
                ServiceLife = 5,
                TemperatureReaction = 220,
                DateOfCommissioning = new DateTime(year: 2022, day: 22, month: 06),
                DateOfDecommissioning = new DateTime(year: 2027, day: 22, month: 06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pt/Zeolite (Платина на цеолите)",
                Activity = 136,
                LoadingRate = 0.4,
                OperatingTime = 5500,
                ServiceLife = 5,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year: 2021, day: 07, month: 06),
                DateOfDecommissioning = new DateTime(year: 2026, day: 07, month: 06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pd/C (Палладий на углероде)",
                Activity = 100,
                LoadingRate = 0.3,
                OperatingTime = 6000,
                ServiceLife = 6,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year: 2023, day: 30, month: 05),
                DateOfDecommissioning = new DateTime(year: 2029, day: 30, month: 05)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "mordenite-based (На основе морденита)",
                Activity = 120,
                LoadingRate = 0.45,
                OperatingTime = 5000,
                ServiceLife = 5,
                TemperatureReaction = 180,
                DateOfCommissioning = new DateTime(year: 2020, day: 01, month: 02),
                DateOfDecommissioning = new DateTime(year: 2025, day: 01, month: 02)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "ZSM-5",
                Activity = 115,
                LoadingRate = 0.4,
                OperatingTime = 2400,
                ServiceLife = 10,
                TemperatureReaction = 300,
                DateOfCommissioning = new DateTime(year: 2021, day: 28, month: 02),
                DateOfDecommissioning = new DateTime(year: 2031, day: 28, month: 02)
            },
            new()
            {
                Type = "Гомогенный",
                Name = "Катализатор на основе комплексов рутения",
                Activity = 130,
                LoadingRate = 0.25,
                OperatingTime = 3900,
                ServiceLife = 3,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year: 2022, day: 30, month: 08),
                DateOfDecommissioning = new DateTime(year: 2025, day: 30, month: 08)
            },
        };
        context.AddRange(defaultCatalysts);
        #endregion

        #region Installations

        var defaultInstallations = new List<Installation>()
        {
            new()
            {
                Type = "Реакторный блок",
                Name = "ЛК-2Б",
                Diameter = 5,
                Height = 8,
                Length = 12,
                Performance = 10000,
                Pressure = 354,
                Status = "в работе",
                Temperature = 654,
                Volume = 351,
                EnergyConsumption = 1000,
                Width = 98,
                RawMaterialConsumption = 9.1,
                DateOfCommissioning = DateTime.Now.AddYears(-2),
                DateOfPlannedWorks = DateTime.Now.AddYears(1),
                Model = new Model()
                {
                    ObjPath =    "resources/1/1.obj",
                    AlbedoPath = "resources/1/1_albedo.png",
                    HeightPath = "resources/1/1_height.png",
                    NormalPath = "resources/1/1_normal.png",
                    RMPath =     "resources/1/1_RM.png",
                },
            },
            new()
            {
                Type = "Реакторный блок",
                Name = "ПГИ-434",
                Diameter = 4,
                Height = 2,
                Length = 5,
                Performance = 9000,
                Pressure = 865,
                Status = "В работе",
                Temperature = 25,
                Volume = 56,
                EnergyConsumption = 850,
                Width = 31,
                RawMaterialConsumption = 8.7,
                DateOfCommissioning = DateTime.Now.AddYears(-3),
                DateOfPlannedWorks = DateTime.Now.AddYears(2),
                Model = new Model()
                {
                    ObjPath =    "resources/2/2.obj",
                    AlbedoPath = "resources/2/2_albedo.png",
                    HeightPath = "resources/2/2_height.png",
                    NormalPath = "resources/2/2_normal.png",
                    RMPath =     "resources/2/2_RM.png",
                },
            },
            new()
            {
                Type = "Реакторный блок",
                Name = "РХ-187",
                Diameter = 4,
                Height = 2,
                Length = 5,
                Performance = 7500,
                Pressure = 865,
                Status = "В работе",
                Temperature = 25,
                Volume = 56,
                EnergyConsumption = 800,
                Width = 31,
                RawMaterialConsumption = 8.7,
                DateOfCommissioning = DateTime.Now.AddYears(-3),
                DateOfPlannedWorks = DateTime.Now.AddYears(2),
                Model = new Model()
                {
                    ObjPath =    "resources/3/3.obj",
                    AlbedoPath = "resources/3/3_albedo.png",
                    HeightPath = "resources/3/3_height.png",
                    NormalPath = "resources/3/3_normal.png",
                    RMPath =     "resources/3/3_RM.png",
                },
            },
        };
        context.AddRange(defaultInstallations);
        
        #endregion

        #region RawMaterial

        var defaultRawMaterial = new List<RawMaterial>()
        {
            new()
            {
               Name="Стабильный гидрогенизат-1",
               Compound=13000,
               Density=0.650,
               HeatCapacity=2100,
               OctaneRating=80,
            },
            
            new()
            {
               Name="Стабильный гидрогенизат-2",
               Compound=20000,
               Density=0.660,
               HeatCapacity=2150,
               OctaneRating=84,
            },
            new()
            {
               Name="Стабильный гидрогенизат-3",
               Compound=18000,
               Density=0.660,
               HeatCapacity=2000,
               OctaneRating=86,
            },
        };
        context.AddRange(defaultRawMaterial);
        context.SaveChanges();
        #endregion

        #region Kinetic

        var defaultKinetic = new List<Kinetic>()
        {
            new()
            {
                PreExponentialFactor=10000,
                EnergyActivation=100,
                ReactionRateConstant = 0.5,
                StehiometricFactor = 12,
                RawMaterial = defaultRawMaterial.First(),
            },
            
        };
        context.AddRange(defaultKinetic);
        
        #endregion

        var concentrations = new List<Concentration>()
        {
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 0,
                Value = 53.39m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 1,
                Value = 15.39m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 2,
                Value = 6.92m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 3,
                Value = 11.8m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 4,
                Value = 7.52m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 5,
                Value = 3.06m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[0].RawMaterialId,
                Order = 6,
                Value = 1.92m,
            },
            
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 0,
                Value = 58.39m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 1,
                Value = 12.39m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 2,
                Value = 6.92m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 3,
                Value = 9.8m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 4,
                Value = 6.52m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 5,
                Value = 3.06m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[1].RawMaterialId,
                Order = 6,
                Value = 2.92m,
            },
            
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 0,
                Value = 51.2m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 1,
                Value = 20.1m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 2,
                Value = 8.5m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 3,
                Value = 10.98m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 4,
                Value = 4.2m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 5,
                Value = 3.1m,
            },
            new()
            {
                RawMaterialId = defaultRawMaterial[2].RawMaterialId,
                Order = 6,
                Value = 1.92m,
            },
        };
        context.AddRange(concentrations);
        context.SaveChanges();
    }
}

using System;
using System.Collections.Generic;
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
                Activity = 80,
                LoadingRate = 0.5,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 220,
                DateOfCommissioning = new DateTime(year: 2022, day: 22, month: 06),
                DateOfDecommissioning = new DateTime(year: 2027, day: 22, month: 06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pt/Zeolite (Платина на цеолите)",
                Activity = 90,
                LoadingRate = 0.4,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year: 2021, day: 07, month: 06),
                DateOfDecommissioning = new DateTime(year: 2026, day: 07, month: 06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pd/C (Палладий на углероде)",
                Activity = 80,
                LoadingRate = 0.3,
                // OperatingTime = 1234,
                ServiceLife = 6,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year: 2023, day: 30, month: 05),
                DateOfDecommissioning = new DateTime(year: 2029, day: 30, month: 05)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "mordenite-based (На основе морденита)",
                Activity = 80,
                LoadingRate = 0.45,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 180,
                DateOfCommissioning = new DateTime(year: 2020, day: 01, month: 02),
                DateOfDecommissioning = new DateTime(year: 2025, day: 01, month: 02)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "ZSM-5",
                Activity = 90,
                LoadingRate = 0.4,
                // OperatingTime = 1234,
                ServiceLife = 10,
                TemperatureReaction = 300,
                DateOfCommissioning = new DateTime(year: 2021, day: 28, month: 02),
                DateOfDecommissioning = new DateTime(year: 2031, day: 28, month: 02)
            },
            new()
            {
                Type = "Гомогенный",
                Name = "Катализатор на основе комплексов рутения",
                Activity = 90,
                LoadingRate = 0.25,
                // OperatingTime = 1234,
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
                Type = "Тип установки 1",
                Name = "Название установки 1",
                Diameter = 5,
                Height = 8,
                Length = 12,
                Performance = 123,
                Pressure = 354,
                Status = "Статус установки 1",
                Temperature = 654,
                Volume = 351,
                EnergyConsumption = 743,
                Width = 98,
                DateOfCommissioning = DateTime.Now.AddYears(-2),
                DateOfPlannedWorks = DateTime.Now.AddYears(1),
            },
            new()
            {
                Type = "Тип установки 2",
                Name = "Название установки 2",
                Diameter = 4,
                Height = 2,
                Length = 5,
                Performance = 531,
                Pressure = 865,
                Status = "Статус установки 2",
                Temperature = 25,
                Volume = 56,
                EnergyConsumption = 92,
                Width = 31,
                DateOfCommissioning = DateTime.Now.AddYears(-3),
                DateOfPlannedWorks = DateTime.Now.AddYears(2),
            },
        };
        context.AddRange(defaultInstallations);
        
        #endregion

        context.SaveChanges();
    }
}
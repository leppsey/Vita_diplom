using System;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Domain.Data;

/// <summary>Добавление столбцов к существующей SQLite БД (без dotnet-ef на машине разработчика).</summary>
public static class SqliteSchemaUpgrade
{
    public static void Apply(IsomerizationContext db)
    {
        foreach (var dimTable in new[] { "DIMIsomerizations", "DimIsomerizations" })
        {
            AddColumn(db, dimTable, "Productivity", "REAL NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "ProcessEnergyConsumption", "REAL NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "OctaneNumber", "REAL NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsopentaneConcentration", "REAL NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsomerizationDegree", "REAL NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsProductivityValid", "INTEGER NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsEnergyValid", "INTEGER NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsOctaneValid", "INTEGER NOT NULL DEFAULT 0");
            AddColumn(db, dimTable, "IsIsopentaneValid", "INTEGER NOT NULL DEFAULT 0");
        }

        AddColumn(db, "Pipelines", "PressureLossLinear", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "PressureLossLocal", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "PressureLossTotal", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "PumpPower", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "PipelineEnergyConsumption", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "AllowablePressure", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "CalculatedWallThickness", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "ActualWallThickness", "REAL NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "IsPressureValid", "INTEGER NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "IsWallThicknessValid", "INTEGER NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "IsEnergyValid", "INTEGER NOT NULL DEFAULT 0");
        AddColumn(db, "Pipelines", "IsNormativeValid", "INTEGER NOT NULL DEFAULT 0");
    }

    private static void AddColumn(IsomerizationContext db, string table, string column, string definition)
    {
        try
        {
            db.Database.ExecuteSqlRaw($"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}");
        }
        catch (Exception)
        {
            // столбец уже есть или таблицы ещё нет
        }
    }
}

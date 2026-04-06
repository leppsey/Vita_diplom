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

        CreateTableIfNotExists(db, "PipelinePipes", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelinePipes"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""DN"" INTEGER NOT NULL,
            ""Material"" TEXT NOT NULL,
            ""Roughness"" REAL NOT NULL,
            ""PressureClass"" TEXT NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""WallThickness"" REAL NOT NULL,
            ""Standard"" TEXT NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineElbows", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineElbows"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""DN"" INTEGER NOT NULL,
            ""Angle"" REAL NOT NULL,
            ""Zeta"" REAL NOT NULL,
            ""PressureClass"" TEXT NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""Standard"" TEXT NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineReducers", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineReducers"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""DNIn"" INTEGER NOT NULL,
            ""DNOut"" INTEGER NOT NULL,
            ""Zeta"" REAL NOT NULL,
            ""PressureClass"" TEXT NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""Standard"" TEXT NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineValves", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineValves"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""Type"" TEXT NOT NULL,
            ""DN"" INTEGER NOT NULL,
            ""Zeta"" REAL NOT NULL,
            ""PressureClass"" TEXT NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""Standard"" TEXT NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelinePumps", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelinePumps"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""DN"" INTEGER NOT NULL,
            ""PressureIncrease"" REAL NOT NULL,
            ""Power"" REAL NOT NULL,
            ""Efficiency"" REAL NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineFilters", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineFilters"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""DN"" INTEGER NOT NULL,
            ""Zeta"" REAL NOT NULL,
            ""PressureClass"" TEXT NOT NULL,
            ""TemperatureMin"" REAL NOT NULL,
            ""TemperatureMax"" REAL NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineTemplates", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineTemplates"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""LineType"" TEXT NOT NULL,
            ""HasPump"" INTEGER NOT NULL,
            ""HasReducer"" INTEGER NOT NULL,
            ""HasElbow"" INTEGER NOT NULL,
            ""HasValve"" INTEGER NOT NULL,
            ""HasFilter"" INTEGER NOT NULL,
            ""SupportedDN"" TEXT NOT NULL,
            ""Template3DPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "Pipeline3DTemplates", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Pipeline3DTemplates"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""LineType"" TEXT NOT NULL,
            ""SupportedDN"" TEXT NOT NULL,
            ""RequiredElements"" TEXT NOT NULL,
            ""PreviewPath"" TEXT NOT NULL,
            ""ModelPath"" TEXT NOT NULL
        ");

        CreateTableIfNotExists(db, "PipelineRules", @"
            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_PipelineRules"" PRIMARY KEY AUTOINCREMENT,
            ""Name"" TEXT NOT NULL,
            ""ConditionType"" TEXT NOT NULL,
            ""ConditionOperator"" TEXT NOT NULL,
            ""ConditionValue"" TEXT NOT NULL,
            ""ActionType"" TEXT NOT NULL,
            ""ActionValue"" TEXT NOT NULL,
            ""Priority"" INTEGER NOT NULL,
            ""IsEnabled"" INTEGER NOT NULL DEFAULT 1
        ");

        EnsureIndex(db, "IX_PipelinePipes_DN", @"CREATE INDEX IF NOT EXISTS ""IX_PipelinePipes_DN"" ON ""PipelinePipes"" (""DN"")");
        EnsureIndex(db, "IX_PipelinePipes_PressureClass", @"CREATE INDEX IF NOT EXISTS ""IX_PipelinePipes_PressureClass"" ON ""PipelinePipes"" (""PressureClass"")");
        EnsureIndex(db, "IX_PipelineElbows_DN", @"CREATE INDEX IF NOT EXISTS ""IX_PipelineElbows_DN"" ON ""PipelineElbows"" (""DN"")");
        EnsureIndex(db, "IX_PipelineValves_DN", @"CREATE INDEX IF NOT EXISTS ""IX_PipelineValves_DN"" ON ""PipelineValves"" (""DN"")");
        EnsureIndex(db, "IX_PipelineTemplates_LineType", @"CREATE INDEX IF NOT EXISTS ""IX_PipelineTemplates_LineType"" ON ""PipelineTemplates"" (""LineType"")");
        EnsureIndex(db, "IX_Pipeline3DTemplates_LineType", @"CREATE INDEX IF NOT EXISTS ""IX_Pipeline3DTemplates_LineType"" ON ""Pipeline3DTemplates"" (""LineType"")");
        EnsureIndex(db, "IX_PipelineRules_Priority", @"CREATE INDEX IF NOT EXISTS ""IX_PipelineRules_Priority"" ON ""PipelineRules"" (""Priority"")");
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

    private static void CreateTableIfNotExists(IsomerizationContext db, string tableName, string columnsDefinition)
    {
        try
        {
            db.Database.ExecuteSqlRaw($"CREATE TABLE IF NOT EXISTS \"{tableName}\" ({columnsDefinition})");
        }
        catch (Exception)
        {
            // таблица уже существует или БД не готова
        }
    }

    private static void EnsureIndex(IsomerizationContext db, string indexName, string query)
    {
        try
        {
            db.Database.ExecuteSqlRaw(query);
        }
        catch (Exception)
        {
            // индекс уже существует
        }
    }
}

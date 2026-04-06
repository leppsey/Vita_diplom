using System;
using System.IO;
using Isomerization.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;


namespace Isomerization.Domain.Data
{
    /// <summary>
    /// Класс для взаимодействия с базой данных
    /// </summary>
    public class IsomerizationContext : DbContext
    {
        public static string DbPath => Path.Combine(AppContext.BaseDirectory, "Membrane.db");

        public IsomerizationContext()
        {
            // Database.EnsureCreated();
            // Catalysts.Load();
            // DimIsomerizations.Load();
            // Installations.Load();
            // Kinetics.Load();
            // Pipelines.Load();
            // RawMaterials.Load();
            // Users.Load();
            // UserRoles.Load();
        }

        public IsomerizationContext(DbContextOptions<IsomerizationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Catalyst> Catalysts { get; set; }
        public virtual DbSet<DIMIsomerization> DimIsomerizations { get; set; }
        public virtual DbSet<Installation> Installations { get; set; }
        public virtual DbSet<Kinetic> Kinetics { get; set; }
        public virtual DbSet<Pipeline> Pipelines { get; set; }
        public virtual DbSet<RawMaterial> RawMaterials { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<PipelinePipe> PipelinePipes { get; set; }
        public virtual DbSet<PipelineElbow> PipelineElbows { get; set; }
        public virtual DbSet<PipelineReducer> PipelineReducers { get; set; }
        public virtual DbSet<PipelineValve> PipelineValves { get; set; }
        public virtual DbSet<PipelinePump> PipelinePumps { get; set; }
        public virtual DbSet<PipelineFilter> PipelineFilters { get; set; }
        public virtual DbSet<PipelineTemplate> PipelineTemplates { get; set; }
        public virtual DbSet<Pipeline3DTemplate> Pipeline3DTemplates { get; set; }
        public virtual DbSet<PipelineRule> PipelineRules { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
            }
        }
        
    }
}
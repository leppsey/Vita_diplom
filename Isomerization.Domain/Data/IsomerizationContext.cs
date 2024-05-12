using System;
using Isomerization.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Isomerization.Domain.Data
{
    /// <summary>
    /// Класс для взаимодействия с базой данных
    /// </summary>
    public class IsomerizationContext : DbContext
    {
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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=Membrane.db");
            }
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            //throw new NotImplementedException();
        }
    }
}
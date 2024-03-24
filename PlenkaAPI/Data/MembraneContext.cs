using System;

using Microsoft.EntityFrameworkCore;

using PlenkaAPI.Models;


namespace PlenkaAPI.Data
{
    public class MembraneContext : DbContext
    {
        public MembraneContext()
        {
            Catalysts.Load();
            DimIsomerizations.Load();
            Installations.Load();
            Kinetics.Load();
            Pipelines.Load();
            RawMaterials.Load();
            Users.Load();
        }

        public MembraneContext(DbContextOptions<MembraneContext> options)
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
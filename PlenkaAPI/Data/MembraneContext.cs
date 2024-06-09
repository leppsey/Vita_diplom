using System;

using Microsoft.EntityFrameworkCore;

using PlenkaAPI.Models;


namespace PlenkaAPI.Data
{
    public class MembraneContext : DbContext
    {
        public MembraneContext()
        {
            Kinetics.Load();
            RawMaterials.Load();
            DIMIsomerizations.Load();
            Users.Load();
            Catalysts.Load();
            Installations.Load();
            Pipelines.Load();
        }

        public MembraneContext(DbContextOptions<MembraneContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Kinetic> Kinetics { get; set; }
        public virtual DbSet<RawMaterial> RawMaterials { get; set; }
        public virtual DbSet<DIMIsomerization> DIMIsomerizations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Catalyst> Catalysts { get; set; }
        public virtual DbSet<Installation> Installations { get; set; }
        public virtual DbSet<Pipeline> Pipelines { get; set; }

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

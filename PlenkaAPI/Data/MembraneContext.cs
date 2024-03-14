using System;

using Microsoft.EntityFrameworkCore;

using PlenkaAPI.Models;


namespace PlenkaAPI.Data
{
    public class MembraneContext : DbContext
    {
        public MembraneContext()
        {
            MembraneObjects.Load();
            Properties.Load();
            Users.Load();
            UserTypes.Load();
            Values.Load();
            Units.Load();
            ObjectTypes.Load();
            DefaultProperties.Load();
        }

        public MembraneContext(DbContextOptions<MembraneContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DefaultProperty> DefaultProperties { get; set; }
        public virtual DbSet<MembraneObject> MembraneObjects { get; set; }
        public virtual DbSet<ObjectType> ObjectTypes { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Value> Values { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("DataSource=Membrane.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefaultProperty>(entity =>
            {
                entity.HasKey(e => e.DfId);

                entity.ToTable("default_properties");

                entity.HasIndex(e => e.DfId, "IX_default_properties_df_id")
                      .IsUnique();

                entity.Property(e => e.DfId).HasColumnName("df_id");

                entity.Property(e => e.PropId).HasColumnName("prop_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Prop)
                      .WithMany(p => p.DefaultProperties)
                      .HasForeignKey(d => d.PropId);

                entity.HasOne(d => d.Type)
                      .WithMany(p => p.DefaultProperties)
                      .HasForeignKey(d => d.TypeId);
            });

            modelBuilder.Entity<MembraneObject>(entity =>
            {
                entity.HasKey(e => e.ObId);

                entity.ToTable("membrane_object");

                entity.HasIndex(e => e.ObId, "IX_membrane_object_ob_id")
                      .IsUnique();

                entity.HasIndex(e => e.ObName, "IX_membrane_object_ob_name")
                      .IsUnique();

                entity.Property(e => e.ObId).HasColumnName("ob_id");

                entity.Property(e => e.ObName)
                      .IsRequired()
                      .HasColumnName("ob_name");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Type)
                      .WithMany(p => p.MembraneObjects)
                      .HasForeignKey(d => d.TypeId);
            });

            modelBuilder.Entity<ObjectType>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("object_type");

                entity.HasIndex(e => e.TypeId, "IX_object_type_type_id")
                      .IsUnique();

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.TypeName)
                      .IsRequired()
                      .HasColumnName("type_name");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.ProperrtyId);

                entity.ToTable("property");

                entity.HasIndex(e => e.ProperrtyId, "IX_property_properrty_id")
                      .IsUnique();

                entity.Property(e => e.ProperrtyId).HasColumnName("properrty_id");

                entity.Property(e => e.PropertyName)
                      .IsRequired()
                      .HasColumnName("property_name");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Unit)
                      .WithMany(p => p.Properties)
                      .HasForeignKey(d => d.UnitId);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("unit");

                entity.HasIndex(e => e.UnitId, "IX_unit_unit_id")
                      .IsUnique();

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.Property(e => e.UnitName)
                      .IsRequired()
                      .HasColumnName("unit_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.UserId, "IX_user_user_id")
                      .IsUnique();

                entity.HasIndex(e => e.UserName, "IX_user_user_name")
                      .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasColumnName("user_name");

                entity.Property(e => e.UserPassword)
                      .IsRequired()
                      .HasColumnName("user_password");

                entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");

                entity.HasOne(d => d.UserType)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.UserTypeId);
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("user_type");

                entity.HasIndex(e => e.UserTypeId, "IX_user_type_user_type_id")
                      .IsUnique();

                entity.HasIndex(e => e.UserTypeName, "IX_user_type_user_type_name")
                      .IsUnique();

                entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");

                entity.Property(e => e.UserTypeName)
                      .IsRequired()
                      .HasColumnName("user_type_name");
            });

            modelBuilder.Entity<Value>(entity =>
            {
                entity.HasKey(e => new {e.MatId, e.PropId,});

                entity.ToTable("value");

                entity.Property(e => e.MatId).HasColumnName("mat_id");

                entity.Property(e => e.PropId).HasColumnName("prop_id");

                entity.Property(e => e.Value1).HasColumnName("value");

                entity.HasOne(d => d.Mat)
                      .WithMany(p => p.Values)
                      .HasForeignKey(d => d.MatId);

                entity.HasOne(d => d.Prop)
                      .WithMany(p => p.Values)
                      .HasForeignKey(d => d.PropId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            //throw new NotImplementedException();
        }
    }
}

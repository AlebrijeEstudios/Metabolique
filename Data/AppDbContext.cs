using Microsoft.EntityFrameworkCore;
using AppVidaSana.Models;
using AppVidaSana.Models.Seguimientos_Mensuales;

namespace AppVidaSana.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<SegMenEjercicio> SegMenEjercicios { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.perfil)
                .WithOne(p => p.cuenta)
                .HasForeignKey<Perfil>(p => p.id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(c => c.ejercicios)
                .WithOne(ej => ej.cuenta)
                .HasForeignKey(ej => ej.id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.seg_men_ej)
                .WithOne(smej => smej.cuenta)
                .HasForeignKey<SegMenEjercicio>(smej => smej.id)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}

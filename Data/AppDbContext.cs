using Microsoft.EntityFrameworkCore;
using AppVidaSana.Models;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Alimentación;
using AppVidaSana.Models.Seguimientos_Mensuales.Respuestas;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Models.Alimentación.Alimentos;
using AppVidaSana.Models.Habitos;

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

        public DbSet<Desayuno> Desayunos { get; set; }
        public DbSet<Almuerzo> Almuerzos { get; set; }
        public DbSet<Comida> Comidas { get; set; }
        public DbSet<Colacion> Colaciones { get; set; }
        public DbSet<Cena> Cenas { get; set; }
        public DbSet<Alimentos_Almuerzo> alimentosAlmuerzo { get; set; }
        public DbSet<Alimentos_Desayuno> alimentosDesayuno { get; set; }
        public DbSet<Alimentos_Comida> alimentosComida { get; set; }
        public DbSet<Alimentos_Colacion> alimentosColacion { get; set; }
        public DbSet<Alimentos_Cena> alimentosCena { get; set; }
        public DbSet<SegMenAlimentacion> SegMenAlimentacion { get; set; }
        public DbSet<RAlimentacion> resultadosAlimentacion { get; set; }

        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<EfectoSecundario> efectoSecundarios { get; set; }
        public DbSet<SegMenMedicamentos> SegMenMedicamentos { get; set; }

        public DbSet<HBebida> habitosBebida { get; set; }
        public DbSet<HDrogas> habitosDroga { get; set; }
        public DbSet<HSueño> habitosSueño { get; set; }
        public DbSet<SegMenHabitos> SegMenHabitos { get; set; }
        public DbSet<RHabitos> resultadosHabitos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Cuenta_Perfil
            modelBuilder.Entity<Cuenta>()
                .HasOne(cuenta => cuenta.perfil)
                .WithOne(perfil => perfil.cuenta)
                .HasForeignKey<Perfil>(perfil => perfil.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //Ejercicio y SegMenEjercicio
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.ejercicios)
                .WithOne(ejercicios => ejercicios.cuenta)
                .HasForeignKey(ejercicios => ejercicios.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.segMenEjercicio)
                .WithOne(segMenEjercicio => segMenEjercicio.cuenta)
                .HasForeignKey(segMenEjercicio => segMenEjercicio.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //Alimentacion(Desayuno, ..., Cena)
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.desayunos)
                .WithOne(desayunos => desayunos.cuenta)
                .HasForeignKey(desayunos => desayunos.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.almuerzos)
                .WithOne(almuerzos => almuerzos.cuenta)
                .HasForeignKey(almuerzos => almuerzos.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.comidas)
                .WithOne(comidas => comidas.cuenta)
                .HasForeignKey(comidas => comidas.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.colaciones)
                .WithOne(colaciones => colaciones.cuenta)
                .HasForeignKey(colaciones => colaciones.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.cenas)
                .WithOne(cenas => cenas.cuenta)
                .HasForeignKey(cenas => cenas.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //Alimentos (Desayuno, ..., Cena)
            modelBuilder.Entity<Desayuno>()
                .HasMany(desayuno => desayuno.alimentosDesayuno)
                .WithOne(alimentosDesayuno => alimentosDesayuno.desayuno)
                .HasForeignKey(alimentosDesayuno => alimentosDesayuno.desayunoID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Almuerzo>()
                .HasMany(almuerzo => almuerzo.alimentosAlmuerzo)
                .WithOne(alimentosAlmuerzo => alimentosAlmuerzo.almuerzo)
                .HasForeignKey(alimentosAlmuerzo => alimentosAlmuerzo.almuerzoID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comida>()
                .HasMany(comida => comida.alimentosComida)
                .WithOne(alimentosComida => alimentosComida.comida)
                .HasForeignKey(alimentosComida => alimentosComida.comidaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Colacion>()
                .HasMany(colacion => colacion.alimentosColacion)
                .WithOne(alimentosCol => alimentosCol.colacion)
                .HasForeignKey(alimentosCol => alimentosCol.colacionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cena>()
                .HasMany(cena => cena.alimentosCena)
                .WithOne(alimentosCena => alimentosCena.cena)
                .HasForeignKey(alimentosCena => alimentosCena.cenaID)
                .OnDelete(DeleteBehavior.Restrict);


            //SegMenAlimentos
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.segMenAlimentacion)
                .WithOne(segMenAlimentacion => segMenAlimentacion.cuenta)
                .HasForeignKey(segMenAlimentacion => segMenAlimentacion.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SegMenAlimentacion>()
                .HasOne(segMenAlimentacion => segMenAlimentacion.resultados)
                .WithOne(resultados => resultados.seguimientoMensualAlimentos)
                .HasForeignKey<RAlimentacion>(resultados => resultados.seguimientoMensualID)
                .OnDelete(DeleteBehavior.Restrict);


            //Medicamentos 
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.medicamentos)
                .WithOne(medicamentos => medicamentos.cuenta)
                .HasForeignKey(medicamentos => medicamentos.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //SegMenMedicamentos
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.segMenMedicamentos)
                .WithOne(segMenMedicamentos => segMenMedicamentos.cuenta)
                .HasForeignKey(segMenMedicamentos => segMenMedicamentos.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //Efectos secundarios
            modelBuilder.Entity<Cuenta>()
               .HasMany(cuenta => cuenta.efectoSecundarios)
               .WithOne(efs => efs.cuenta)
               .HasForeignKey(efs => efs.cuentaID)
               .OnDelete(DeleteBehavior.Restrict);

            //Habitos(Sueño, Bebidas, Drogas)
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.habitosSueño)
                .WithOne(habitosSueño => habitosSueño.cuenta)
                .HasForeignKey(habitosSueño => habitosSueño.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.habitosBebida)
                .WithOne(habitosBebida => habitosBebida.cuenta)
                .HasForeignKey(habitosBebida => habitosBebida.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.habitosDroga)
                .WithOne(habitosDroga => habitosDroga.cuenta)
                .HasForeignKey(habitosDroga => habitosDroga.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            //Seguimiento Mensual Habitos
            modelBuilder.Entity<Cuenta>()
                .HasMany(cuenta => cuenta.segMenHabitos)
                .WithOne(segMenHabitos => segMenHabitos.cuenta)
                .HasForeignKey(segMenHabitos => segMenHabitos.cuentaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SegMenHabitos>()
                .HasOne(segMenHabitos => segMenHabitos.resultados)
                .WithOne(resultados => resultados.seguimientoMensualHabitos)
                .HasForeignKey<RHabitos>(resultados => resultados.seguimientoMensualID)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}

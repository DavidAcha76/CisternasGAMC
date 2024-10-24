using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Model;

namespace CisternasGAMC.Data
{
    // Clase para manejar la conexión con la base de datos y el acceso a las tablas
    public class ApplicationDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración para el contexto de la base de datos
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets que representan las tablas en la base de datos
        public DbSet<Cistern> Cisterns { get; set; }  // Tabla de cisternas
        public DbSet<Otb> Otbs { get; set; }          // Tabla de OTBs
        public DbSet<User> Users { get; set; }        // Tabla de usuarios
        public DbSet<WaterDelivery> WaterDeliveries { get; set; } // Tabla de entregas de agua
    }
}

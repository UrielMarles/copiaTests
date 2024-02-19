using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacturasAxoft.Clases;
using Microsoft.EntityFrameworkCore;

namespace FacturasAxoft
{
    public class FacturasAxoftDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Factura> Facturas { get; set; }
        public DbSet<RenglonFactura> RenglonesFactura { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Articulo> Articulos { get; set; }

        public FacturasAxoftDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factura>() // relacion de uno a muchos factura -> renglones
                .HasMany(f => f.Renglones)
                .WithOne(r => r.Factura)
                .HasForeignKey(r => r.FacturaId);

            modelBuilder.Entity<Cliente>() // relacion de uno a muchos cliente -> facturas
                .HasMany(f => f.Facturas)
                .WithOne(r => r.Cliente)
                .HasForeignKey(r => r.ClienteId);

            modelBuilder.Entity<Factura>() // relacion de muchos a uno facturas -> cliente
                .HasOne(r => r.Cliente)
                .WithMany(f => f.Facturas)
                .HasForeignKey(r => r.ClienteId);

            modelBuilder.Entity<RenglonFactura>() // relacion muchos a uno renglones -> factura
                .HasOne(r => r.Factura)
                .WithMany(f => f.Renglones)
                .HasForeignKey(r => r.FacturaId);

            modelBuilder.Entity<RenglonFactura>() // relacion de muchos a uno renglones -> articulo
                .HasOne(r => r.Articulo)
                .WithMany(a => a.Renglones)
                .HasForeignKey(r => r.ArticuloId);
        }

    }
}

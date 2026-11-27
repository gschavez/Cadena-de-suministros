using CadenaSuministro.Models;
using Microsoft.EntityFrameworkCore;

namespace CadenaSuministro.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proveedor>().HasIndex(p => p.Nit).IsUnique();
        modelBuilder.Entity<Producto>().HasIndex(p => p.Codigo).IsUnique();

        modelBuilder.Entity<Producto>().Property(p => p.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<Cotizacion>().Property(c => c.MontoEstimado).HasPrecision(18, 2);

        // No se puede borrar un proveedor/producto/empleado con registros asociados
        modelBuilder.Entity<Cotizacion>()
            .HasOne(c => c.Proveedor).WithMany(p => p.Cotizaciones)
            .HasForeignKey(c => c.ProveedorId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cotizacion>()
            .HasOne(c => c.Producto).WithMany(p => p.Cotizaciones)
            .HasForeignKey(c => c.ProductoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrdenCompra>()
            .HasOne(o => o.Cotizacion).WithMany(c => c.Ordenes)
            .HasForeignKey(o => o.CotizacionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrdenCompra>()
            .HasOne(o => o.Empleado).WithMany(e => e.OrdenesAprobadas)
            .HasForeignKey(o => o.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
    }
}
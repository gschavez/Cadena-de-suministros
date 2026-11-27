using System.ComponentModel.DataAnnotations;

namespace CadenaSuministro.Models;

public enum EstadoCotizacion
{
    Pendiente,
    Aprobada,
    Rechazada
}

public class Cotizacion
{
    public int Id { get; set; }

    [Display(Name = "Proveedor")]
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    [Display(Name = "Producto")]
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    [Display(Name = "Monto estimado")]
    public decimal MontoEstimado { get; set; }

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Pendiente;

    public ICollection<OrdenCompra> Ordenes { get; set; } = new List<OrdenCompra>();
}
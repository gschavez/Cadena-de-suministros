using System.ComponentModel.DataAnnotations;

namespace CadenaSuministro.Models;

public enum EstadoOrden
{
    Pendiente,
    Aprobada,
    Rechazada
}

public class OrdenCompra
{
    public int Id { get; set; }

    [Display(Name = "Cotización")]
    public int CotizacionId { get; set; }
    public Cotizacion? Cotizacion { get; set; }

    [Display(Name = "Aprobada por")]
    public int EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de aprobación")]
    public DateTime? FechaAprobacion { get; set; }

    public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;
}
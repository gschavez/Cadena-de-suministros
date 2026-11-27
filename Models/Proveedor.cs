using System.ComponentModel.DataAnnotations;

namespace CadenaSuministro.Models;

public class Proveedor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El NIT es obligatorio")]
    [StringLength(20)]
    public string Nit { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Contacto { get; set; }

    [StringLength(200)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Phone, StringLength(30)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [EmailAddress, StringLength(120)]
    public string? Email { get; set; }

    public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
}
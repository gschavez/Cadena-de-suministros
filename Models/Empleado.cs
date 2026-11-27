using System.ComponentModel.DataAnnotations;

namespace CadenaSuministro.Models;

public class Empleado
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Cargo { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Departamento { get; set; } = string.Empty;

    public ICollection<OrdenCompra> OrdenesAprobadas { get; set; } = new List<OrdenCompra>();
}
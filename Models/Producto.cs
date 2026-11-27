using System.ComponentModel.DataAnnotations;

namespace CadenaSuministro.Models;

public enum TipoItem
{
    Producto,
    MateriaPrima,
    Insumo
}

public class Producto
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    public TipoItem Tipo { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Unidad de medida")]
    public string UnidadMedida { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Cantidad { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    [Display(Name = "Precio unitario")]
    public decimal PrecioUnitario { get; set; }

    public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
}
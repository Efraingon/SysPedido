using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

/// <summary>
/// Entidad de solo lectura que mapea la tabla legacy dbo.renglonCotizacion del ERP.
/// </summary>
public class RenglonCotizacion
{
    [Column(TypeName = "varchar(11)")]
    public string NumeroCotizacion { get; set; } = string.Empty;

    public int ConsecutivoRenglon { get; set; }

    [Column(TypeName = "varchar(30)")]
    public string CodigoArticulo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }
    public decimal PrecioSinIVA { get; set; }
    public decimal PrecioConIVA { get; set; }
    public decimal TotalRenglon { get; set; }

    // Multi-tenant — mapeado desde ConsecutivoCompania
    public int EmpresaId { get; set; }

    // Navegación
    [ForeignKey("NumeroCotizacion")]
    public virtual Cotizacion? Cotizacion { get; set; }
}

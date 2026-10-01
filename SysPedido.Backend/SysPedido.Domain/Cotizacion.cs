using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

/// <summary>
/// Entidad de solo lectura que mapea la tabla legacy dbo.cotizacion del ERP.
/// Se usa como fuente de pedidos históricos.
/// </summary>
public class Cotizacion : IMultiTenantEntity
{
    [Key]
    [Column(TypeName = "varchar(11)")]
    public string Numero { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    [Column(TypeName = "varchar(10)")]
    public string CodigoCliente { get; set; } = string.Empty;

    [Column(TypeName = "varchar(5)")]
    public string CodigoVendedor { get; set; } = string.Empty;

    public int ConsecutivoVendedor { get; set; }

    public string Observaciones { get; set; } = string.Empty;

    // Totales — mapeados desde columnas con distinto nombre en la tabla
    [Column("TotalCotizacion")]
    public decimal Total { get; set; }

    [Column("TotalMontoExento")]
    public decimal Exento { get; set; }

    [Column("TotalBaseImponible")]
    public decimal Base { get; set; }

    [Column("TotalIVA")]
    public decimal IVA { get; set; }

    public string Moneda { get; set; } = string.Empty;

    [Column(TypeName = "varchar(4)")]
    public string CodigoMoneda { get; set; } = string.Empty;

    // Multi-tenant — mapeado desde ConsecutivoCompania
    public int EmpresaId { get; set; }

    // Propiedades de navegación
    [ForeignKey("CodigoCliente")]
    public virtual Cliente? Cliente { get; set; }
    public virtual ICollection<RenglonCotizacion> Renglones { get; set; } = new List<RenglonCotizacion>();
}

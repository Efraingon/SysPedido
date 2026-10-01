using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SysPedido.Domain.Entities;

public class Producto : IMultiTenantEntity
{
    [Column(TypeName = "varchar(30)")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string Descripcion { get; set; } = string.Empty;

    // Precios tomados de CamposMonedaExtranjera
    [NotMapped]
    [JsonPropertyName("precioSinIva")]
    public decimal PrecioSinIva { get => CamposMonedaExtranjera?.MePrecioSinIva ?? 0; set { } }

    [NotMapped]
    [JsonPropertyName("precioConIva")]
    public decimal PrecioConIva { get => CamposMonedaExtranjera?.MePrecioConIva ?? 0; set { } }

    public decimal Existencia { get; set; }
    
    [Column(TypeName = "varchar(100)")]
    public string Categoria { get; set; } = string.Empty;

    [Column(TypeName = "char(1)")]
    public string AlicuotaIva { get; set; } = "0";

    [Column(TypeName = "varchar(20)")]
    public string LineaDeProducto { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }

    // Relación con CamposMonedaExtranjera (Codigo + ConsecutivoCompania)
    public virtual CamposMonedaExtranjera? CamposMonedaExtranjera { get; set; }
}

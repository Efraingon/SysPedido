using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

public class RenglonPedido : IMultiTenantEntity
{
    // FK
    [Column(TypeName = "varchar(11)")]
    public string NumeroCotizacion { get; set; } = string.Empty;
    
    public int ConsecutivoRenglon { get; set; }
    
    // FK
    [Column(TypeName = "varchar(30)")]
    public string CodigoArticulo { get; set; } = string.Empty;
    
    public string Descripcion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioSinIVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioConIVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalRenglon { get; set; }
    
    public int EmpresaId { get; set; }

    // Navigation
    public virtual Pedido? Pedido { get; set; }
    public virtual Producto? Producto { get; set; }
}

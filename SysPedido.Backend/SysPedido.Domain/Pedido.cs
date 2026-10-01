using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

public class Pedido : IMultiTenantEntity
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

    [Column(TypeName = "varchar(255)")]
    public string Observaciones { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public string CodigoMoneda { get; set; } = string.Empty;
    
    // Geolocalizacion
    public double Latitud { get; set; }
    
    public double Longitud { get; set; }
    
    public int EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }

    // Navigation Properties
    [ForeignKey("CodigoCliente")]
    public virtual Cliente? Cliente { get; set; }

    [ForeignKey("CodigoVendedor,ConsecutivoVendedor")]
    public virtual Vendedor? Vendedor { get; set; }
    public virtual ICollection<RenglonPedido> Renglones { get; set; } = new List<RenglonPedido>();
}

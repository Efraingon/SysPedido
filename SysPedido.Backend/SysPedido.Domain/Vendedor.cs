using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysPedido.Domain.Interfaces;

namespace SysPedido.Domain.Entities;

public class Vendedor : IMultiTenantEntity
{
    [Key]
    public int Consecutivo { get; set; }
    
    [Column(TypeName = "varchar(5)")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string Nombre { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }

    public string RIF { get; set; } = string.Empty;
    public string StatusVendedor { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ZonaPostal { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

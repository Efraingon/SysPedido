using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysPedido.Domain.Interfaces;

namespace SysPedido.Domain.Entities;

public class Cliente : IMultiTenantEntity
{
    [Key]
    public int Consecutivo { get; set; }
    
    [Column(TypeName = "varchar(10)")]
    public string Codigo { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(160)")]
    public string Nombre { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(20)")]
    public string NumeroRIF { get; set; } = string.Empty;
    
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ZonaPostal { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    
    public string CodigoVendedor { get; set; } = string.Empty;
    public int ConsecutivoVendedor { get; set; }
    
    public string Email { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}

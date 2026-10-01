using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

public class Usuario : IMultiTenantEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(255)")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(20)")]
    public string Rol { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public virtual Empresa? Empresa { get; set; }
}

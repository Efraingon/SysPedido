using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

public class Empresa
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string RIF { get; set; } = string.Empty;
}

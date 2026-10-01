using SysPedido.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysPedido.Domain.Entities;

public class CamposMonedaExtranjera : IMultiTenantEntity
{
    [Column(TypeName = "varchar(30)")]
    public string Codigo { get; set; } = string.Empty;

    public decimal MePrecioSinIva { get; set; }
    public decimal MePrecioConIva { get; set; }

    // Multi-tenant: maps to ConsecutivoCompania in legacy DB
    public int EmpresaId { get; set; }
}

namespace SysPedido.Application.DTOs;

public class PedidoDto
{
    public string Numero { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string CodigoCliente { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string CodigoVendedor { get; set; } = string.Empty;
    public int ConsecutivoVendedor { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public decimal Exento { get; set; }
    public decimal Base { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public string CodigoMoneda { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public int CantidadItems { get; set; }
    public int EmpresaId { get; set; }
    public List<RenglonPedidoDto> Renglones { get; set; } = new();
}

public class RenglonPedidoDto
{
    public int ConsecutivoRenglon { get; set; }
    public string CodigoArticulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioSinIVA { get; set; }
    public decimal PrecioConIVA { get; set; }
    public decimal TotalRenglon { get; set; }
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
}

namespace SysPedido.Application.DTOs;

public class ResumenDashboardDto
{
    public int TotalPedidos { get; set; }
    public decimal TotalVentas { get; set; }
    public List<VentasPorProductoDto> VentasPorProducto { get; set; } = new();
    public List<PedidosPorDiaDto> PedidosPorDia { get; set; } = new();
    public List<DistribucionCategoriaDto> DistribucionCategoria { get; set; } = new();
}

public class VentasPorProductoDto
{
    public string NombreProducto { get; set; } = string.Empty;
    public decimal TotalVendido { get; set; }
}

public class PedidosPorDiaDto
{
    public string Fecha { get; set; } = string.Empty;
    public int CantidadPedidos { get; set; }
}

public class DistribucionCategoriaDto
{
    public string Categoria { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

using SysPedido.Application.DTOs;

namespace SysPedido.Application.Interfaces;

public interface IDashboardService
{
    Task<ResumenDashboardDto> GetResumenAsync(DateTime? startDate, DateTime? endDate);
}

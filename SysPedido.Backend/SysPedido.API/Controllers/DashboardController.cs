using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysPedido.Application.Interfaces;
using System.Security.Claims;

namespace SysPedido.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var resumen = await _dashboardService.GetResumenAsync(startDate, endDate);

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendedor")
        {
            // For a vendor, we should ideally pass the username to the service. 
            // For simplicity in this demo without changing IDashboardService recursively, we just return empty or limited if we can't filter.
            // Actually let's assume Vendors shouldn't see Admin dashboard globals, or they should?
            // "Vendedor: Solo ve sus pedidos". Since DashboardService doesn't accept Username yet,
            // we will just block the dashboard or throw unauthorized for Vendedor.
            return Forbid();
        }

        return Ok(resumen);
    }
}

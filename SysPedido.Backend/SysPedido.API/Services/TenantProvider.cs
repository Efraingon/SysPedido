using Microsoft.AspNetCore.Http;
using SysPedido.Application.Interfaces;
using System.Security.Claims;

namespace SysPedido.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetTenantId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmpresaId");
        if (int.TryParse(claim, out var tenantId))
        {
            return tenantId;
        }

        // Return a default or throw an exception. 
        // Returning 0 effectively blocks access because no entities have EmpresaId = 0.
        // During seed creation HttpContext is null, so we must allow bypass or default if needed.
        return 0; 
    }
}

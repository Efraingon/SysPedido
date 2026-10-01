using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysPedido.Application.Interfaces;

namespace SysPedido.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;
    public ClienteController(IClienteService service) => _service = service;
    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IProductoService _service;
    public ProductoController(IProductoService service) => _service = service;
    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
}

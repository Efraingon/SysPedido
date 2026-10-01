using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysPedido.Application.DTOs;
using SysPedido.Application.Interfaces;
using System.Security.Claims;

namespace SysPedido.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pedidos = await _pedidoService.GetAllAsync();
        
        // Filter by role manually here or in service. Doing it here for quickly prototype
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendedor")
        {
            var username = User.Identity?.Name;
            pedidos = pedidos.Where(p => p.CodigoVendedor == username).ToList();
        }

        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PedidoDto dto)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == "Vendedor")
        {
            dto.CodigoVendedor = User.Identity?.Name ?? dto.CodigoVendedor;
        }

        var result = await _pedidoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Numero }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] PedidoDto dto)
    {
        var result = await _pedidoService.UpdateAsync(id, dto);
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _pedidoService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SysPedido.Infrastructure.Data;
using SysPedido.Application.Interfaces;

var services = new ServiceCollection();
var connectionString = "Server=DESKTOP-HA6EJBN\\SQLEXPRESS;Database=SAWDB_CLUB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
services.AddDbContext<SysPedidoDbContext>(options => options.UseSqlServer(connectionString));
services.AddScoped<ITenantProvider, MockTenantProvider>();
var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SysPedidoDbContext>();

int empresaId = 4;
int yearTwoDigits = DateTime.Now.Year % 100;
string prefijo = $"C{yearTwoDigits}-";

var maxNew = await context.Pedidos
    .IgnoreQueryFilters()
    .Where(p => p.EmpresaId == empresaId && p.Numero.StartsWith(prefijo))
    .Select(p => p.Numero)
    .OrderByDescending(n => n)
    .FirstOrDefaultAsync();

Console.WriteLine($"Max New: {maxNew ?? "null"}");

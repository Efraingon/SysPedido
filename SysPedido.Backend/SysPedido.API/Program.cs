using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SysPedido.Application.Interfaces;
using SysPedido.Application.Services;
using SysPedido.CrossCutting.Security;
using SysPedido.Infrastructure.Data;
using SysPedido.Infrastructure.Messaging;
using SysPedido.Infrastructure.Repositories;
using SysPedido.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SysPedido API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Architecture specific DI
// Architecture specific DI (Redirected to SAWDB_CLUB SQLEXPRESS)
var connectionString = "Server=DESKTOP-HA6EJBN\\SQLEXPRESS;Database=SAWDB_CLUB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
builder.Services.AddDbContext<SysPedidoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Tenant Provider and HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IEventBus, MockServiceBus>();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, SysPedido.API.Hubs.CustomUserIdProvider>();
builder.Services.AddScoped<IPedidoNotificationService, SysPedido.API.Services.PedidoNotificationService>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("EstaEsUnaLLaveMuySecretaYDebeSerLarga12345!")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/pedidos"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// CORS (Para frontend local Vite)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
    {
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware de Diagnóstico Global para prevenir bloqueos de acceso
app.Use(async (context, next) => {
    try {
        await next();
    }
    catch (Exception ex) {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        var errorResponse = new { 
            error = "Error Crítico de Configuración", 
            message = ex.Message, 
            inner = ex.InnerException?.Message,
            source = ex.Source
        };
        await System.Text.Json.JsonSerializer.SerializeAsync(context.Response.Body, errorResponse);
    }
});

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // DB initialization handled via migrations or setup script
    // EnsureCreated was removed to avoid conflicts with legacy DB

}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SysPedido.API.Hubs.PedidoHub>("/hubs/pedidos");
app.Run();

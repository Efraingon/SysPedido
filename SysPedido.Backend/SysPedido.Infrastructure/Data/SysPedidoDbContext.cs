using Microsoft.EntityFrameworkCore;
using SysPedido.Domain.Entities;
using SysPedido.Domain.Interfaces;
using SysPedido.Application.Interfaces;

namespace SysPedido.Infrastructure.Data;

public class SysPedidoDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    private int TenantId => _tenantProvider?.GetTenantId() ?? 0;

    public SysPedidoDbContext(DbContextOptions<SysPedidoDbContext> options, ITenantProvider? tenantProvider = null) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<RenglonPedido> RenglonPedidos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Vendedor> Vendedores { get; set; }
    public DbSet<CamposMonedaExtranjera> CamposMonedaExtranjera { get; set; }

    // ── Tablas legacy del ERP (solo lectura) ──────────────────────────────────
    public DbSet<Cotizacion> Cotizaciones { get; set; }
    public DbSet<RenglonCotizacion> RenglonesCotizacion { get; set; }

    public override int SaveChanges()
    {
        SetTenantIdAutomatically();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTenantIdAutomatically();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetTenantIdAutomatically()
    {
        var entries = ChangeTracker.Entries<IMultiTenantEntity>()
            .Where(e => e.State == EntityState.Added);

        var tenantId = TenantId;
        
        // Disable automatic assign if we are seeding or we have no context, but in production, we block
        if (tenantId > 0)
        {
            foreach (var entry in entries)
            {
                entry.Entity.EmpresaId = tenantId;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Global Query Filters (Tenancy - Mapped to ConsecutivoCompania in legacy DB)
        modelBuilder.Entity<Usuario>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);
        modelBuilder.Entity<Empresa>().HasQueryFilter(e => TenantId == 0 || e.Id == TenantId);
        
        modelBuilder.Entity<Pedido>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);
        
        modelBuilder.Entity<Producto>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);
        modelBuilder.Entity<Cliente>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);
        modelBuilder.Entity<Vendedor>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);
        modelBuilder.Entity<CamposMonedaExtranjera>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);

        // Filtro multi-tenant para entidades legacy
        modelBuilder.Entity<Cotizacion>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);

        modelBuilder.Entity<RenglonPedido>().HasQueryFilter(e => TenantId == 0 || e.EmpresaId == TenantId);

        // Map to SAWDB_CLUB Legacy Tables (Read-only for the app, exclude from migrations)
        modelBuilder.Entity<Cotizacion>(entity => {
            entity.ToTable("cotizacion", "dbo", t => t.ExcludeFromMigrations());
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
            
            // Mapeo de columnas con nombres legacy
            entity.Property(e => e.Total).HasColumnName("TotalCotizacion");
            entity.Property(e => e.Exento).HasColumnName("TotalMontoExento");
            entity.Property(e => e.Base).HasColumnName("TotalBaseImponible");
            entity.Property(e => e.IVA).HasColumnName("TotalIVA");

            // Relación con Cliente
            entity.HasOne(c => c.Cliente)
                .WithMany()
                .HasPrincipalKey(cl => cl.Codigo)
                .HasForeignKey(c => c.CodigoCliente)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RenglonCotizacion>(entity => {
            entity.ToTable("renglonCotizacion", "dbo", t => t.ExcludeFromMigrations());
            entity.HasKey(e => new { e.NumeroCotizacion, e.ConsecutivoRenglon });
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
            entity.HasOne(r => r.Cotizacion)
                  .WithMany(c => c.Renglones)
                  .HasForeignKey(r => r.NumeroCotizacion);
        });

        modelBuilder.Entity<Cliente>(entity => {
            entity.ToTable("Cliente", "dbo", t => t.ExcludeFromMigrations());
            entity.HasKey(c => c.Consecutivo);
            entity.HasAlternateKey(c => c.Codigo); // Necesario para usar Codigo como PrincipalKey para relaciones
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
        });

        modelBuilder.Entity<Vendedor>(entity => {
            entity.ToTable("Vendedor", "Adm", t => t.ExcludeFromMigrations());
            entity.HasKey(v => v.Consecutivo);
            entity.HasAlternateKey(v => new { v.Codigo, v.Consecutivo }); // Key compuesta para relación con Pedido
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
        });

        modelBuilder.Entity<Producto>(entity => {
            entity.ToTable("ArticuloInventario", "dbo", t => t.ExcludeFromMigrations());
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
            entity.HasKey(e => new { e.Codigo, e.EmpresaId });
            
            // Ignorar las columnas físicas de precio en ArticuloInventario (legacy)
            entity.Ignore(e => e.PrecioSinIva);
            entity.Ignore(e => e.PrecioConIva);
        });

        modelBuilder.Entity<CamposMonedaExtranjera>(entity => {
            entity.ToTable("CamposMonedaExtranjera", "dbo", t => t.ExcludeFromMigrations());
            entity.Property(e => e.EmpresaId).HasColumnName("ConsecutivoCompania");
            entity.HasKey(e => new { e.Codigo, e.EmpresaId });
        });

        // Relación Producto -> CamposMonedaExtranjera
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.CamposMonedaExtranjera)
            .WithOne()
            .HasForeignKey<CamposMonedaExtranjera>(c => new { c.Codigo, c.EmpresaId })
            .HasPrincipalKey<Producto>(p => new { p.Codigo, p.EmpresaId });

        // App Internal Tables (New ones in PedidoApp schema)
        modelBuilder.Entity<Usuario>().ToTable("Usuario", "PedidoApp");
        modelBuilder.Entity<Empresa>().ToTable("Empresa", "PedidoApp");
        
        modelBuilder.Entity<Pedido>(entity => {
            entity.ToTable("Pedido", "PedidoApp");
            entity.HasKey(p => p.Numero);
            
            // Relación Pedido -> Cliente
            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasPrincipalKey(c => c.Codigo)
                .HasForeignKey(p => p.CodigoCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Pedido -> Vendedor (CodigoVendedor + ConsecutivoVendedor)
            entity.HasOne(p => p.Vendedor)
                .WithMany(v => v.Pedidos)
                .HasPrincipalKey(v => new { v.Codigo, v.Consecutivo })
                .HasForeignKey(p => new { p.CodigoVendedor, p.ConsecutivoVendedor })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RenglonPedido>(entity => {
            entity.ToTable("RenglonPedido", "PedidoApp");
            entity.HasKey(r => new { r.NumeroCotizacion, r.ConsecutivoRenglon });
            
            entity.Property(e => e.Descripcion).HasColumnName("DescripcionArticulo");
            entity.Property(e => e.PrecioConIVA).HasColumnName("Precio");
            entity.Property(e => e.TotalRenglon).HasColumnName("Total");
            entity.Ignore(e => e.PrecioSinIVA);

            // RenglonPedido -> Pedido
            entity.HasOne(r => r.Pedido)
                .WithMany(p => p.Renglones)
                .HasForeignKey(r => r.NumeroCotizacion)
                .OnDelete(DeleteBehavior.Cascade);

            // RenglonPedido -> Producto
            entity.HasOne(r => r.Producto)
                .WithMany()
                .HasForeignKey(r => new { r.CodigoArticulo, r.EmpresaId })
                .HasPrincipalKey(p => new { p.Codigo, p.EmpresaId })
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

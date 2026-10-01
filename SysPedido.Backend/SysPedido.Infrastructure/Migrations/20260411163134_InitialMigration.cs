using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SysPedido.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PedidoApp");

            migrationBuilder.CreateTable(
                name: "Empresa",
                schema: "PedidoApp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(150)", nullable: false),
                    RIF = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                schema: "PedidoApp",
                columns: table => new
                {
                    Numero = table.Column<string>(type: "varchar(11)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodigoCliente = table.Column<string>(type: "varchar(10)", nullable: false),
                    CodigoVendedor = table.Column<string>(type: "varchar(5)", nullable: false),
                    ConsecutivoVendedor = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(255)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoMoneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    ClienteConsecutivo = table.Column<int>(type: "int", nullable: true),
                    VendedorConsecutivo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Numero);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_ClienteConsecutivo",
                        column: x => x.ClienteConsecutivo,
                        principalSchema: "dbo",
                        principalTable: "Cliente",
                        principalColumn: "Consecutivo");
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_CodigoCliente",
                        column: x => x.CodigoCliente,
                        principalSchema: "dbo",
                        principalTable: "Cliente",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "PedidoApp",
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedido_Vendedor_CodigoVendedor_ConsecutivoVendedor",
                        columns: x => new { x.CodigoVendedor, x.ConsecutivoVendedor },
                        principalSchema: "Adm",
                        principalTable: "Vendedor",
                        principalColumns: new[] { "Codigo", "Consecutivo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Vendedor_VendedorConsecutivo",
                        column: x => x.VendedorConsecutivo,
                        principalSchema: "Adm",
                        principalTable: "Vendedor",
                        principalColumn: "Consecutivo");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "PedidoApp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", nullable: false),
                    Rol = table.Column<string>(type: "varchar(20)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "PedidoApp",
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RenglonPedido",
                schema: "PedidoApp",
                columns: table => new
                {
                    NumeroCotizacion = table.Column<string>(type: "varchar(11)", nullable: false),
                    ConsecutivoRenglon = table.Column<int>(type: "int", nullable: false),
                    CodigoArticulo = table.Column<string>(type: "varchar(30)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioSinIVA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioConIVA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRenglon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenglonPedido", x => new { x.NumeroCotizacion, x.ConsecutivoRenglon });
                    table.ForeignKey(
                        name: "FK_RenglonPedido_ArticuloInventario_CodigoArticulo_EmpresaId",
                        columns: x => new { x.CodigoArticulo, x.EmpresaId },
                        principalSchema: "dbo",
                        principalTable: "ArticuloInventario",
                        principalColumns: new[] { "Codigo", "ConsecutivoCompania" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RenglonPedido_Pedido_NumeroCotizacion",
                        column: x => x.NumeroCotizacion,
                        principalSchema: "PedidoApp",
                        principalTable: "Pedido",
                        principalColumn: "Numero",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "PedidoApp",
                table: "Empresa",
                columns: new[] { "Id", "Nombre", "RIF" },
                values: new object[,]
                {
                    { 2, "Empresa B - SAWDB", "J-2222222" },
                    { 4, "Empresa 4 - SAWDB", "J-4444444" }
                });

            migrationBuilder.InsertData(
                schema: "PedidoApp",
                table: "Usuario",
                columns: new[] { "Id", "EmpresaId", "PasswordHash", "Rol", "Username" },
                values: new object[,]
                {
                    { 1, 4, "$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi", "Admin", "admin" },
                    { 2, 2, "$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi", "Vendedor", "ventas" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ClienteConsecutivo",
                schema: "PedidoApp",
                table: "Pedido",
                column: "ClienteConsecutivo");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_CodigoCliente",
                schema: "PedidoApp",
                table: "Pedido",
                column: "CodigoCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_CodigoVendedor_ConsecutivoVendedor",
                schema: "PedidoApp",
                table: "Pedido",
                columns: new[] { "CodigoVendedor", "ConsecutivoVendedor" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_EmpresaId",
                schema: "PedidoApp",
                table: "Pedido",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_VendedorConsecutivo",
                schema: "PedidoApp",
                table: "Pedido",
                column: "VendedorConsecutivo");

            migrationBuilder.CreateIndex(
                name: "IX_RenglonPedido_CodigoArticulo_EmpresaId",
                schema: "PedidoApp",
                table: "RenglonPedido",
                columns: new[] { "CodigoArticulo", "EmpresaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EmpresaId",
                schema: "PedidoApp",
                table: "Usuario",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RenglonPedido",
                schema: "PedidoApp");

            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "PedidoApp");

            migrationBuilder.DropTable(
                name: "Pedido",
                schema: "PedidoApp");

            migrationBuilder.DropTable(
                name: "Empresa",
                schema: "PedidoApp");
        }
    }
}

USE SAWDB_CLUB;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'PedidoApp') EXEC('CREATE SCHEMA PedidoApp');
GO

-- 1. Empresa
IF OBJECT_ID('PedidoApp.Empresa', 'U') IS NULL
BEGIN
    CREATE TABLE PedidoApp.Empresa (
        Id INT PRIMARY KEY,
        Nombre VARCHAR(150) NOT NULL,
        RIF VARCHAR(50) NOT NULL
    );
    
    INSERT INTO PedidoApp.Empresa (Id, Nombre, RIF) VALUES (4, 'Empresa 4 - SAWDB', 'J-4444444');
    INSERT INTO PedidoApp.Empresa (Id, Nombre, RIF) VALUES (2, 'Empresa B - SAWDB', 'J-2222222');
END
GO

-- 2. Usuario
IF OBJECT_ID('PedidoApp.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE PedidoApp.Usuario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username VARCHAR(100) NOT NULL,
        PasswordHash VARCHAR(255) NOT NULL,
        Rol VARCHAR(50) NOT NULL,
        EmpresaId INT NOT NULL,
        CONSTRAINT FK_Usuario_Empresa FOREIGN KEY (EmpresaId) REFERENCES PedidoApp.Empresa(Id)
    );
    
    -- Admin y Ventas con password '123456'
    INSERT INTO PedidoApp.Usuario (Username, PasswordHash, Rol, EmpresaId) 
    VALUES ('admin', '$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi', 'Admin', 4);
    
    INSERT INTO PedidoApp.Usuario (Username, PasswordHash, Rol, EmpresaId) 
    VALUES ('ventas', '$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi', 'Vendedor', 2);
END
ELSE
BEGIN
    -- Asegurar hashes correctos si ya existe
    UPDATE PedidoApp.Usuario 
    SET PasswordHash = '$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi' 
    WHERE Username IN ('admin', 'ventas');
END
GO

-- 3. Pedido (Simplificado para evitar errores de FK con tablas legacy por ahora)
IF OBJECT_ID('PedidoApp.Pedido', 'U') IS NULL
BEGIN
    CREATE TABLE PedidoApp.Pedido (
        Numero VARCHAR(11) PRIMARY KEY,
        Fecha DATETIME2 NOT NULL,
        CodigoCliente VARCHAR(10) NOT NULL,
        CodigoVendedor VARCHAR(5) NOT NULL,
        ConsecutivoVendedor INT NOT NULL,
        Observaciones VARCHAR(255) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        Moneda NVARCHAR(MAX) NOT NULL,
        CodigoMoneda NVARCHAR(MAX) NOT NULL,
        Latitud FLOAT NOT NULL,
        Longitud FLOAT NOT NULL,
        EmpresaId INT NOT NULL,
        CONSTRAINT FK_Pedido_Empresa FOREIGN KEY (EmpresaId) REFERENCES PedidoApp.Empresa(Id)
    );
END
GO

-- 4. RenglonPedido
IF OBJECT_ID('PedidoApp.RenglonPedido', 'U') IS NULL
BEGIN
    CREATE TABLE PedidoApp.RenglonPedido (
        NumeroCotizacion VARCHAR(11) NOT NULL,
        ConsecutivoRenglon INT NOT NULL,
        CodigoArticulo VARCHAR(20) NOT NULL,
        DescripcionArticulo VARCHAR(200) NOT NULL,
        Cantidad DECIMAL(18,2) NOT NULL,
        Precio DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        EmpresaId INT NOT NULL,
        PRIMARY KEY (NumeroCotizacion, ConsecutivoRenglon),
        CONSTRAINT FK_Renglon_Pedido FOREIGN KEY (NumeroCotizacion) REFERENCES PedidoApp.Pedido(Numero) ON DELETE CASCADE,
        CONSTRAINT FK_Renglon_Empresa FOREIGN KEY (EmpresaId) REFERENCES PedidoApp.Empresa(Id)
    );
END
GO

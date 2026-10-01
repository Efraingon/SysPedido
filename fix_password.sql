USE SAWDB_CLUB;
GO
UPDATE PedidoApp.Usuario 
SET PasswordHash = '$2a$11$1A47p9DojqvUa2Px6tUNbu0142TS723j1MwewEgLYpVI4SvwpvBSi' 
WHERE Username = 'admin';
GO
SELECT Username, LEN(PasswordHash) as Len, PasswordHash FROM PedidoApp.Usuario;
GO

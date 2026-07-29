
-- SP: Obtener Perfiles de un Usuario
CREATE   PROCEDURE [dbo].[ObtenerPerfilesUsuario]
    @IdUsuario UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.[id], p.[Nombre]
    FROM [dbo].[Perfiles] p
    INNER JOIN [dbo].[PerfilesxUsuario] pxu ON p.[id] = pxu.[IdPerfil]
    WHERE pxu.[IdUsuario] = @IdUsuario;
END
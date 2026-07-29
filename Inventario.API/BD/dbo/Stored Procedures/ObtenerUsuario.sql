
-- SP: Obtener Usuario (Por Nombre o Correo)
CREATE   PROCEDURE [dbo].[ObtenerUsuario]
    @NombreUsuario VARCHAR(MAX) = NULL,
    @CorreoElectronico VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        [id], [NombreUsuario], [PasswordHash], [CorreoElectronico], 
        [FechaCreacion], [FechaModificacion], [UsuarioCrea], [UsuarioModifica]
    FROM [dbo].[Usuarios]
    WHERE 
        (@NombreUsuario IS NOT NULL AND [NombreUsuario] = @NombreUsuario)
        OR 
        (@CorreoElectronico IS NOT NULL AND [CorreoElectronico] = @CorreoElectronico);
END
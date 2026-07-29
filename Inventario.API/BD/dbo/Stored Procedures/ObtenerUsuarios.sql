
-- 3. SP: Obtener Todos los Usuarios (Lista general para la pantalla de administración)
CREATE   PROCEDURE [dbo].[ObtenerUsuarios]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [id], 
           [NombreUsuario], 
           [CorreoElectronico], 
           [FechaCreacion], 
           [FechaModificacion], 
           [UsuarioCrea], 
           [UsuarioModifica]
      FROM [dbo].[Usuarios]
     ORDER BY [NombreUsuario] ASC;
END

-- 4. SP: Editar Usuario
CREATE   PROCEDURE [dbo].[EditarUsuario]
    @Id UNIQUEIDENTIFIER,
    @NombreUsuario VARCHAR(MAX),
    @CorreoElectronico VARCHAR(MAX),
    @PasswordHash VARCHAR(MAX) = NULL, -- Opcional: si viene NULL, se mantiene la contraseña actual
    @UsuarioModifica UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRAN;
        UPDATE [dbo].[Usuarios]
           SET [NombreUsuario]     = @NombreUsuario,
               [CorreoElectronico] = @CorreoElectronico,
               -- Si el frontend no manda una nueva contraseña (@PasswordHash es NULL), deja la que ya estaba:
               [PasswordHash]      = ISNULL(@PasswordHash, [PasswordHash]), 
               [UsuarioModifica]   = @UsuarioModifica,
               [FechaModificacion] = GETDATE()
         WHERE [id] = @Id;
    COMMIT TRAN;

    -- Retorno estándar para tu ExecuteScalarAsync en Dapper
    SELECT @Id;
END
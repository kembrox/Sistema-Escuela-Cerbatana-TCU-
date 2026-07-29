-- =========================================================
-- MÓDULO: SEGURIDAD Y AUTENTICACIÓN
-- =========================================================

-- SP: Agregar Usuario
CREATE   PROCEDURE [dbo].[AgregarUsuario]
    @NombreUsuario VARCHAR(MAX),
    @PasswordHash VARCHAR(MAX),
    @CorreoElectronico VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id AS UNIQUEIDENTIFIER = NEWID();

    BEGIN TRAN;
        INSERT INTO [dbo].[Usuarios] (
            [id], [NombreUsuario], [PasswordHash], [CorreoElectronico]
        )
        VALUES (
            @Id, @NombreUsuario, @PasswordHash, @CorreoElectronico
        );

        INSERT INTO [dbo].[PerfilesxUsuario] (
            [IdUsuario], [IdPerfil]
        )
        VALUES (
            @Id, 2 -- Perfil por defecto
        );
    COMMIT TRAN;

    -- Retorno para ExecuteScalarAsync
    SELECT @Id;
END
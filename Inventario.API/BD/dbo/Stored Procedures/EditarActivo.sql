
-- EDITAR ACTIVO
CREATE   PROCEDURE EditarActivo
    @Id                AS UNIQUEIDENTIFIER,
    @CodigoFisico      AS VARCHAR(MAX) = NULL,
    @Descripcion       AS VARCHAR(MAX) = NULL,
    @IdCategoria       AS UNIQUEIDENTIFIER,
    @IdUbicacion       AS UNIQUEIDENTIFIER,
    @Estado            AS BIT,
    @Observaciones     AS VARCHAR(MAX) = NULL,
    @UsuarioModifica   AS UNIQUEIDENTIFIER = NULL,
    @FechaModificacion AS DATETIME = NULL,
    @Marca             AS VARCHAR(100) = NULL,
    @Modelo            AS VARCHAR(100) = NULL,
    @Serie             AS VARCHAR(100) = NULL,
    @Precio            AS DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        UPDATE dbo.Activos
           SET CodigoFisico      = @CodigoFisico,
               Descripcion       = @Descripcion,
               IdCategoria       = @IdCategoria,
               IdUbicacion       = @IdUbicacion,
               Estado            = @Estado,
               Observaciones     = @Observaciones,
               UsuarioModifica   = @UsuarioModifica,
               FechaModificacion = ISNULL(@FechaModificacion, GETDATE()),
               Marca             = @Marca,
               Modelo            = @Modelo,
               Serie             = @Serie,
               Precio            = @Precio
         WHERE id = @Id;
    COMMIT TRANSACTION;

    -- Retorno fundamental para tu ExecuteScalarAsync en C#
    SELECT @Id;
END
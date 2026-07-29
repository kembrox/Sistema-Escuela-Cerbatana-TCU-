

-- =========================================================
-- MÓDULO: INVENTARIO (ACTIVOS)
-- =========================================================

-- AGREGAR ACTIVO
CREATE   PROCEDURE AgregarActivo
    @Id              AS UNIQUEIDENTIFIER,
    @CodigoFisico    AS VARCHAR(MAX) = NULL,
    @Descripcion     AS VARCHAR(MAX) = NULL,
    @IdCategoria     AS UNIQUEIDENTIFIER,
    @IdUbicacion     AS UNIQUEIDENTIFIER,
    @Estado          AS BIT,
    @Observaciones   AS VARCHAR(MAX) = NULL,
    @UsuarioRegistra AS UNIQUEIDENTIFIER = NULL,
    @FechaRegistro   AS DATETIME = NULL,
    @Marca           AS VARCHAR(100) = NULL,
    @Modelo          AS VARCHAR(100) = NULL,
    @Serie           AS VARCHAR(100) = NULL,
    @Precio          AS DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        INSERT INTO dbo.Activos (
            id, CodigoFisico, Descripcion, IdCategoria, IdUbicacion, 
            Estado, Observaciones, UsuarioRegistra, FechaRegistro,
            Marca, Modelo, Serie, Precio
        )
        VALUES (
            @Id, @CodigoFisico, @Descripcion, @IdCategoria, @IdUbicacion, 
            @Estado, @Observaciones, @UsuarioRegistra, ISNULL(@FechaRegistro, GETDATE()),
            @Marca, @Modelo, @Serie, @Precio
        );
    COMMIT TRANSACTION;

    -- Retorno fundamental para tu ExecuteScalarAsync en C#
    SELECT @Id;
END
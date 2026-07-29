

-- =========================================================
-- MÓDULO: UBICACIONES
-- =========================================================

-- AGREGAR UBICACIÓN
CREATE   PROCEDURE AgregarUbicacion
    @Id       AS UNIQUEIDENTIFIER,
    @Nombre   AS VARCHAR(MAX),
    @TipoArea AS VARCHAR(MAX),
    @Estado   AS BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        INSERT INTO dbo.Ubicaciones (id, Nombre, TipoArea, Estado)
        VALUES (@Id, @Nombre, @TipoArea, @Estado);
    COMMIT TRANSACTION;

    SELECT @Id;
END

-- EDITAR UBICACIÓN
CREATE   PROCEDURE EditarUbicacion
    @Id       AS UNIQUEIDENTIFIER,
    @Nombre   AS VARCHAR(MAX),
    @TipoArea AS VARCHAR(MAX),
    @Estado   AS BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        UPDATE dbo.Ubicaciones
           SET Nombre   = @Nombre,
               TipoArea = @TipoArea,
               Estado   = @Estado
         WHERE id = @Id;
    COMMIT TRANSACTION;

    SELECT @Id;
END
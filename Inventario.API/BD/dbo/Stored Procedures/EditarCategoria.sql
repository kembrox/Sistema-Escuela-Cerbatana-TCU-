
-- EDITAR CATEGORÍA
CREATE   PROCEDURE EditarCategoria
    @Id          AS UNIQUEIDENTIFIER,
    @Nombre      AS VARCHAR(MAX),
    @Descripcion AS VARCHAR(MAX),
    @Estado      AS BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        UPDATE dbo.Categorias
           SET Nombre      = @Nombre,
               Descripcion = @Descripcion,
               Estado      = @Estado
         WHERE id = @Id;
    COMMIT TRANSACTION;

    SELECT @Id;
END
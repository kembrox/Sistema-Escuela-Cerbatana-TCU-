

-- =========================================================
-- MÓDULO: CATEGORÍAS
-- =========================================================

-- AGREGAR CATEGORÍA
CREATE   PROCEDURE AgregarCategoria
    @Id          AS UNIQUEIDENTIFIER,
    @Nombre      AS VARCHAR(MAX),
    @Descripcion AS VARCHAR(MAX),
    @Estado      AS BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        INSERT INTO dbo.Categorias (id, Nombre, Descripcion, Estado)
        VALUES (@Id, @Nombre, @Descripcion, @Estado);
    COMMIT TRANSACTION;

    SELECT @Id;
END
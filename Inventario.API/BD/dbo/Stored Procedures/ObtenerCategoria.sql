
-- OBTENER CATEGORÍA (uno por Id)
CREATE   PROCEDURE ObtenerCategoria
    @Id AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, Nombre, Descripcion, Estado
      FROM dbo.Categorias
     WHERE id = @Id;
END
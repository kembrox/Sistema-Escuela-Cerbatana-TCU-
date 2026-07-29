
-- OBTENER CATEGORÍAS (todos)
CREATE   PROCEDURE ObtenerCategorias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, Nombre, Descripcion, Estado
      FROM dbo.Categorias;
END
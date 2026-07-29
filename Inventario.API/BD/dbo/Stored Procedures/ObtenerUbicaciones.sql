
-- OBTENER UBICACIONES (todos)
CREATE   PROCEDURE ObtenerUbicaciones
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, Nombre, TipoArea, Estado
      FROM dbo.Ubicaciones;
END
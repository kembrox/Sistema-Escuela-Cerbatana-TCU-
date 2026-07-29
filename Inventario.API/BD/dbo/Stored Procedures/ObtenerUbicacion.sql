
-- OBTENER UBICACIÓN (uno por Id)
CREATE   PROCEDURE ObtenerUbicacion
    @Id AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, Nombre, TipoArea, Estado
      FROM dbo.Ubicaciones
     WHERE id = @Id;
END
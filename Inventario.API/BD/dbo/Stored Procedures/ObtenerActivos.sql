
-- OBTENER ACTIVOS (todos)
CREATE   PROCEDURE ObtenerActivos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT A.id AS Id,
           A.CodigoFisico,
           A.Descripcion,
           A.IdCategoria,
           A.IdUbicacion,
           A.Estado,
           A.Observaciones,
           A.UsuarioRegistra,
           A.FechaRegistro,
           A.UsuarioModifica,
           A.FechaModificacion,
           A.Marca,
           A.Modelo,
           A.Serie,
           A.Precio
      FROM dbo.Activos AS A
     ORDER BY A.FechaRegistro DESC;
END
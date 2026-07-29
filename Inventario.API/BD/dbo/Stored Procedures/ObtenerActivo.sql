
-- OBTENER ACTIVO (uno por Id)
CREATE   PROCEDURE ObtenerActivo
    @Id AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT A.id,
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
     WHERE A.id = @Id;
END
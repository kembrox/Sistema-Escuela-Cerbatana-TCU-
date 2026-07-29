
-- =========================================================
-- COMPLEMENTOS MÓDULO: SEGURIDAD (PERFILES Y USUARIOS)
-- =========================================================

-- 1. SP: Obtener Todos los Perfiles del Sistema (Para llenar listas desplegables en la web)
CREATE   PROCEDURE [dbo].[ObtenerPerfiles]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [id], 
           [Nombre]
      FROM [dbo].[Perfiles]
     ORDER BY [Nombre] ASC;
END
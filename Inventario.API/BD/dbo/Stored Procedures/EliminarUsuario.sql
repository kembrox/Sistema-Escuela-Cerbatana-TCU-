
-- 5. SP: Eliminar Usuario (Borrado seguro)
CREATE   PROCEDURE [dbo].[EliminarUsuario]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRAN;
        -- PASO 1: Eliminamos primero sus perfiles asignados en la tabla intermedia
        -- Si no hacemos esto, SQL Server dará un error de llave foránea (Foreign Key Violation)
        DELETE FROM [dbo].[PerfilesxUsuario] 
         WHERE [IdUsuario] = @Id;

        -- PASO 2: Ahora sí podemos eliminar el usuario limpiamente
        DELETE FROM [dbo].[Usuarios] 
         WHERE [id] = @Id;
    COMMIT TRAN;

    -- Retorno estándar para tu ExecuteScalarAsync en Dapper
    SELECT @Id;
END
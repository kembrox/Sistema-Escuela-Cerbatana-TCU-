
-- ELIMINAR ACTIVO
CREATE   PROCEDURE EliminarActivo
    @Id AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
        DELETE FROM dbo.Activos WHERE id = @Id;
    COMMIT TRANSACTION;

    -- Retorno fundamental para tu ExecuteScalarAsync en C#
    SELECT @Id;
END
CREATE TABLE [dbo].[Perfiles] (
    [id]                INT              IDENTITY (1, 1) NOT NULL,
    [Nombre]            VARCHAR (MAX)    NOT NULL,
    [FechaCreacion]     DATETIME         NULL,
    [FechaModificacion] DATETIME         NULL,
    [UsuarioCrea]       UNIQUEIDENTIFIER NULL,
    [UsuarioModifica]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[Usuarios] (
    [id]                UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NombreUsuario]     VARCHAR (MAX)    NOT NULL,
    [PasswordHash]      VARCHAR (MAX)    NOT NULL,
    [CorreoElectronico] VARCHAR (MAX)    NOT NULL,
    [FechaCreacion]     DATETIME         NULL,
    [FechaModificacion] DATETIME         NULL,
    [UsuarioCrea]       UNIQUEIDENTIFIER NULL,
    [UsuarioModifica]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


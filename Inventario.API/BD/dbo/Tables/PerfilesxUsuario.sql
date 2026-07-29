CREATE TABLE [dbo].[PerfilesxUsuario] (
    [IdUsuario] UNIQUEIDENTIFIER NOT NULL,
    [IdPerfil]  INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([IdUsuario] ASC, [IdPerfil] ASC),
    FOREIGN KEY ([IdPerfil]) REFERENCES [dbo].[Perfiles] ([id]),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuarios] ([id])
);


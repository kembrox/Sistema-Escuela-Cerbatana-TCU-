CREATE TABLE [dbo].[Activos] (
    [id]                UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [CodigoFisico]      VARCHAR (MAX)    NULL,
    [Descripcion]       VARCHAR (MAX)    NULL,
    [IdCategoria]       UNIQUEIDENTIFIER NOT NULL,
    [IdUbicacion]       UNIQUEIDENTIFIER NOT NULL,
    [Estado]            BIT              NOT NULL,
    [Observaciones]     VARCHAR (MAX)    NULL,
    [UsuarioRegistra]   UNIQUEIDENTIFIER NULL,
    [FechaRegistro]     DATETIME         NULL,
    [UsuarioModifica]   UNIQUEIDENTIFIER NULL,
    [FechaModificacion] DATETIME         NULL,
    [Marca]             VARCHAR (100)    NULL,
    [Modelo]            VARCHAR (100)    NULL,
    [Serie]             VARCHAR (100)    NULL,
    [Precio]            DECIMAL (18, 2)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([IdCategoria]) REFERENCES [dbo].[Categorias] ([id]),
    FOREIGN KEY ([IdUbicacion]) REFERENCES [dbo].[Ubicaciones] ([id]),
    FOREIGN KEY ([UsuarioModifica]) REFERENCES [dbo].[Usuarios] ([id]),
    FOREIGN KEY ([UsuarioRegistra]) REFERENCES [dbo].[Usuarios] ([id])
);


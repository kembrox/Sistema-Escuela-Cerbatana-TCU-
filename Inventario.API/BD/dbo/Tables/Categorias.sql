CREATE TABLE [dbo].[Categorias] (
    [id]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Nombre]      VARCHAR (MAX)    NOT NULL,
    [Descripcion] VARCHAR (MAX)    NULL,
    [Estado]      BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


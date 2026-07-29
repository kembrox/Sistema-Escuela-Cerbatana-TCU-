CREATE TABLE [dbo].[Ubicaciones] (
    [id]       UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Nombre]   VARCHAR (MAX)    NOT NULL,
    [TipoArea] VARCHAR (MAX)    NULL,
    [Estado]   BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[Aspect] (
    [AspectUID]       VARCHAR (200)  NOT NULL,
    [AspectCode]      VARCHAR (200)  NOT NULL,
    [AspectName]      VARCHAR (200)  NOT NULL,
    [AspectDesc]      VARCHAR (2000) NULL,
    [AspectLabel]     VARCHAR (200)  NOT NULL,
    [CreateTimestamp] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [ModifyTimestamp] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [dbo_Aspect_PK] PRIMARY KEY CLUSTERED ([AspectUID] ASC)
);


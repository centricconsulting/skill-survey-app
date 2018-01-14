CREATE TABLE [dbo].[Aspect] (
    [AspectUID]       NVARCHAR (128)  NOT NULL,
    [AspectName]      NVARCHAR (200)  NOT NULL,
    [AspectCode]      NVARCHAR (200)  NOT NULL,
    [AspectLabel]     NVARCHAR (200)  NOT NULL,
    [AspectDesc]      NVARCHAR (2000) NULL,
    [CreateTimestamp] DATETIME        NOT NULL,
    [ModifyTimestamp] DATETIME        NOT NULL,
    CONSTRAINT [PK_dbo.Aspect] PRIMARY KEY CLUSTERED ([AspectUID] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [dbo_Aspect_U2]
    ON [dbo].[Aspect]([AspectCode] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [dbo_Aspect_U1]
    ON [dbo].[Aspect]([AspectName] ASC);


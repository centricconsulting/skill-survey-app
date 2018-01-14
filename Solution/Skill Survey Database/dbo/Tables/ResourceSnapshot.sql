CREATE TABLE [dbo].[ResourceSnapshot] (
    [ResourceUID]         NVARCHAR (128) NOT NULL,
    [SnapshotTimestamp]   DATETIME       NOT NULL,
    [EmployerDesc]        NVARCHAR (200) NOT NULL,
    [FirstName]           NVARCHAR (200) NULL,
    [LastName]            NVARCHAR (200) NOT NULL,
    [EmailAddress]        NVARCHAR (200) NOT NULL,
    [ResourceLabel]       NVARCHAR (200) NOT NULL,
    [CentricEmployerFlag] TINYINT        NOT NULL,
    [CreateTimestamp]     DATETIME       NOT NULL,
    [ModifyTimestamp]     DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.ResourceSnapshot] PRIMARY KEY CLUSTERED ([ResourceUID] ASC, [SnapshotTimestamp] ASC)
);






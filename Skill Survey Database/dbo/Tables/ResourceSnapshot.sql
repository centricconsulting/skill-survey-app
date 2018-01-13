CREATE TABLE [dbo].[ResourceSnapshot] (
    [ResourceUID]         VARCHAR (200) NOT NULL,
    [SnapshotTimestamp]   VARCHAR (200) NOT NULL,
    [EmployerDesc]        VARCHAR (200) NULL,
    [CentricEmployerFlag] BIT           NOT NULL,
    [FirstName]           VARCHAR (200) NULL,
    [LastName]            VARCHAR (200) NOT NULL,
    [EmailAddress]        VARCHAR (200) NOT NULL,
    [ResourceLabel]       VARCHAR (200) NOT NULL,
    [CreateTimestamp]     DATETIME2 (7) DEFAULT (getdate()) NOT NULL,
    [ModifyTimestamp]     DATETIME2 (7) DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [dbo_ResourceSnapshot_PK] PRIMARY KEY CLUSTERED ([ResourceUID] ASC, [SnapshotTimestamp] ASC)
);




CREATE TABLE [dbo].[EventLog] (
    [EventKey]       INT             IDENTITY (1, 1) NOT NULL,
    [EventTimestamp] DATETIME        NOT NULL,
    [EventType]      NVARCHAR (20)   NOT NULL,
    [EventContext]   NVARCHAR (200)  NOT NULL,
    [EventMessage]   NVARCHAR (2000) NOT NULL,
    CONSTRAINT [PK_dbo.EventLog] PRIMARY KEY CLUSTERED ([EventKey] ASC)
);


CREATE TABLE [dbo].[ResourceSurveySnapshot] (
    [ResourceUID]       VARCHAR (200) NOT NULL,
    [SkillUID]          VARCHAR (200) NOT NULL,
    [AspectUID]         VARCHAR (200) NOT NULL,
    [SnapshotTimestamp] VARCHAR (200) NOT NULL,
    [Ratingvalue]       SMALLINT      NOT NULL,
    [CreateTimestamp]   DATETIME2 (7) DEFAULT (getdate()) NOT NULL,
    [ModifyTimestamp]   DATETIME2 (7) DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [dbo_ResourceSurveySnapshot_PK] PRIMARY KEY CLUSTERED ([ResourceUID] ASC, [SnapshotTimestamp] ASC, [SkillUID] ASC, [AspectUID] ASC)
);


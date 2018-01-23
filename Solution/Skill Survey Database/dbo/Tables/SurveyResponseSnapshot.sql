CREATE TABLE [dbo].[SurveyResponseSnapshot] (
    [ResourceUID]       NVARCHAR (128)  NOT NULL,
    [SkillUID]          NVARCHAR (128)  NOT NULL,
    [AspectUID]         NVARCHAR (128)  NOT NULL,
    [SnapshotTimestamp] DATETIME        NOT NULL,
    [RatingValue]       INT             NOT NULL,
    [OtherSkillInfo]    NVARCHAR (200)  NULL,
    [RespondentInfo]    NVARCHAR (2000) NULL,
    [CreateTimestamp]   DATETIME        NOT NULL,
    [ModifyTimestamp]   DATETIME        NOT NULL,
    CONSTRAINT [PK_dbo.SurveyResponseSnapshot] PRIMARY KEY CLUSTERED ([ResourceUID] ASC, [SkillUID] ASC, [AspectUID] ASC, [SnapshotTimestamp] ASC)
);




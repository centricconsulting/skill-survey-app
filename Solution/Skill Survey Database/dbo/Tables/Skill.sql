CREATE TABLE [dbo].[Skill] (
    [SkillUID]          NVARCHAR (128)  NOT NULL,
    [SkillClassUID]     NVARCHAR (200)  NOT NULL,
    [SkillName]         NVARCHAR (200)  NOT NULL,
    [SkillLabel]        NVARCHAR (200)  NOT NULL,
    [SkillCode]         NVARCHAR (20)   NOT NULL,
    [SkillDescription]  NVARCHAR (2000) NULL,
    [SkillCategoryName] NVARCHAR (200)  NOT NULL,
    [SkillTagList]      NVARCHAR(2000)  NULL,
    [OtherFlag]         TINYINT         NOT NULL,
    [CreateTimestamp]   DATETIME        NOT NULL,
    [ModifyTimestamp]   DATETIME        NOT NULL,
    CONSTRAINT [PK_dbo.Skill] PRIMARY KEY CLUSTERED ([SkillUID] ASC)
);






GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [dbo_SkillSurvey_U2]
    ON [dbo].[Skill]([SkillCode] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [dbo_SkillSurvey_U1]
    ON [dbo].[Skill]([SkillClassUID] ASC, [SkillName] ASC);


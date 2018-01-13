CREATE TABLE [dbo].[Skill] (
    [SkillUID]          VARCHAR (200)  NOT NULL,
    [SkillCode]         VARCHAR (200)  NOT NULL,
    [SkillTypeUID]      VARCHAR (200)  NOT NULL,
    [SkillName]         VARCHAR (2000) NOT NULL,
    [SkillLabel]        VARCHAR (200)  NOT NULL,
    [SkillCategoryName] VARCHAR (200)  NOT NULL,
    [CreateTimestamp]   DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [ModifyTimestamp]   DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [dbo_Skill_PK] PRIMARY KEY CLUSTERED ([SkillUID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [Skill_U2]
    ON [dbo].[Skill]([SkillTypeUID] ASC, [SkillName] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [Skill_U1]
    ON [dbo].[Skill]([SkillCode] ASC);


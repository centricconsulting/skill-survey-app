CREATE TABLE [dbo].[AspectRating] (
    [AspectUID]         NVARCHAR (128)  NOT NULL,
    [RatingValue]       INT             NOT NULL,
    [ScaledRatingValue] INT             NOT NULL,
    [RatingName]        NVARCHAR (200)  NOT NULL,
    [RatingLabel]       NVARCHAR (200)  NOT NULL,
    [RatingDesc]        NVARCHAR (2000) NULL,
    [CreateTimestamp]   DATETIME        NOT NULL,
    [ModifyTimestamp]   DATETIME        NOT NULL,
    CONSTRAINT [PK_dbo.AspectRating] PRIMARY KEY CLUSTERED ([AspectUID] ASC, [RatingValue] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [dbo_Rating_U1]
    ON [dbo].[AspectRating]([AspectUID] ASC, [RatingName] ASC);


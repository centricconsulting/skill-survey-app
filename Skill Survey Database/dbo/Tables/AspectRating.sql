CREATE TABLE [dbo].[AspectRating] (
    [AspectUID]       VARCHAR (200)  NOT NULL,
    [RatingValue]     SMALLINT       NOT NULL,
    [RatingName]      VARCHAR (200)  NOT NULL,
    [RatingDesc]      VARCHAR (2000) NOT NULL,
    [RatingLabel]     VARCHAR (200)  NOT NULL,
    [CreateTimestamp] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [ModifyTimestamp] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [dbo_AspectRating_PK] PRIMARY KEY CLUSTERED ([AspectUID] ASC, [RatingValue] ASC)
);


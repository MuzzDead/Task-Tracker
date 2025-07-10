CREATE TABLE [Comments]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Text]              NVARCHAR(MAX) NOT NULL,
    [CardId]            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Cards(Id) ON DELETE CASCADE,
    [UserId]            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    [CreatedAt]         DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [UpdatedAt]         DATETIME                     DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                DEFAULT NULL
)

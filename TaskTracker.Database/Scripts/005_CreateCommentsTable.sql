CREATE TABLE [Comments]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Text]              NVARCHAR(MAX) NOT NULL,
    [CardId]            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Cards(Id) ON DELETE CASCADE,
    [UserId]            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    [CreatedAtUtc]      DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [LastModifiedAtUtc] DATETIME                     DEFAULT NULL,
    [LastModifiedBy]    NVARCHAR(100)                DEFAULT NULL
)
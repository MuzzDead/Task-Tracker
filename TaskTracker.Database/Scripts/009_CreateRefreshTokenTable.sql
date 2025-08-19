CREATE TABLE [RefreshTokens] (
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [UserId]            UNIQUEIDENTIFIER NOT NULL,
    [Token]             NVARCHAR(200)    NOT NULL,
    [ExpiresAt]         DATETIMEOFFSET   NOT NULL,
    [CreatedAt]         DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [CreatedByIp]       NVARCHAR(50)     NULL,
    [RevokedAt]         DATETIMEOFFSET   NULL,
    [RevokedByIp]       NVARCHAR(50)     NULL,
    [ReplacedByToken]   NVARCHAR(200)    NULL,
    
    CONSTRAINT [FK_RefreshTokens_Users_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

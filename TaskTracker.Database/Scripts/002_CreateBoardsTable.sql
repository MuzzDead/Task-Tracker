CREATE TABLE [Boards]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [Description]       NVARCHAR(MAX),
    [CreatedAtUtc]      DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [LastModifiedAtUtc] DATETIME                     DEFAULT NULL,
    [LastModifiedBy]    NVARCHAR(100)                DEFAULT NULL,
    [IsArchived]        BIT           NOT NULL       DEFAULT 0,
    [ArchivedAtUtc]     DATETIME                     DEFAULT NULL
)
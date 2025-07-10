CREATE TABLE [Boards]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [Description]       NVARCHAR(MAX),
    [CreatedAt]         DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [UpdatedAt]         DATETIME                     DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                DEFAULT NULL,
    [IsArchived]        BIT           NOT NULL       DEFAULT 0,
    [ArchivedAt]        DATETIME                     DEFAULT NULL
)
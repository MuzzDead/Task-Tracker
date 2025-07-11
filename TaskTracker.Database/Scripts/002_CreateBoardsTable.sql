CREATE TABLE [Boards]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [Description]       NVARCHAR(MAX),
    [CreatedAt]         DATETIMEOFFSET      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [UpdatedAt]         DATETIMEOFFSET               DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                DEFAULT NULL,
    [IsArchived]        BIT           NOT NULL       DEFAULT 0,
    [ArchivedAt]        DATETIMEOFFSET                     DEFAULT NULL
)
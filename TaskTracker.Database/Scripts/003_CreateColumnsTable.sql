CREATE TABLE [Columns]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [ColumnIndex]       INT           NOT NULL,
    [BoardId]           UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Boards(Id) ON DELETE CASCADE,
    [CreatedAtUtc]      DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [LastModifiedAtUtc] DATETIME                     DEFAULT NULL,
    [LastModifiedBy]    NVARCHAR(100)                DEFAULT NULL,
    
    CONSTRAINT [UQ_Columns_BoardId_ColumnIndex] UNIQUE ([BoardId], [ColumnIndex])
)
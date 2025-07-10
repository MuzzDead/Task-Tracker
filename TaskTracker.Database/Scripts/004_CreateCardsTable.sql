CREATE TABLE [Cards]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [ColumnId]          UNIQUEIDENTIFIER,
    [RowIndex]          INT           NOT NULL       DEFAULT 0,
    [CreatedAtUtc]      DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [LastModifiedAtUtc] DATETIME                     DEFAULT NULL,
    [LastModifiedBy]    NVARCHAR(100)                DEFAULT NULL,
    
    CONSTRAINT [UQ_Cards_ColumnId_RowIndex] UNIQUE ([ColumnId], [RowIndex])
)

ALTER TABLE Cards
    ADD CONSTRAINT FK_Cards_Columns
        FOREIGN KEY (ColumnId) REFERENCES Columns(Id) ON DELETE CASCADE
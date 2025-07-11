CREATE TABLE [Cards]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Title]             NVARCHAR(255) NOT NULL,
    [ColumnId]          UNIQUEIDENTIFIER,
    [RowIndex]          INT           NOT NULL       DEFAULT 0,
    [CreatedAt]         DATETIMEOFFSET NOT NULL       DEFAULT SYSDATETIMEOFFSET(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [UpdatedAt]         DATETIMEOFFSET                DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                DEFAULT NULL,

    CONSTRAINT [UQ_Cards_ColumnId_RowIndex] UNIQUE ([ColumnId], [RowIndex])
)

ALTER TABLE Cards
    ADD CONSTRAINT FK_Cards_Columns
        FOREIGN KEY (ColumnId) REFERENCES Columns(Id) ON DELETE CASCADE
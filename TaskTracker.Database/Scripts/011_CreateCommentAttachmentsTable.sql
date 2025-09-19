CREATE TABLE [CommentAttachments]
(
    [Id]                UNIQUEIDENTIFIER    PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [CommentId]         UNIQUEIDENTIFIER    FOREIGN KEY REFERENCES Comments(Id) ON DELETE CASCADE,
    [BlobId]            UNIQUEIDENTIFIER    NOT NULL,
    [FileName]          NVARCHAR(255)       NOT NULL,
    [ContentType]       NVARCHAR(100)       NOT NULL,
    [Size]              BIGINT              NOT NULL,
    [CreatedAt]         DATETIMEOFFSET      NOT NULL       DEFAULT SYSDATETIMEOFFSET(),
    [CreatedBy]         NVARCHAR(100)       NOT NULL,
    [UpdatedAt]         DATETIMEOFFSET                      DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                      DEFAULT NULL
);

CREATE INDEX IX_CommentAttachments_CommentId
    ON [CommentAttachments]([CommentId]);

CREATE INDEX IX_CommentAttachments_BlobId
    ON [CommentAttachments]([BlobId]);

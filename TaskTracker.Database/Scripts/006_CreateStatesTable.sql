CREATE TABLE [States]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Description]       NVARCHAR(MAX) NOT NULL,
    [Status]            INT           NOT NULL       DEFAULT 1, -- 1=Pending, 2=InProgress, 3=Completed, 4=Cancelled
    [Priority]          INT           NOT NULL       DEFAULT 2, -- 1=Low, 2=Medium, 3=High, 4=Critical
    [CardId]            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Cards(Id) ON DELETE CASCADE,
    [CreatedAtUtc]      DATETIME      NOT NULL       DEFAULT GETUTCDATE(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [LastModifiedAtUtc] DATETIME                     DEFAULT NULL,
    [LastModifiedBy]    NVARCHAR(100)                DEFAULT NULL,
    
    CONSTRAINT [CK_States_Status] CHECK ([Status] IN (1, 2, 3, 4)),
    CONSTRAINT [CK_States_Priority] CHECK ([Priority] IN (1, 2, 3, 4))
)
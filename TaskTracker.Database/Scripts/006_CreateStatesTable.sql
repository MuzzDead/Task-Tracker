CREATE TABLE [States]
(
    [Id]                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [Description]       NVARCHAR(MAX) NOT NULL,
    [IsCompleted]       BIT           NOT NULL       DEFAULT 0, -- 0=false, 1=true
    [Priority]          INT           NOT NULL       DEFAULT 1, -- 1=Low, 2=Medium, 3=High, 4=Critical
    [Deadline]          DATETIMEOFFSET               DEFAULT NULL,
    [AssigneeId]        UNIQUEIDENTIFIER             DEFAULT NULL,
    [CardId]            UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt]         DATETIMEOFFSET NOT NULL       DEFAULT SYSDATETIMEOFFSET(),
    [CreatedBy]         NVARCHAR(100) NOT NULL,
    [UpdatedAt]         DATETIMEOFFSET               DEFAULT NULL,
    [UpdatedBy]         NVARCHAR(100)                DEFAULT NULL,
    
    CONSTRAINT [FK_States_Cards] 
        FOREIGN KEY ([CardId]) REFERENCES [Cards]([Id]) ON DELETE CASCADE,
    
    CONSTRAINT [FK_States_Assignees] 
        FOREIGN KEY ([AssigneeId]) REFERENCES [Users]([Id]) ON DELETE SET NULL,
    
    CONSTRAINT [CK_States_Priority] 
        CHECK ([Priority] IN (1, 2, 3, 4)) -- Low, Medium, High, Critical
);
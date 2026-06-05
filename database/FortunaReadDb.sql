USE master
GO

IF DB_ID(N'FortunaReadDb') IS NULL
BEGIN
    CREATE DATABASE [FortunaReadDb];
END
GO

USE [FortunaReadDb];
GO


IF OBJECT_ID(N'[dbo].[ProductTile]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProductTile]
    (
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [ProductCategory] NVARCHAR(50) NOT NULL,
        [ProductType] NVARCHAR(50) NOT NULL,
        [ProductName] NVARCHAR(200) NOT NULL,
        [ProductNumber] NVARCHAR(64) NOT NULL,
        [Balance] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(3) NOT NULL,
        CONSTRAINT [PKreadProductTile] PRIMARY KEY CLUSTERED ([ProductId] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[TimelineEvent]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TimelineEvent]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [ProductId] UNIQUEIDENTIFIER NULL,
        [EventDateUtc] DATETIME2(7) NOT NULL,
        [EventType] NVARCHAR(100) NOT NULL,
        [Title] NVARCHAR(300) NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(3) NOT NULL,
        [IsPositive] BIT NOT NULL,
        CONSTRAINT [PKreadTimelineEvent] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[ProcessedOutboxMessage]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProcessedOutboxMessage]
    (
        [OutboxMessageId] UNIQUEIDENTIFIER NOT NULL,
        [ProcessedAtUtc] DATETIME2(7) NOT NULL,
        CONSTRAINT [PKreadProcessedOutboxMessage] PRIMARY KEY CLUSTERED ([OutboxMessageId] ASC)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXreadProductTileCustomerId'
      AND OBJECT_ID = OBJECT_ID(N'[dbo].[ProductTile]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXreadProductTileCustomerId]
        ON [dbo].[ProductTile] ([CustomerId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXreadTimelineEventCustomerId'
      AND OBJECT_ID = OBJECT_ID(N'[dbo].[TimelineEvent]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXreadTimelineEventCustomerId]
        ON [dbo].[TimelineEvent] ([CustomerId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXreadTimelineEventEventDateUtc'
      AND OBJECT_ID = OBJECT_ID(N'[dbo].[TimelineEvent]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXreadTimelineEventEventDateUtc]
        ON [dbo].[TimelineEvent] ([EventDateUtc] DESC);
END
GO

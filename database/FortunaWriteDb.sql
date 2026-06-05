USE master
GO

IF DB_ID(N'FortunaWriteDb') IS NULL
BEGIN
    CREATE DATABASE [FortunaWriteDb];
END
GO

USE [FortunaWriteDb];
GO

IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Customers]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [PasswordHash] NVARCHAR(512) NOT NULL,
        [CustomerType] INT NOT NULL,
        [CreatedAtUtc] DATETIME2(7) NOT NULL,
        CONSTRAINT [PK_dbo_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF COL_LENGTH(N'[dbo].[Customers]', N'CustomerType') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ADD [CustomerType] INT NOT NULL CONSTRAINT [DF_Customers_CustomerType] DEFAULT (1);
END
GO

IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Products]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [Discriminator] NVARCHAR(50) NOT NULL,
        [ProductCategory] INT NOT NULL,
        [ProductName] NVARCHAR(200) NOT NULL,
        [ProductNumber] NVARCHAR(64) NOT NULL,
        [NumberSequence] BIGINT NOT NULL,
        [AccountNumber] NVARCHAR(34) NULL,
        [BankAccountType] INT NULL,
        [CardType] INT NULL,
        [CreditLimit] DECIMAL(18, 2) NULL,
        [LoanType] INT NULL,
        [Balance] DECIMAL(18, 2) NOT NULL,
        [BalanceCurrency] NVARCHAR(3) NOT NULL,
        [Currency] NVARCHAR(3) NOT NULL,
        [Status] INT NOT NULL,
        [CreatedAtUtc] DATETIME2(7) NOT NULL,
        CONSTRAINT [PKProducts] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FKProductsCustomers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id])
    );
END
GO

IF COL_LENGTH(N'[dbo].[Products]', N'CreditLimit') IS NULL
BEGIN
    ALTER TABLE [dbo].[Products]
    ADD [CreditLimit] DECIMAL(18, 2) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[Products]', N'NumberSequence') IS NULL
BEGIN
    ALTER TABLE [dbo].[Products]
    ADD [NumberSequence] BIGINT NULL;
END
GO

DECLARE @CurrentMaxNumberSequence BIGINT;

SELECT @CurrentMaxNumberSequence = ISNULL(MAX([NumberSequence]), 0)
FROM [dbo].[Products];

;WITH [ProductSequenceCte] AS
(
    SELECT
        [Id],
        @CurrentMaxNumberSequence + ROW_NUMBER() OVER (ORDER BY [CreatedAtUtc], [Id]) AS [SequenceValue]
    FROM [dbo].[Products]
    WHERE [NumberSequence] IS NULL
)
UPDATE [Products]
SET [Products].[NumberSequence] = [ProductSequenceCte].[SequenceValue]
FROM [dbo].[Products] AS [Products]
INNER JOIN [ProductSequenceCte]
    ON [ProductSequenceCte].[Id] = [Products].[Id];
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Products]')
      AND name = N'NumberSequence'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE [dbo].[Products]
    ALTER COLUMN [NumberSequence] BIGINT NOT NULL;
END
GO

IF OBJECT_ID(N'[dbo].[Transactions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Transactions]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [BankAccountId] UNIQUEIDENTIFIER NOT NULL,
        [TransferId] UNIQUEIDENTIFIER NULL,
        [Type] INT NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(3) NOT NULL,
        [Title] NVARCHAR(300) NOT NULL,
        [BookedAtUtc] DATETIME2(7) NOT NULL,
        CONSTRAINT [PKTransactions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FKTransactionsProducts] FOREIGN KEY ([BankAccountId]) REFERENCES [dbo].[Products]([Id])
    );
END
GO

IF OBJECT_ID(N'[dbo].[Transfers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Transfers]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [TransferType] INT NOT NULL CONSTRAINT [DFTransfersTransferType] DEFAULT (1),
        [SourceAccountId] UNIQUEIDENTIFIER NOT NULL,
        [TargetAccountId] UNIQUEIDENTIFIER NULL,
        [ExternalTargetAccountNumber] NVARCHAR(34) NULL,
        [ExternalRecipientName] NVARCHAR(200) NULL,
        [Amount] DECIMAL(18, 2) NOT NULL,
        [Currency] NVARCHAR(3) NOT NULL,
        [Title] NVARCHAR(300) NOT NULL,
        [Status] INT NOT NULL,
        [CreatedAtUtc] DATETIME2(7) NOT NULL,
        [CompletedAtUtc] DATETIME2(7) NULL,
        CONSTRAINT [PKTransfers] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FKTransfersSourceProduct] FOREIGN KEY ([SourceAccountId]) REFERENCES [dbo].[Products]([Id]),
        CONSTRAINT [FKTransfersTargetProduct] FOREIGN KEY ([TargetAccountId]) REFERENCES [dbo].[Products]([Id])
    );
END
GO

IF COL_LENGTH(N'[dbo].[Transfers]', N'TransferType') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transfers]
    ADD [TransferType] INT NOT NULL CONSTRAINT [DFTransfersTransferType_Existing] DEFAULT (1);
END
GO

IF COL_LENGTH(N'[dbo].[Transfers]', N'ExternalTargetAccountNumber') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transfers]
    ADD [ExternalTargetAccountNumber] NVARCHAR(34) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[Transfers]', N'ExternalRecipientName') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transfers]
    ADD [ExternalRecipientName] NVARCHAR(200) NULL;
END
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Transfers]')
      AND name = N'TargetAccountId'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE [dbo].[Transfers]
    ALTER COLUMN [TargetAccountId] UNIQUEIDENTIFIER NULL;
END
GO

IF OBJECT_ID(N'[dbo].[OutboxMessages]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OutboxMessages]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Type] NVARCHAR(500) NOT NULL,
        [Payload] NVARCHAR(MAX) NOT NULL,
        [OccurredOnUtc] DATETIME2(7) NOT NULL,
        [ProcessedOnUtc] DATETIME2(7) NULL,
        [Error] NVARCHAR(MAX) NULL,
        CONSTRAINT [PKOutboxMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXCustomersEmail'
      AND object_id = OBJECT_ID(N'[dbo].[Customers]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IXCustomersEmail]
        ON [dbo].[Customers] ([Email] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXProductsCustomerId'
      AND object_id = OBJECT_ID(N'[dbo].[Products]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXProductsCustomerId]
        ON [dbo].[Products] ([CustomerId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXProductsProductNumber'
      AND object_id = OBJECT_ID(N'[dbo].[Products]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IXProductsProductNumber]
        ON [dbo].[Products] ([ProductNumber] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXProductsNumberSequence'
      AND object_id = OBJECT_ID(N'[dbo].[Products]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IXProductsNumberSequence]
        ON [dbo].[Products] ([NumberSequence] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXTransactionsBankAccountId'
      AND object_id = OBJECT_ID(N'[dbo].[Transactions]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXTransactionsBankAccountId]
        ON [dbo].[Transactions] ([BankAccountId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXTransactionsTransferId'
      AND object_id = OBJECT_ID(N'[dbo].[Transactions]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXTransactionsTransferId]
        ON [dbo].[Transactions] ([TransferId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXTransfersSourceAccountId'
      AND object_id = OBJECT_ID(N'[dbo].[Transfers]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXTransfersSourceAccountId]
        ON [dbo].[Transfers] ([SourceAccountId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXTransfersTargetAccountId'
      AND object_id = OBJECT_ID(N'[dbo].[Transfers]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXTransfersTargetAccountId]
        ON [dbo].[Transfers] ([TargetAccountId] ASC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IXOutboxMessagesProcessedOnUtc'
      AND object_id = OBJECT_ID(N'[dbo].[OutboxMessages]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IXOutboxMessagesProcessedOnUtc]
        ON [dbo].[OutboxMessages] ([ProcessedOnUtc] ASC);
END
GO

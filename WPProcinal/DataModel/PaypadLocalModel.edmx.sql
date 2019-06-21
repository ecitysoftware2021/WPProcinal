
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/11/2019 13:46:07
-- Generated from EDMX file: C:\Users\H81\Documents\GitHub\WPFDavivienda\WPFDavivienda\WPFDavivienda\DataModel\PaypadLocalModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DB_Local];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_DESCRIPTION_TRANSACTION_TRANSACTION]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TRANSACTION_DESCRIPTION] DROP CONSTRAINT [FK_DESCRIPTION_TRANSACTION_TRANSACTION];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CONFIGURATION_PAYDAD]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CONFIGURATION_PAYDAD];
GO
IF OBJECT_ID(N'[dbo].[DEVICE_LOG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DEVICE_LOG];
GO
IF OBJECT_ID(N'[dbo].[ERROR_LOG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ERROR_LOG];
GO
IF OBJECT_ID(N'[dbo].[PAYER]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PAYER];
GO
IF OBJECT_ID(N'[dbo].[PAYPAD_ACTION_LOG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PAYPAD_ACTION_LOG];
GO
IF OBJECT_ID(N'[dbo].[PAYPAD_CONSOLE_ERROR]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PAYPAD_CONSOLE_ERROR];
GO
IF OBJECT_ID(N'[dbo].[PAYPAD_LOG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PAYPAD_LOG];
GO
IF OBJECT_ID(N'[dbo].[TRANSACTION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TRANSACTION];
GO
IF OBJECT_ID(N'[dbo].[TRANSACTION_DESCRIPTION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TRANSACTION_DESCRIPTION];
GO
IF OBJECT_ID(N'[dbo].[TRANSACTION_DETAIL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TRANSACTION_DETAIL];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CONFIGURATION_PAYDAD'
CREATE TABLE [dbo].[CONFIGURATION_PAYDAD] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [USER_API] nvarchar(max)  NULL,
    [PASSWORD_API] nvarchar(max)  NULL,
    [USER] nvarchar(max)  NULL,
    [PASSWORD] nvarchar(max)  NULL,
    [ID_SESSION] int  NULL,
    [ID_PAYPAD] int  NULL,
    [ID_SITE] varchar(max)  NULL,
    [TOKEN_API] nvarchar(max)  NULL,
    [TYPE] int  NULL
);
GO

-- Creating table 'DEVICE_LOG'
CREATE TABLE [dbo].[DEVICE_LOG] (
    [DEVICE_LOG_ID] int IDENTITY(1,1) NOT NULL,
    [TRANSACTION_ID] int  NULL,
    [DESCRIPTION] varchar(max)  NOT NULL,
    [DATETIME] datetime  NOT NULL,
    [CODE] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ERROR_LOG'
CREATE TABLE [dbo].[ERROR_LOG] (
    [ERROR_LOG_ID] int IDENTITY(1,1) NOT NULL,
    [NAME_CLASS] nvarchar(max)  NULL,
    [NAME_FUNCTION] nvarchar(max)  NULL,
    [MESSAGE_ERROR] nvarchar(max)  NULL,
    [DESCRIPTION] nvarchar(max)  NULL,
    [DATE] datetime  NULL,
    [TYPE] int  NOT NULL,
    [STATE] bit  NULL
);
GO

-- Creating table 'PAYER'
CREATE TABLE [dbo].[PAYER] (
    [PAYER_ID] int IDENTITY(1,1) NOT NULL,
    [IDENTIFICATION] nvarchar(50)  NULL,
    [NAME] nvarchar(max)  NULL,
    [LAST_NAME] nvarchar(max)  NULL,
    [PHONE] decimal(18,0)  NULL,
    [EMAIL] nvarchar(max)  NULL,
    [ADDRESS] nvarchar(max)  NULL,
    [STATE] bit  NOT NULL
);
GO

-- Creating table 'PAYPAD_ACTION_LOG'
CREATE TABLE [dbo].[PAYPAD_ACTION_LOG] (
    [PAYPAD_ACTION_LOG_ID] int IDENTITY(1,1) NOT NULL,
    [ACTION_LOG_ID] int  NOT NULL,
    [DEVICE_PAYPAD_ID] int  NULL,
    [DESCRIPTION] nvarchar(max)  NOT NULL,
    [DATE_EXECUTE] datetime  NOT NULL,
    [QUANTITY_INTENTS] int  NOT NULL,
    [INTENTS] int  NOT NULL,
    [STATE] int  NOT NULL
);
GO

-- Creating table 'PAYPAD_CONSOLE_ERROR'
CREATE TABLE [dbo].[PAYPAD_CONSOLE_ERROR] (
    [PAYPAD_CONSOLE_ERROR_ID] int IDENTITY(1,1) NOT NULL,
    [PAYPAD_ID] int  NOT NULL,
    [ERROR_ID] int  NOT NULL,
    [ERROR_LEVEL_ID] int  NOT NULL,
    [DEVICE_PAYPAD_ID] int  NULL,
    [DESCRIPTION] nvarchar(max)  NOT NULL,
    [DATE] datetime  NOT NULL,
    [OBSERVATION] nvarchar(max)  NOT NULL,
    [STATE] bit  NOT NULL
);
GO

-- Creating table 'PAYPAD_LOG'
CREATE TABLE [dbo].[PAYPAD_LOG] (
    [PAYPAD_LOG_ID] int IDENTITY(1,1) NOT NULL,
    [REFERENCE] nvarchar(max)  NULL,
    [DESCRIPTION] nvarchar(max)  NOT NULL,
    [STATE] bit  NOT NULL
);
GO

-- Creating table 'TRANSACTION'
CREATE TABLE [dbo].[TRANSACTION] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TRANSACTION_ID] int  NULL,
    [PAYPAD_ID] int  NOT NULL,
    [TYPE_TRANSACTION_ID] int  NOT NULL,
    [DATE_BEGIN] datetime  NOT NULL,
    [DATE_END] datetime  NULL,
    [TOTAL_AMOUNT] decimal(18,0)  NOT NULL,
    [INCOME_AMOUNT] decimal(18,0)  NULL,
    [RETURN_AMOUNT] decimal(18,0)  NULL,
    [DESCRIPTION] nvarchar(max)  NULL,
    [PAYER_ID] int  NOT NULL,
    [STATE_TRANSACTION_ID] int  NOT NULL,
    [STATE_NOTIFICATION] bit  NOT NULL
);
GO

-- Creating table 'TRANSACTION_DESCRIPTION'
CREATE TABLE [dbo].[TRANSACTION_DESCRIPTION] (
    [TRANSACTION_DESCRIPTION_ID] int IDENTITY(1,1) NOT NULL,
    [TRANSACTION_ID] int  NOT NULL,
    [REFERENCE] nvarchar(50)  NOT NULL,
    [AMOUNT] decimal(18,0)  NULL,
    [OBSERVATION] nvarchar(max)  NULL,
    [STATE] bit  NULL
);
GO

-- Creating table 'TRANSACTION_DETAIL'
CREATE TABLE [dbo].[TRANSACTION_DETAIL] (
    [TRANSACTION_DETAIL_ID] int IDENTITY(1,1) NOT NULL,
    [TRANSACTION_ID] int  NOT NULL,
    [CODE] nvarchar(max)  NOT NULL,
    [DENOMINATION] int  NULL,
    [OPERATION] int  NULL,
    [QUANTITY] int  NULL,
    [DESCRIPTION] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'CONFIGURATION_PAYDAD'
ALTER TABLE [dbo].[CONFIGURATION_PAYDAD]
ADD CONSTRAINT [PK_CONFIGURATION_PAYDAD]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [DEVICE_LOG_ID] in table 'DEVICE_LOG'
ALTER TABLE [dbo].[DEVICE_LOG]
ADD CONSTRAINT [PK_DEVICE_LOG]
    PRIMARY KEY CLUSTERED ([DEVICE_LOG_ID] ASC);
GO

-- Creating primary key on [ERROR_LOG_ID] in table 'ERROR_LOG'
ALTER TABLE [dbo].[ERROR_LOG]
ADD CONSTRAINT [PK_ERROR_LOG]
    PRIMARY KEY CLUSTERED ([ERROR_LOG_ID] ASC);
GO

-- Creating primary key on [PAYER_ID] in table 'PAYER'
ALTER TABLE [dbo].[PAYER]
ADD CONSTRAINT [PK_PAYER]
    PRIMARY KEY CLUSTERED ([PAYER_ID] ASC);
GO

-- Creating primary key on [PAYPAD_ACTION_LOG_ID] in table 'PAYPAD_ACTION_LOG'
ALTER TABLE [dbo].[PAYPAD_ACTION_LOG]
ADD CONSTRAINT [PK_PAYPAD_ACTION_LOG]
    PRIMARY KEY CLUSTERED ([PAYPAD_ACTION_LOG_ID] ASC);
GO

-- Creating primary key on [PAYPAD_CONSOLE_ERROR_ID] in table 'PAYPAD_CONSOLE_ERROR'
ALTER TABLE [dbo].[PAYPAD_CONSOLE_ERROR]
ADD CONSTRAINT [PK_PAYPAD_CONSOLE_ERROR]
    PRIMARY KEY CLUSTERED ([PAYPAD_CONSOLE_ERROR_ID] ASC);
GO

-- Creating primary key on [PAYPAD_LOG_ID] in table 'PAYPAD_LOG'
ALTER TABLE [dbo].[PAYPAD_LOG]
ADD CONSTRAINT [PK_PAYPAD_LOG]
    PRIMARY KEY CLUSTERED ([PAYPAD_LOG_ID] ASC);
GO

-- Creating primary key on [ID] in table 'TRANSACTION'
ALTER TABLE [dbo].[TRANSACTION]
ADD CONSTRAINT [PK_TRANSACTION]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [TRANSACTION_DESCRIPTION_ID] in table 'TRANSACTION_DESCRIPTION'
ALTER TABLE [dbo].[TRANSACTION_DESCRIPTION]
ADD CONSTRAINT [PK_TRANSACTION_DESCRIPTION]
    PRIMARY KEY CLUSTERED ([TRANSACTION_DESCRIPTION_ID] ASC);
GO

-- Creating primary key on [TRANSACTION_DETAIL_ID] in table 'TRANSACTION_DETAIL'
ALTER TABLE [dbo].[TRANSACTION_DETAIL]
ADD CONSTRAINT [PK_TRANSACTION_DETAIL]
    PRIMARY KEY CLUSTERED ([TRANSACTION_DETAIL_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TRANSACTION_ID] in table 'TRANSACTION_DESCRIPTION'
ALTER TABLE [dbo].[TRANSACTION_DESCRIPTION]
ADD CONSTRAINT [FK_DESCRIPTION_TRANSACTION_TRANSACTION]
    FOREIGN KEY ([TRANSACTION_ID])
    REFERENCES [dbo].[TRANSACTION]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DESCRIPTION_TRANSACTION_TRANSACTION'
CREATE INDEX [IX_FK_DESCRIPTION_TRANSACTION_TRANSACTION]
ON [dbo].[TRANSACTION_DESCRIPTION]
    ([TRANSACTION_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
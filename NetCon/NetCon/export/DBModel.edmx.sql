
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/11/2020 15:32:14
-- Generated from EDMX file: C:\Users\Ola\Desktop\Witek\Ethernet Tap\EthernetTap\NetCon\NetCon\export\DBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EthernetTapDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[TargetNameSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TargetNameSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TargetNameSet'
CREATE TABLE [dbo].[TargetNameSet] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [creationDate] datetime  NOT NULL,
    [modifyDate] datetime  NOT NULL
);
GO

-- Creating table 'TargetSet'
CREATE TABLE [dbo].[TargetSet] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [date] datetime  NOT NULL,
    [rawData] varbinary(max)  NOT NULL,
    [dataType] int  NOT NULL,
    [arraySize] int  NOT NULL,
    [TargetName_id] bigint  NOT NULL
);
GO

-- Creating table 'VariableEventSet'
CREATE TABLE [dbo].[VariableEventSet] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [date] datetime  NOT NULL,
    [thresholdValue] varbinary(max)  NOT NULL,
    [thresholdValue2] varbinary(max)  NOT NULL,
    [thresholdType] int  NOT NULL,
    [thresholdDataType] int  NOT NULL,
    [Target_id] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'TargetNameSet'
ALTER TABLE [dbo].[TargetNameSet]
ADD CONSTRAINT [PK_TargetNameSet]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'TargetSet'
ALTER TABLE [dbo].[TargetSet]
ADD CONSTRAINT [PK_TargetSet]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'VariableEventSet'
ALTER TABLE [dbo].[VariableEventSet]
ADD CONSTRAINT [PK_VariableEventSet]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TargetName_id] in table 'TargetSet'
ALTER TABLE [dbo].[TargetSet]
ADD CONSTRAINT [FK_TargetNameTarget]
    FOREIGN KEY ([TargetName_id])
    REFERENCES [dbo].[TargetNameSet]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TargetNameTarget'
CREATE INDEX [IX_FK_TargetNameTarget]
ON [dbo].[TargetSet]
    ([TargetName_id]);
GO

-- Creating foreign key on [Target_id] in table 'VariableEventSet'
ALTER TABLE [dbo].[VariableEventSet]
ADD CONSTRAINT [FK_TargetVariableEvent]
    FOREIGN KEY ([Target_id])
    REFERENCES [dbo].[TargetSet]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TargetVariableEvent'
CREATE INDEX [IX_FK_TargetVariableEvent]
ON [dbo].[VariableEventSet]
    ([Target_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
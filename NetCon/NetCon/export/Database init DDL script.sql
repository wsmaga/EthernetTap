
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/06/2020 23:05:16
-- Generated from EDMX file: C:\Users\Wiciu\Documents\Reposy z gita\EthernetTap\NetCon\NetCon\export\Database design EDM.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Test local MSSQL database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ByteMeasurementSet'
CREATE TABLE [dbo].[ByteMeasurementSet] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MeasurementID] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Value] tinyint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'ByteMeasurementSet'
ALTER TABLE [dbo].[ByteMeasurementSet]
ADD CONSTRAINT [PK_ByteMeasurementSet]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
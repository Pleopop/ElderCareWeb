-- =============================================
-- AI Matching System - Database Schema Update
-- =============================================
-- This script adds the MatchingResults table and updates BeneficiaryPreferences
-- Run this after the initial database setup

USE ElderCareDB;
GO

-- =============================================
-- 1. Update BeneficiaryPreferences table
-- =============================================
-- Add new columns for matching preferences
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BeneficiaryPreferences]') AND name = 'RequiredSkills')
BEGIN
    ALTER TABLE [dbo].[BeneficiaryPreferences]
    ADD [RequiredSkills] NVARCHAR(500) NULL,
        [PreferredPersonalityTraits] NVARCHAR(500) NULL,
        [PreferredGender] NVARCHAR(50) NULL,
        [MinAge] INT NULL,
        [MaxAge] INT NULL;
    
    PRINT 'Added matching preference columns to BeneficiaryPreferences';
END
ELSE
BEGIN
    PRINT 'BeneficiaryPreferences already has matching columns';
END
GO

-- =============================================
-- 2. Create MatchingResults table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MatchingResults')
BEGIN
    CREATE TABLE [dbo].[MatchingResults] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [BeneficiaryId] UNIQUEIDENTIFIER NOT NULL,
        [CaregiverProfileId] UNIQUEIDENTIFIER NOT NULL,
        
        -- Scores (0-100 scale, using FLOAT for double precision)
        [OverallScore] FLOAT NOT NULL,
        [PersonalityScore] FLOAT NOT NULL,
        [SkillScore] FLOAT NOT NULL,
        [AvailabilityScore] FLOAT NOT NULL,
        [LocationScore] FLOAT NOT NULL,
        [PerformanceScore] FLOAT NOT NULL,
        
        -- Metadata
        [CalculatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        -- BaseEntity properties
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(256) NULL,
        [UpdatedBy] NVARCHAR(256) NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        
        -- Foreign Keys
        CONSTRAINT [FK_MatchingResults_Beneficiaries] 
            FOREIGN KEY ([BeneficiaryId]) 
            REFERENCES [dbo].[Beneficiaries]([Id]) 
            ON DELETE CASCADE,
            
        CONSTRAINT [FK_MatchingResults_CaregiverProfiles] 
            FOREIGN KEY ([CaregiverProfileId]) 
            REFERENCES [dbo].[CaregiverProfiles]([Id]) 
            ON DELETE NO ACTION -- Prevent cascade delete conflicts
    );
    
    -- Create indexes for performance
    CREATE INDEX [IX_MatchingResults_BeneficiaryId] 
        ON [dbo].[MatchingResults]([BeneficiaryId]);
    
    CREATE INDEX [IX_MatchingResults_CaregiverProfileId] 
        ON [dbo].[MatchingResults]([CaregiverProfileId]);
    
    CREATE INDEX [IX_MatchingResults_OverallScore] 
        ON [dbo].[MatchingResults]([OverallScore] DESC);
    
    CREATE INDEX [IX_MatchingResults_CalculatedAt] 
        ON [dbo].[MatchingResults]([CalculatedAt] DESC);
    
    -- Composite index for common query pattern
    CREATE INDEX [IX_MatchingResults_Beneficiary_Score] 
        ON [dbo].[MatchingResults]([BeneficiaryId], [OverallScore] DESC, [IsDeleted]);
    
    PRINT 'Created MatchingResults table with indexes';
END
ELSE
BEGIN
    PRINT 'MatchingResults table already exists';
END
GO

-- =============================================
-- 3. Insert sample data for testing (Optional)
-- =============================================
-- This section adds sample personality assessments and preferences
-- Uncomment to populate test data

/*
-- Sample: Add personality assessment for an existing caregiver
DECLARE @CaregiverId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CaregiverProfiles WHERE VerificationStatus = 2); -- Approved

IF @CaregiverId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM PersonalityAssessments WHERE CaregiverProfileId = @CaregiverId)
BEGIN
    INSERT INTO PersonalityAssessments (Id, CaregiverProfileId, PersonalityType, ExtroversionScore, PatienceScore, EmpathyScore, CommunicationScore, FlexibilityScore, CompletedAt, CreatedAt)
    VALUES (NEWID(), @CaregiverId, 'ISFJ', 75, 90, 95, 85, 80, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Added sample personality assessment';
END

-- Sample: Add beneficiary preference
DECLARE @BeneficiaryId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Beneficiaries);

IF @BeneficiaryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM BeneficiaryPreferences WHERE BeneficiaryId = @BeneficiaryId)
BEGIN
    INSERT INTO BeneficiaryPreferences (Id, BeneficiaryId, RequiredSkills, PreferredPersonalityTraits, PreferredGender, MinAge, MaxAge, CreatedAt)
    VALUES (NEWID(), @BeneficiaryId, 'Dementia Care,Mobility Assistance,Medication Management', 'Patient,Empathetic,Communicative', NULL, 25, 55, GETUTCDATE());
    
    PRINT 'Added sample beneficiary preference';
END
*/

-- =============================================
-- 4. Verification queries
-- =============================================
PRINT '==============================================';
PRINT 'Database Schema Update Complete!';
PRINT '==============================================';
PRINT '';
PRINT 'Table Status:';
SELECT 
    'MatchingResults' AS TableName,
    COUNT(*) AS RecordCount
FROM MatchingResults
UNION ALL
SELECT 
    'BeneficiaryPreferences',
    COUNT(*)
FROM BeneficiaryPreferences;

PRINT '';
PRINT 'Index Status:';
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'MatchingResults'
ORDER BY i.index_id;

GO

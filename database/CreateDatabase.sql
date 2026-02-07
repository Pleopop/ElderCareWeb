-- =============================================
-- ElderCare Database Creation Script
-- Version: 1.0
-- Date: 2026-02-07
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ElderCareDb')
BEGIN
    ALTER DATABASE ElderCareDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ElderCareDb;
END
GO

-- Create database
CREATE DATABASE ElderCareDb;
GO

USE ElderCareDb;
GO

-- =============================================
-- USERS & AUTHENTICATION
-- =============================================

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Role INT NOT NULL, -- 0=Customer, 1=Caregiver, 2=Admin
    Status INT NOT NULL DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended, 3=Deleted
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- =============================================
-- CUSTOMER PROFILES
-- =============================================

CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Address NVARCHAR(500),
    EmergencyContact NVARCHAR(200),
    EmergencyPhone NVARCHAR(20),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE Beneficiaries (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    DateOfBirth DATETIME2 NOT NULL,
    Gender INT NOT NULL,
    Address NVARCHAR(500),
    MedicalConditions NVARCHAR(MAX),
    Medications NVARCHAR(MAX),
    Allergies NVARCHAR(500),
    MobilityLevel INT,
    CognitiveStatus INT,
    SpecialNeeds NVARCHAR(MAX),
    PersonalityTraits NVARCHAR(500),
    Hobbies NVARCHAR(500),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
);

CREATE TABLE BeneficiaryPreferences (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    PreferredGender INT,
    PreferredAgeRange NVARCHAR(50),
    PreferredLanguages NVARCHAR(200),
    PreferredPersonalityTraits NVARCHAR(500),
    AvoidPersonalityTraits NVARCHAR(500),
    SpecialRequirements NVARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id) ON DELETE CASCADE
);

-- =============================================
-- CAREGIVER PROFILES
-- =============================================

CREATE TABLE Caregivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    DateOfBirth DATETIME2 NOT NULL,
    Gender INT NOT NULL,
    Address NVARCHAR(500),
    Bio NVARCHAR(MAX),
    Experience INT NOT NULL,
    Certifications NVARCHAR(MAX),
    Languages NVARCHAR(200),
    HourlyRate DECIMAL(18,2) NOT NULL,
    VerificationStatus INT NOT NULL DEFAULT 0,
    VerifiedAt DATETIME2,
    VerifiedBy NVARCHAR(200),
    AverageRating DECIMAL(3,2),
    TotalReviews INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE CaregiverSkills (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    SkillName NVARCHAR(200) NOT NULL,
    ProficiencyLevel INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id) ON DELETE CASCADE
);

CREATE TABLE CaregiverAvailabilities (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    DayOfWeek INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id) ON DELETE CASCADE
);

CREATE TABLE PersonalityAssessments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    Openness INT NOT NULL,
    Conscientiousness INT NOT NULL,
    Extraversion INT NOT NULL,
    Agreeableness INT NOT NULL,
    EmotionalStability INT NOT NULL,
    Patience INT NOT NULL,
    Empathy INT NOT NULL,
    Adaptability INT NOT NULL,
    AssessedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id) ON DELETE CASCADE
);

-- =============================================
-- BOOKINGS
-- =============================================

CREATE TABLE Bookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    TotalHours DECIMAL(18,2) NOT NULL,
    HourlyRate DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    SpecialInstructions NVARCHAR(MAX),
    CancellationReason NVARCHAR(500),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id)
);

CREATE TABLE Reviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE
);

-- =============================================
-- PAYMENTS & WALLETS
-- =============================================

CREATE TABLE Wallets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    EscrowBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WalletId UNIQUEIDENTIFIER NOT NULL,
    BookingId UNIQUEIDENTIFIER,
    Type INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    Description NVARCHAR(500),
    ReferenceNumber NVARCHAR(100),
    ProcessedAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (WalletId) REFERENCES Wallets(Id),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);

-- =============================================
-- DISPUTES
-- =============================================

CREATE TABLE Disputes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Reason NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    Resolution NVARCHAR(2000),
    ResolvedBy NVARCHAR(200),
    ResolvedAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);

CREATE TABLE DisputeEvidences (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DisputeId UNIQUEIDENTIFIER NOT NULL,
    UploadedBy UNIQUEIDENTIFIER NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    FileType NVARCHAR(50),
    Description NVARCHAR(500),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (DisputeId) REFERENCES Disputes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedBy) REFERENCES Users(Id)
);

-- =============================================
-- AI MATCHING
-- =============================================

CREATE TABLE MatchingResults (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    OverallScore DECIMAL(5,2) NOT NULL,
    SkillMatchScore DECIMAL(5,2) NOT NULL,
    PersonalityMatchScore DECIMAL(5,2) NOT NULL,
    AvailabilityMatchScore DECIMAL(5,2) NOT NULL,
    PreferenceMatchScore DECIMAL(5,2) NOT NULL,
    MatchedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id)
);

-- =============================================
-- TRACKING
-- =============================================

CREATE TABLE LocationLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL,
    Latitude DECIMAL(10,8) NOT NULL,
    Longitude DECIMAL(11,8) NOT NULL,
    Accuracy DECIMAL(10,2),
    LoggedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE
);

-- =============================================
-- NOTIFICATIONS
-- =============================================

CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Type INT NOT NULL DEFAULT 0,
    Category INT NOT NULL DEFAULT 0,
    Priority INT NOT NULL DEFAULT 1,
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2,
    ActionUrl NVARCHAR(500),
    RelatedEntityId UNIQUEIDENTIFIER,
    RelatedEntityType NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- =============================================
-- AI CAREGIVER ASSISTANT
-- =============================================

CREATE TABLE CareNotes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL,
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    Observation NVARCHAR(2000) NOT NULL,
    AssessedMood INT NOT NULL,
    ObservedAt DATETIME2 NOT NULL,
    AiMoodAnalysis NVARCHAR(1000),
    SentimentScore DECIMAL(3,2),
    DetectedEmotions NVARCHAR(MAX),
    SuggestedActions NVARCHAR(MAX),
    RequiresAttention BIT NOT NULL DEFAULT 0,
    NotifiedCustomer BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id) ON DELETE CASCADE
);

CREATE TABLE ActivitySuggestions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Category INT NOT NULL,
    DurationMinutes INT NOT NULL,
    Difficulty INT NOT NULL,
    AiReasoning NVARCHAR(500),
    ConfidenceScore DECIMAL(3,2) NOT NULL,
    BasedOnTraits NVARCHAR(MAX),
    BasedOnHobbies NVARCHAR(MAX),
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME2,
    CaregiverFeedback NVARCHAR(1000),
    BeneficiaryEngagementRating INT,
    GeneratedAt DATETIME2 NOT NULL,
    ExpiresAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id) ON DELETE CASCADE
);

CREATE TABLE DailyReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL,
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ReportDate DATE NOT NULL,
    Summary NVARCHAR(2000) NOT NULL,
    AverageMood INT NOT NULL,
    ActivitiesCompleted NVARCHAR(MAX),
    MealsConsumed NVARCHAR(MAX),
    HealthNotes NVARCHAR(1000),
    BehaviorNotes NVARCHAR(1000),
    AiInsights NVARCHAR(1000),
    PositiveHighlights NVARCHAR(MAX),
    AreasOfConcern NVARCHAR(MAX),
    CaregiverNotes NVARCHAR(2000),
    CaregiverApproved BIT NOT NULL DEFAULT 0,
    ApprovedAt DATETIME2,
    ViewedByCustomer BIT NOT NULL DEFAULT 0,
    ViewedAt DATETIME2,
    GeneratedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id),
    FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id),
    FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

-- =============================================
-- INDEXES
-- =============================================

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Bookings_CustomerId ON Bookings(CustomerId);
CREATE INDEX IX_Bookings_CaregiverId ON Bookings(CaregiverId);
CREATE INDEX IX_Bookings_Status ON Bookings(Status);
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
CREATE INDEX IX_CareNotes_BeneficiaryId ON CareNotes(BeneficiaryId);
CREATE INDEX IX_CareNotes_RequiresAttention ON CareNotes(RequiresAttention);
CREATE INDEX IX_ActivitySuggestions_BeneficiaryId ON ActivitySuggestions(BeneficiaryId);
CREATE INDEX IX_DailyReports_BeneficiaryId ON DailyReports(BeneficiaryId);
CREATE UNIQUE INDEX IX_DailyReports_BookingId_ReportDate ON DailyReports(BookingId, ReportDate);

GO

PRINT 'ElderCare Database created successfully!';

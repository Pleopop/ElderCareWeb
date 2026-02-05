-- ELDER-CARE CONNECT Database Creation Script
-- Part 1: Core Tables

CREATE DATABASE ElderCareDb;
GO
USE ElderCareDb;
GO

-- Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL,
    PhoneNumber NVARCHAR(20),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    IsPhoneVerified BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
GO

-- CustomerProfiles
CREATE TABLE CustomerProfiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CustomerProfiles_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_CustomerProfiles_UserId ON CustomerProfiles(UserId);
GO

-- CaregiverProfiles
CREATE TABLE CaregiverProfiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Bio NVARCHAR(MAX),
    YearsOfExperience INT,
    HourlyRate DECIMAL(18,2),
    VerificationStatus INT NOT NULL DEFAULT 0,
    IdentityNumber NVARCHAR(50),
    IdentityImageUrl NVARCHAR(500),
    SelfieUrl NVARCHAR(500),
    CriminalRecordUrl NVARCHAR(500),
    ApprovedAt DATETIME2,
    ApprovedBy NVARCHAR(256),
    RejectionReason NVARCHAR(MAX),
    AverageRating FLOAT DEFAULT 0,
    TotalReviews INT DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CaregiverProfiles_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_CaregiverProfiles_UserId ON CaregiverProfiles(UserId);
CREATE INDEX IX_CaregiverProfiles_VerificationStatus ON CaregiverProfiles(VerificationStatus);
GO

-- Beneficiaries
CREATE TABLE Beneficiaries (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerProfileId UNIQUEIDENTIFIER NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    DateOfBirth DATETIME2 NOT NULL,
    Gender INT NOT NULL,
    CognitiveStatus INT NOT NULL,
    MedicalConditions NVARCHAR(MAX),
    Allergies NVARCHAR(MAX),
    Medications NVARCHAR(MAX),
    EmergencyContact NVARCHAR(200),
    EmergencyPhone NVARCHAR(20),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Beneficiaries_CustomerProfiles FOREIGN KEY (CustomerProfileId) REFERENCES CustomerProfiles(Id)
);
CREATE INDEX IX_Beneficiaries_CustomerProfileId ON Beneficiaries(CustomerProfileId);
GO

-- Bookings
CREATE TABLE Bookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerProfileId UNIQUEIDENTIFIER NOT NULL,
    CaregiverProfileId UNIQUEIDENTIFIER NOT NULL,
    BeneficiaryId UNIQUEIDENTIFIER NOT NULL,
    ScheduledStartTime DATETIME2 NOT NULL,
    ScheduledEndTime DATETIME2 NOT NULL,
    ActualStartTime DATETIME2,
    ActualEndTime DATETIME2,
    ServiceLocation NVARCHAR(500) NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    SpecialRequirements NVARCHAR(MAX),
    Status INT NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18,2) NOT NULL,
    EscrowAmount DECIMAL(18,2) NOT NULL,
    CommissionAmount DECIMAL(18,2) NOT NULL,
    CancellationReason NVARCHAR(MAX),
    CheckInLatitude FLOAT,
    CheckInLongitude FLOAT,
    CheckInPhotoUrl NVARCHAR(500),
    CheckOutLatitude FLOAT,
    CheckOutLongitude FLOAT,
    CheckOutNotes NVARCHAR(MAX),
    AiMatchScore FLOAT,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Bookings_CustomerProfiles FOREIGN KEY (CustomerProfileId) REFERENCES CustomerProfiles(Id),
    CONSTRAINT FK_Bookings_CaregiverProfiles FOREIGN KEY (CaregiverProfileId) REFERENCES CaregiverProfiles(Id),
    CONSTRAINT FK_Bookings_Beneficiaries FOREIGN KEY (BeneficiaryId) REFERENCES Beneficiaries(Id)
);
CREATE INDEX IX_Bookings_Status ON Bookings(Status);
GO

-- Reviews
CREATE TABLE Reviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookingId UNIQUEIDENTIFIER NOT NULL,
    CaregiverId UNIQUEIDENTIFIER NOT NULL,
    OverallRating INT NOT NULL,
    PunctualityRating INT NOT NULL,
    ProfessionalismRating INT NOT NULL,
    CommunicationRating INT NOT NULL,
    CareQualityRating INT NOT NULL,
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Reviews_Bookings FOREIGN KEY (BookingId) REFERENCES Bookings(Id),
    CONSTRAINT FK_Reviews_CaregiverProfiles FOREIGN KEY (CaregiverId) REFERENCES CaregiverProfiles(Id)
);
CREATE UNIQUE INDEX IX_Reviews_BookingId ON Reviews(BookingId);
GO

-- Wallets
CREATE TABLE Wallets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Wallets_UserId ON Wallets(UserId);
GO

PRINT 'Core tables created successfully!';

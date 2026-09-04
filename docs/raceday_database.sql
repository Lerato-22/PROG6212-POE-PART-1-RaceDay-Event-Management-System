
-- Student: Lerato Maropola
-- Module: PROG6212 - Programming 2B
-- Part: 1 - System Planning and Database

CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

IF OBJECT_ID('dbo.Results', 'U') IS NOT NULL DROP TABLE dbo.Results;
IF OBJECT_ID('dbo.Enrolments', 'U') IS NOT NULL DROP TABLE dbo.Enrolments;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Profiles', 'U') IS NOT NULL DROP TABLE dbo.Profiles;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO
 
CREATE TABLE dbo.Users (
    UserId            INT IDENTITY(1,1)  PRIMARY KEY,
    FirstName         NVARCHAR(100)      NOT NULL,
    LastName          NVARCHAR(100)      NOT NULL,
    Email             NVARCHAR(255)      NOT NULL UNIQUE,
    PasswordHash      NVARCHAR(512)      NOT NULL,
    Role              NVARCHAR(20)       NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    PhoneNumber       NVARCHAR(20)       NULL,
    ProfilePictureUrl NVARCHAR(1000)     NULL,
    CreatedAt         DATETIME           NOT NULL DEFAULT GETDATE()
);
GO
 
CREATE TABLE dbo.Profiles (
    ProfileId        INT IDENTITY(1,1)  PRIMARY KEY,
    UserId           INT                NOT NULL UNIQUE,
    DateOfBirth      DATE               NULL,
    Gender           NVARCHAR(20)       NULL,
    EmergencyContact NVARCHAR(200)      NULL,
    CONSTRAINT FK_Profiles_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users(UserId) ON DELETE CASCADE
);
GO
 
CREATE TABLE dbo.Events (
    EventId        INT IDENTITY(1,1)  PRIMARY KEY,
    OrganiserId    INT                NOT NULL,
    EventName      NVARCHAR(200)      NOT NULL,
    Description    NVARCHAR(MAX)      NULL,
    EventDate      DATETIME           NOT NULL,
    Location       NVARCHAR(300)      NOT NULL,
    Distance       DECIMAL(8,2)       NOT NULL,
    EventType      NVARCHAR(20)       NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    BannerImageUrl NVARCHAR(1000)     NULL,
    CreatedAt      DATETIME           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Events_Organiser FOREIGN KEY (OrganiserId)
        REFERENCES dbo.Users(UserId) ON DELETE CASCADE
);
GO
 
CREATE TABLE dbo.Categories (
    CategoryId   INT IDENTITY(1,1)  PRIMARY KEY,
    EventId      INT                NOT NULL,
    CategoryName NVARCHAR(100)      NOT NULL,
    Description  NVARCHAR(300)      NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId)
        REFERENCES dbo.Events(EventId) ON DELETE CASCADE
);
GO
 
CREATE TABLE dbo.Enrolments (
    EnrolmentId   INT IDENTITY(1,1)  PRIMARY KEY,
    ParticipantId INT                NOT NULL,
    EventId       INT                NOT NULL,
    CategoryId    INT                NOT NULL,
    Status        NVARCHAR(20)       NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    EnrolledAt    DATETIME           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Enrolments_Participant_Event UNIQUE (ParticipantId, EventId),
    CONSTRAINT FK_Enrolments_Participant FOREIGN KEY (ParticipantId)
        REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Enrolments_Event FOREIGN KEY (EventId)
        REFERENCES dbo.Events(EventId),
    CONSTRAINT FK_Enrolments_Category FOREIGN KEY (CategoryId)
        REFERENCES dbo.Categories(CategoryId)
);
GO
 
CREATE TABLE dbo.Results (
    ResultId          INT IDENTITY(1,1)  PRIMARY KEY,
    EnrolmentId       INT                NOT NULL UNIQUE,
    FinishTime        NVARCHAR(20)       NOT NULL,
    FinishingPosition INT                NOT NULL,
    TotalFinishers    INT                NOT NULL,
    RecordedAt        DATETIME           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Results_Enrolment FOREIGN KEY (EnrolmentId)
        REFERENCES dbo.Enrolments(EnrolmentId) ON DELETE CASCADE
);
GO
 
INSERT INTO dbo.Users (FirstName, LastName, Email, PasswordHash, Role, PhoneNumber)
VALUES
('Sarah',  'Nkosi',    'sarah.nkosi@raceday.co.za',    '$2a$11$examplehashOrganiser1xxxxxxxxxxxxxxxxxxxxxxx', 'Organiser',   '0821234567'),
('James',  'van Wyk',  'james.vanwyk@raceday.co.za',   '$2a$11$examplehashOrganiser2xxxxxxxxxxxxxxxxxxxxxxx', 'Organiser',   '0839876543'),
('Thabo',  'Dlamini',  'thabo.dlamini@raceday.co.za',  '$2a$11$examplehashParticipant1xxxxxxxxxxxxxxxxxxxxx', 'Participant',  '0711112222'),
('Lerato', 'Mokoena',  'lerato.mokoena@raceday.co.za', '$2a$11$examplehashParticipant2xxxxxxxxxxxxxxxxxxxxx', 'Participant',  '0723334444');
GO
 
INSERT INTO dbo.Profiles (UserId, DateOfBirth, Gender, EmergencyContact)
VALUES
(1, '1985-03-12', 'Female', 'Sipho Nkosi - 0821119999'),
(2, '1979-07-24', 'Male',   'Ann van Wyk - 0835557777'),
(3, '1998-11-05', 'Male',   'Grace Dlamini - 0716665555'),
(4, '2001-02-18', 'Female', 'Peter Mokoena - 0728884444');
GO
 
INSERT INTO dbo.Events (OrganiserId, EventName, Description, EventDate, Location, Distance, EventType)
VALUES
(1, 'Soweto 10K Fun Run',
    'A scenic 10km community run through the heart of Soweto. Suitable for all fitness levels.',
    '2026-08-09 07:00:00', 'Soweto, Johannesburg', 10.00, 'Run'),
(1, 'Pretoria Heritage Cycle Tour',
    'A 60km cycling event through the historic streets and parks of Pretoria on Heritage Day.',
    '2026-09-24 06:30:00', 'Pretoria, Gauteng', 60.00, 'Cycle'),
(2, 'Cape Winelands Half Marathon',
    'A stunning 21km run through the vineyards and mountains of the Western Cape.',
    '2026-10-18 07:30:00', 'Stellenbosch, Western Cape', 21.10, 'Run');
GO
 
INSERT INTO dbo.Categories (EventId, CategoryName, Description)
VALUES
(1, 'Under 20',       'Runners aged 19 and under'),
(1, 'Senior (20-39)', 'Runners aged 20 to 39'),
(1, 'Masters (40+)',  'Runners aged 40 and above'),
(2, '60km Open',      'Open category for all ages on the 60km route'),
(2, '60km Masters',   'Cyclists aged 50 and above on the 60km route'),
(3, '21km Open',      'Open category for all finishers'),
(3, '21km Women',     'Female-only category for the 21km route');
GO
 
INSERT INTO dbo.Enrolments (ParticipantId, EventId, CategoryId, Status)
VALUES
(3, 1, 2, 'Confirmed'),
(3, 3, 6, 'Pending'),
(4, 1, 1, 'Confirmed'),
(4, 2, 4, 'Pending');
GO
 
INSERT INTO dbo.Results (EnrolmentId, FinishTime, FinishingPosition, TotalFinishers)
VALUES
(1, '00:52:14', 47,  312),
(3, '01:08:33', 112, 312);
GO
 
SELECT 'Users'      AS TableName, COUNT(*) AS TotalRows FROM dbo.Users
UNION ALL
SELECT 'Profiles',   COUNT(*) FROM dbo.Profiles
UNION ALL
SELECT 'Events',     COUNT(*) FROM dbo.Events
UNION ALL
SELECT 'Categories', COUNT(*) FROM dbo.Categories
UNION ALL
SELECT 'Enrolments', COUNT(*) FROM dbo.Enrolments
UNION ALL
SELECT 'Results',    COUNT(*) FROM dbo.Results;
GO
 

-- ============================================================
--  WashNGo Database Schema
--  SQL Server (SSMS 20.1)
-- ============================================================

CREATE DATABASE WashNGo;
GO
USE WashNGo;
GO

-- ─────────────────────────────────────────
-- 1. ROLES
-- ─────────────────────────────────────────
CREATE TABLE Roles (
    RoleID   INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Customer');

-- ─────────────────────────────────────────
-- 2. USERS
-- ─────────────────────────────────────────
CREATE TABLE Users (
    UserID           INT IDENTITY(1,1) PRIMARY KEY,
    Username         NVARCHAR(100)  NOT NULL UNIQUE,
    Email            NVARCHAR(200)  NOT NULL UNIQUE,
    PasswordHash     NVARCHAR(500)  NOT NULL,
    PhoneNumber      NVARCHAR(20)   NOT NULL,
    Address          NVARCHAR(500)  NOT NULL,
    RoleID           INT            NOT NULL DEFAULT 2,
    RegistrationDate DATETIME       NOT NULL DEFAULT GETDATE(),
    LastLogin        DATETIME       NULL,
    AccountStatus    NVARCHAR(20)   NOT NULL DEFAULT 'Active',   -- Active | Suspended
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- Default admin account (password: Admin@123)
INSERT INTO Users (Username, Email, PasswordHash, PhoneNumber, Address, RoleID)
VALUES (
    'admin',
    'admin@washngo.com',
    'AQAAAAIAAYagAAAAEK9z2q1x8vP3mN7rL5tY6wZ0uA4cB8dE1fG2hJ3iK4lM5nO6pQ7rS8tU9vW0xY1zA==',
    '01700000000',
    'Dhaka, Bangladesh',
    1
);

-- ─────────────────────────────────────────
-- 3. SERVICES
-- ─────────────────────────────────────────
CREATE TABLE Services (
    ServiceID        INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName      NVARCHAR(200)  NOT NULL,
    Description      NVARCHAR(2000) NOT NULL,
    Price            DECIMAL(10,2)  NOT NULL,
    DurationMinutes  INT            NOT NULL,
    VehicleType      NVARCHAR(10)   NOT NULL CHECK (VehicleType IN ('Bike','Car','Both')),
    ImageUrl         NVARCHAR(500)  NULL,
    IsAvailable      BIT            NOT NULL DEFAULT 1,
    CreatedAt        DATETIME       NOT NULL DEFAULT GETDATE()
);

INSERT INTO Services (ServiceName, Description, Price, DurationMinutes, VehicleType, ImageUrl) VALUES
('Basic Bike Wash',      'Exterior rinse, foam wash, and dry for bikes.',                          150.00,  30,  'Bike', '/images/services/basic-bike.jpg'),
('Premium Bike Wash',    'Full exterior + chain lube + polish for bikes.',                          300.00,  60,  'Bike', '/images/services/premium-bike.jpg'),
('Basic Car Wash',       'Exterior rinse, foam wash, and dry for cars.',                            250.00,  45,  'Car',  '/images/services/basic-car.jpg'),
('Premium Car Wash',     'Full exterior wash, wax polish and tire shine.',                          500.00,  90,  'Car',  '/images/services/premium-car.jpg'),
('Interior Cleaning',    'Full interior vacuum, dashboard wipe, seat cleaning.',                    600.00, 120,  'Car',  '/images/services/interior.jpg'),
('Foam Wash',            'High-pressure foam cannon wash for deep cleaning.',                       350.00,  60,  'Both', '/images/services/foam.jpg'),
('Wax Polish Service',   'Hand-applied carnauba wax for lasting shine and protection.',             700.00, 150,  'Car',  '/images/services/wax.jpg'),
('Engine Cleaning',      'Safe degreaser-based engine bay cleaning and rinse.',                     800.00,  90,  'Car',  '/images/services/engine.jpg');

-- ─────────────────────────────────────────
-- 4. TIME SLOTS
-- ─────────────────────────────────────────
CREATE TABLE TimeSlots (
    SlotID       INT IDENTITY(1,1) PRIMARY KEY,
    SlotLabel    NVARCHAR(50)  NOT NULL,
    StartTime    TIME          NOT NULL,
    EndTime      TIME          NOT NULL,
    MaxBookings  INT           NOT NULL DEFAULT 5,
    IsActive     BIT           NOT NULL DEFAULT 1,
    DayOfWeek    INT           NOT NULL DEFAULT -1  -- 0=Sun,1=Mon..6=Sat. -1=legacy all-day
);
-- NOTE: Time slot data is seeded by WashNGo_SlotMatrix_Migration.sql
-- Run that script after this one to populate the weekly slot matrix.

-- ─────────────────────────────────────────
-- 5. BOOKINGS
-- ─────────────────────────────────────────
CREATE TABLE Bookings (
    BookingID       INT IDENTITY(1,1) PRIMARY KEY,
    ReservationID   AS ('WNG-' + RIGHT('000000' + CAST(BookingID AS NVARCHAR), 6)) PERSISTED,
    UserID          INT            NOT NULL,
    ServiceID       INT            NOT NULL,
    SlotID          INT            NOT NULL,
    BookingDate     DATE           NOT NULL,
    VehicleType     NVARCHAR(10)   NOT NULL CHECK (VehicleType IN ('Bike','Car')),
    VehicleNumber   NVARCHAR(50)   NULL,
    TotalAmount     DECIMAL(10,2)  NOT NULL,
    BookingStatus   NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
    PaymentMethod   NVARCHAR(50)   NOT NULL DEFAULT 'Cash on Service',
    PaymentStatus   NVARCHAR(20)   NOT NULL DEFAULT 'Unpaid',
    Notes           NVARCHAR(500)  NULL,
    CreatedAt       DATETIME       NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME       NULL,
    CONSTRAINT FK_Bookings_Users    FOREIGN KEY (UserID)    REFERENCES Users(UserID),
    CONSTRAINT FK_Bookings_Services FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    CONSTRAINT FK_Bookings_Slots    FOREIGN KEY (SlotID)    REFERENCES TimeSlots(SlotID),
    CONSTRAINT CHK_BookingStatus    CHECK (BookingStatus IN ('Pending','Confirmed','In Progress','Completed','Cancelled'))
);

-- Prevent double-booking: same user, same date, same slot
CREATE UNIQUE INDEX UX_Bookings_UserDateSlot
    ON Bookings (UserID, BookingDate, SlotID)
    WHERE BookingStatus NOT IN ('Cancelled');

-- ─────────────────────────────────────────
-- 6. PAYMENTS
-- ─────────────────────────────────────────
CREATE TABLE Payments (
    PaymentID     INT IDENTITY(1,1) PRIMARY KEY,
    BookingID     INT           NOT NULL,
    Amount        DECIMAL(10,2) NOT NULL,
    Method        NVARCHAR(50)  NOT NULL,
    TransactionID NVARCHAR(200) NULL,
    PaidAt        DATETIME      NULL,
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- ─────────────────────────────────────────
-- 7. REVIEWS
-- ─────────────────────────────────────────
CREATE TABLE Reviews (
    ReviewID   INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT            NOT NULL,
    ServiceID  INT            NOT NULL,
    BookingID  INT            NOT NULL,
    Rating     TINYINT        NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment    NVARCHAR(1000) NULL,
    CreatedAt  DATETIME       NOT NULL DEFAULT GETDATE(),
    IsVisible  BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Reviews_Users    FOREIGN KEY (UserID)    REFERENCES Users(UserID),
    CONSTRAINT FK_Reviews_Services FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    CONSTRAINT FK_Reviews_Bookings FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID),
    CONSTRAINT UX_Reviews_Booking  UNIQUE (BookingID)   -- one review per booking
);

-- ─────────────────────────────────────────
-- 8. CONTACT MESSAGES
-- ─────────────────────────────────────────
CREATE TABLE ContactMessages (
    MessageID  INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100)  NOT NULL,
    Email      NVARCHAR(200)  NOT NULL,
    Subject    NVARCHAR(300)  NOT NULL,
    Message    NVARCHAR(3000) NOT NULL,
    SentAt     DATETIME       NOT NULL DEFAULT GETDATE(),
    IsRead     BIT            NOT NULL DEFAULT 0
);

-- ─────────────────────────────────────────
-- 9. NOTIFICATIONS
-- ─────────────────────────────────────────
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID         INT            NOT NULL,
    Title          NVARCHAR(200)  NOT NULL,
    Message        NVARCHAR(1000) NOT NULL,
    IsRead         BIT            NOT NULL DEFAULT 0,
    CreatedAt      DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ─────────────────────────────────────────
-- USEFUL VIEWS
-- ─────────────────────────────────────────
GO
CREATE VIEW vw_BookingDetails AS
SELECT
    b.BookingID,
    b.ReservationID,
    u.Username,
    u.Email,
    u.PhoneNumber,
    s.ServiceName,
    s.Price,
    s.DurationMinutes,
    ts.SlotLabel,
    ts.StartTime,
    ts.EndTime,
    b.BookingDate,
    b.VehicleType,
    b.VehicleNumber,
    b.TotalAmount,
    b.BookingStatus,
    b.PaymentMethod,
    b.PaymentStatus,
    b.CreatedAt
FROM Bookings b
JOIN Users    u  ON b.UserID    = u.UserID
JOIN Services s  ON b.ServiceID = s.ServiceID
JOIN TimeSlots ts ON b.SlotID   = ts.SlotID;
GO

CREATE VIEW vw_SlotAvailability AS
SELECT
    ts.SlotID,
    ts.SlotLabel,
    ts.MaxBookings,
    b.BookingDate,
    COUNT(b.BookingID) AS BookedCount,
    ts.MaxBookings - COUNT(b.BookingID) AS AvailableSlots
FROM TimeSlots ts
CROSS JOIN (SELECT DISTINCT BookingDate FROM Bookings) dates
LEFT JOIN Bookings b
    ON ts.SlotID = b.SlotID
    AND b.BookingDate = dates.BookingDate
    AND b.BookingStatus NOT IN ('Cancelled')
GROUP BY ts.SlotID, ts.SlotLabel, ts.MaxBookings, b.BookingDate;
GO

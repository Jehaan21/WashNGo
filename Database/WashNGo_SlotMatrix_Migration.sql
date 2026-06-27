-- ============================================================
--  WashNGo_SlotMatrix_Migration.sql
--  Run this in SSMS AFTER WashNGo_Schema.sql
--  Adds DayOfWeek column and seeds the full weekly slot matrix
-- ============================================================

USE WashNGo;
GO

-- Step 1: Add DayOfWeek column if not already present (safe to run multiple times)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME='TimeSlots' AND COLUMN_NAME='DayOfWeek'
)
BEGIN
    ALTER TABLE TimeSlots ADD DayOfWeek INT NOT NULL DEFAULT -1;
    PRINT 'DayOfWeek column added.';
END
ELSE
    PRINT 'DayOfWeek column already exists - skipping.';
GO

-- Step 2: Clear any old generic slots (DayOfWeek = -1 means legacy)
DELETE FROM TimeSlots WHERE DayOfWeek = -1;
-- Also clear any previously seeded matrix slots before reseeding
DELETE FROM TimeSlots WHERE DayOfWeek >= 0;
PRINT 'Existing slots cleared.';
GO

-- Step 3: Seed weekly matrix (Mon=1 to Sun=0, hourly 5AM-10PM)
-- We generate slots for each day × each hour
DECLARE @day   INT;
DECLARE @hour  INT;
DECLARE @start TIME;
DECLARE @end   TIME;
DECLARE @label NVARCHAR(50);
DECLARE @ampm1 NVARCHAR(5);
DECLARE @ampm2 NVARCHAR(5);
DECLARE @h1    INT;
DECLARE @h2    INT;

-- Days: 0=Sun,1=Mon,2=Tue,3=Wed,4=Thu,5=Fri,6=Sat
SET @day = 0;
WHILE @day <= 6
BEGIN
    -- Hours: 5 AM to 9 PM (last slot ends at 10 PM)
    SET @hour = 5;
    WHILE @hour <= 21
    BEGIN
        SET @start = CAST(CAST(@hour AS VARCHAR) + ':00:00' AS TIME);
        SET @end   = CAST(CAST(@hour + 1 AS VARCHAR) + ':00:00' AS TIME);

        -- Build label e.g. "9:00 AM – 10:00 AM"
        SET @h1   = @hour;
        SET @ampm1 = CASE WHEN @h1 < 12 THEN 'AM' WHEN @h1 = 12 THEN 'PM' ELSE 'PM' END;
        IF @h1 > 12 SET @h1 = @h1 - 12;
        IF @h1 = 0  SET @h1 = 12;

        SET @h2   = @hour + 1;
        SET @ampm2 = CASE WHEN @h2 < 12 THEN 'AM' WHEN @h2 = 12 THEN 'PM' ELSE 'PM' END;
        IF @h2 > 12 SET @h2 = @h2 - 12;
        IF @h2 = 0  SET @h2 = 12;

        SET @label = CAST(@h1 AS VARCHAR) + ':00 ' + @ampm1 + ' – ' +
                     CAST(@h2 AS VARCHAR) + ':00 ' + @ampm2;

        -- Default: Mon-Fri active, Sat-Sun inactive
        INSERT INTO TimeSlots (SlotLabel, StartTime, EndTime, MaxBookings, IsActive, DayOfWeek)
        VALUES (
            @label,
            @start,
            @end,
            5,
            CASE WHEN @day BETWEEN 1 AND 5 THEN 1 ELSE 0 END,
            @day
        );

        SET @hour = @hour + 1;
    END
    SET @day = @day + 1;
END

PRINT 'Weekly slot matrix seeded successfully.';
SELECT COUNT(*) AS TotalSlotsCreated FROM TimeSlots;
GO

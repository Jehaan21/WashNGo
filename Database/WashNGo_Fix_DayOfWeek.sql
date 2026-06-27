-- ============================================================
--  WashNGo_Fix_DayOfWeek.sql
--  Run this in SSMS to fix the "Invalid column name 'DayOfWeek'" error
--  This is a one-time fix for existing databases
-- ============================================================

USE WashNGo;
GO

-- Step 1: Add DayOfWeek column if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TimeSlots' AND COLUMN_NAME = 'DayOfWeek'
)
BEGIN
    ALTER TABLE TimeSlots ADD DayOfWeek INT NOT NULL DEFAULT -1;
    PRINT '✅ DayOfWeek column added successfully.';
END
ELSE
BEGIN
    PRINT '⚠️ DayOfWeek column already exists - skipping ALTER.';
END
GO

-- Step 2: Clear all old slots (they are incompatible with the new matrix system)
DELETE FROM TimeSlots;
PRINT '✅ Old time slots cleared.';
GO

-- Step 3: Seed the full weekly slot matrix (Mon–Sun, 5 AM – 10 PM hourly)
DECLARE @day   INT = 0;
DECLARE @hour  INT;
DECLARE @start TIME;
DECLARE @end   TIME;
DECLARE @label NVARCHAR(50);
DECLARE @h1    INT;
DECLARE @h2    INT;
DECLARE @ampm1 NVARCHAR(5);
DECLARE @ampm2 NVARCHAR(5);

WHILE @day <= 6
BEGIN
    SET @hour = 5;
    WHILE @hour <= 21
    BEGIN
        SET @start = CAST(CAST(@hour     AS VARCHAR) + ':00:00' AS TIME);
        SET @end   = CAST(CAST(@hour + 1 AS VARCHAR) + ':00:00' AS TIME);

        -- Format start hour
        SET @h1    = @hour;
        SET @ampm1 = CASE WHEN @h1 < 12 THEN 'AM' ELSE 'PM' END;
        IF @h1 = 0  SET @h1 = 12;
        IF @h1 > 12 SET @h1 = @h1 - 12;

        -- Format end hour
        SET @h2    = @hour + 1;
        SET @ampm2 = CASE WHEN @h2 < 12 THEN 'AM' ELSE 'PM' END;
        IF @h2 = 0  SET @h2 = 12;
        IF @h2 > 12 SET @h2 = @h2 - 12;

        SET @label = CAST(@h1 AS VARCHAR) + ':00 ' + @ampm1
                   + ' – '
                   + CAST(@h2 AS VARCHAR) + ':00 ' + @ampm2;

        INSERT INTO TimeSlots (SlotLabel, StartTime, EndTime, MaxBookings, IsActive, DayOfWeek)
        VALUES (
            @label,
            @start,
            @end,
            5,
            -- Mon(1)–Fri(5) active by default; Sat(6) and Sun(0) inactive
            CASE WHEN @day BETWEEN 1 AND 5 THEN 1 ELSE 0 END,
            @day
        );

        SET @hour = @hour + 1;
    END
    SET @day = @day + 1;
END

PRINT '✅ Weekly slot matrix seeded: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' slots created.';

-- Verify
SELECT
    CASE DayOfWeek
        WHEN 0 THEN 'Sunday'    WHEN 1 THEN 'Monday'
        WHEN 2 THEN 'Tuesday'   WHEN 3 THEN 'Wednesday'
        WHEN 4 THEN 'Thursday'  WHEN 5 THEN 'Friday'
        WHEN 6 THEN 'Saturday'
    END AS [Day],
    COUNT(*) AS [Total Slots],
    SUM(CAST(IsActive AS INT)) AS [Active],
    COUNT(*) - SUM(CAST(IsActive AS INT)) AS [Inactive]
FROM TimeSlots
GROUP BY DayOfWeek
ORDER BY DayOfWeek;
GO

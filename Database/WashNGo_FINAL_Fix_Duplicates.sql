-- ============================================================
--  WashNGo_FINAL_Fix_Duplicates.sql
--  Run in SSMS → WashNGo database → F5
-- ============================================================

USE WashNGo;
GO

-- Step 1: See what's in there right now
SELECT COUNT(*) AS TotalSlots FROM TimeSlots;
GO

-- Step 2: Keep only ONE slot per (DayOfWeek, StartTime) combination
-- This deletes the duplicates while keeping one of each
WITH CTE AS (
    SELECT SlotID,
           ROW_NUMBER() OVER (
               PARTITION BY DayOfWeek, StartTime
               ORDER BY SlotID ASC
           ) AS RowNum
    FROM TimeSlots
)
DELETE FROM CTE WHERE RowNum > 1;

PRINT 'Duplicates removed. Kept one slot per day/hour.';
PRINT 'Remaining slots: ' + CAST(@@ROWCOUNT AS VARCHAR);
GO

-- Step 3: Also remove any old-style slots (DayOfWeek = -1)
DELETE FROM TimeSlots WHERE DayOfWeek = -1;
PRINT 'Old legacy slots removed.';
GO

-- Step 4: Verify - should be exactly 17 per day
SELECT
    CASE DayOfWeek
        WHEN 0 THEN 'Sunday'
        WHEN 1 THEN 'Monday'
        WHEN 2 THEN 'Tuesday'
        WHEN 3 THEN 'Wednesday'
        WHEN 4 THEN 'Thursday'
        WHEN 5 THEN 'Friday'
        WHEN 6 THEN 'Saturday'
        ELSE 'Unknown (DayOfWeek=' + CAST(DayOfWeek AS VARCHAR) + ')'
    END AS [Day],
    COUNT(*) AS [Total Slots],
    SUM(CAST(IsActive AS INT)) AS [Active],
    COUNT(*) - SUM(CAST(IsActive AS INT)) AS [Inactive]
FROM TimeSlots
GROUP BY DayOfWeek
ORDER BY DayOfWeek;
GO

-- ============================================================
--  WashNGo_AdminFix.sql
--  Run this in SSMS AFTER running WashNGo_Schema.sql
--  This replaces the placeholder admin password hash.
--  The app (Program.cs → AdminSeeder) will also fix this
--  automatically on first startup — but running this SQL
--  manually is an alternative if needed.
-- ============================================================

USE WashNGo;
GO

-- Remove placeholder admin so the app re-creates it with a proper hash
-- OR just leave it — AdminSeeder in Program.cs handles it automatically.

-- If you want to manually check the admin user:
SELECT UserID, Username, Email, RoleID, AccountStatus,
       LEFT(PasswordHash, 30) + '...' AS HashPreview
FROM Users
WHERE RoleID = 1;

-- To promote any registered user to Admin (replace the email):
-- UPDATE Users SET RoleID = 1 WHERE Email = 'your@email.com';

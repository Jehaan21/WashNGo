-- ============================================================
--  WashNGo_ForgotPassword_Migration.sql
--  Adds PasswordResetTokens table
--  Run in SSMS → WashNGo database → F5
-- ============================================================

USE WashNGo;
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'PasswordResetTokens'
)
BEGIN
    CREATE TABLE PasswordResetTokens (
        TokenID     INT IDENTITY(1,1) PRIMARY KEY,
        UserID      INT            NOT NULL,
        Token       NVARCHAR(200)  NOT NULL UNIQUE,
        ExpiresAt   DATETIME       NOT NULL,
        IsUsed      BIT            NOT NULL DEFAULT 0,
        CreatedAt   DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ResetTokens_Users FOREIGN KEY (UserID)
            REFERENCES Users(UserID) ON DELETE CASCADE
    );
    PRINT 'PasswordResetTokens table created.';
END
ELSE
    PRINT 'PasswordResetTokens table already exists.';
GO

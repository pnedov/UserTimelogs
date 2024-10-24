using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UsersTimeLog.Repository;

namespace UsersTimeLog.Services;

public class SystemService : ISystemService
{
	private readonly string StoredProcedureName = "SeedDb";
   
    private UsersTimeLogsDbContext _context;

    /// <summary>
    /// initialize DbContext object
    /// </summary>
    /// <param name="_context"></param>
    public SystemService(UsersTimeLogsDbContext context)
    {
        _context = context;
    }

    public async Task InitializeDatabaseAsync(CancellationToken token)
    {
		_context.Database.EnsureCreated();
		if (!IsStoredProcedureExistsAsync(token))
		{
            await SeedDatabase(token);
		}

        await CallStoredProcedure(token);
    }

    private async Task CallStoredProcedure(CancellationToken cancellationToken)
    {
        var sql = $"EXEC {StoredProcedureName}";
        await _context.Database.ExecuteSqlRawAsync(sql);
    }

    private async Task SeedDatabase(CancellationToken cancellationToken)
    {
		await CreateStoredProcedure(cancellationToken);
    }

    public bool IsStoredProcedureExistsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = "SELECT COUNT(object_id) as count FROM [users_timelogs].[sys].[objects] WHERE [name] = @name";
        using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", StoredProcedureName);
                connection.Open();
                var count = (int)command.ExecuteScalar();
                connection.Close();

                return count > 0;
            }
        }
    }

    private async Task CreateStoredProcedure(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sql = @$"
        CREATE PROCEDURE dbo.SeedDb
        	AS
        	BEGIN
        		SET NOCOUNT ON;
        		-- delete all records 
        		DELETE FROM users;
        		DELETE FROM projects;
        		DELETE FROM time_logs;
        		-- generate random records in users table 
        		DECLARE @FirstNames NVARCHAR(MAX) = 'John,Gringo,Mark,Lisa,Maria,Sonya,Philip,Jose,Lorenzo,George,Justin';  
        		DECLARE @LastNames NVARCHAR(MAX) = 'Johnson,Lamas,Jackson,Brown,Mason,Rodriguez,Roberts,Thomas,Rose,McDonalds';
        		DECLARE @Domains NVARCHAR(MAX) = 'hotmailcom,gmailcom,livecom';
        		DECLARE @RandomNames TABLE (FirstName NVARCHAR(64), LastName NVARCHAR(64), Domain NVARCHAR(128));
        		-- Insert split names into the temporary table
        		INSERT INTO @RandomNames (FirstName, LastName, Domain)
        		SELECT 
        			fn.value AS FirstName,
        			ln.value AS LastName,
        			d.value AS Domain
        		FROM STRING_SPLIT(@FirstNames, ',') AS fn
        		CROSS JOIN STRING_SPLIT(@LastNames, ',') AS ln
        		CROSS JOIN STRING_SPLIT(@Domains, ',') AS d;
        		-- Create a temporary table to hold potential unique combinations
        		DECLARE @PotentialUsers TABLE (Email NVARCHAR(128) PRIMARY KEY, FirstName NVARCHAR(64), LastName NVARCHAR(64));
        		INSERT INTO @PotentialUsers (Email, FirstName, LastName)
        		SELECT 
        			CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain) AS Email,
        			n.FirstName,
        			n.LastName
        		FROM @RandomNames n
        		WHERE NOT EXISTS (
        			SELECT 1 
        			FROM [dbo].[users] u 
        			WHERE u.email = CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain)
        		)
        		AND NOT EXISTS (
        			SELECT 1 
        			FROM @PotentialUsers pu 
        			WHERE pu.Email = CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain)
        		);
        		IF (SELECT COUNT(*) FROM @PotentialUsers) < 100
        		BEGIN
        			DECLARE @Count INT = (SELECT COUNT(*) FROM @PotentialUsers);
        			WHILE @Count < 100
        			BEGIN
        				INSERT INTO @PotentialUsers (Email, FirstName, LastName)
        				SELECT TOP 1
        					CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain) AS Email,
        					n.FirstName,
        					n.LastName
        				FROM @RandomNames n
        				WHERE NOT EXISTS (
        					SELECT 1 
        					FROM @PotentialUsers pu 
        					WHERE pu.Email = CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain)
        				)
        				AND NOT EXISTS (
        					SELECT 1 
        					FROM [dbo].[users] u 
        					WHERE u.email = CONCAT(LOWER(n.FirstName), '.', LOWER(n.LastName), '@', n.Domain)
        				)
        				ORDER BY NEWID();  -- Randomize to add more diversity
        				SET @Count = (SELECT COUNT(*) FROM @PotentialUsers);
        			END
        		END
        		-- Finally, select 100 unique records to insert into the users table
        		INSERT INTO [dbo].[users] (fname, lname, email)
        		SELECT TOP (100) FirstName, LastName, Email
        		FROM @PotentialUsers
        		ORDER BY NEWID();  -- Randomize the selection
        		-- generate records for Projects table 
        		INSERT INTO projects ([name])
        		VALUES ('My own'), ('Free Time'), ('Work');
        		-- generate records for time_logs table 
        		DECLARE @UserId INT
        		DECLARE @ProjectId INT
        		DECLARE @Date DATE
        		DECLARE @Hours FLOAT
        		DECLARE @TotalHours FLOAT
        		DECLARE @UserCount INT
        		DECLARE @ProjectCount INT
        		DECLARE @MinID INT, @MaxID INT;
        		SELECT  @MinID = MIN(id), @MaxID = MAX(id) FROM projects;
        		DECLARE UserCursor CURSOR FOR
        		SELECT id FROM users
        		OPEN UserCursor
        		FETCH NEXT FROM UserCursor INTO @UserId
        		WHILE @@FETCH_STATUS = 0
        		BEGIN
        			-- Generate a random number of time log entries for the user
        			DECLARE @EntryCount INT = FLOOR(RAND() * 20) + 1
        			SET @TotalHours = 0
        			-- Loop through each entry
        			WHILE @EntryCount > 0
        			BEGIN
        				-- Generate random project, date, and hours
        				SET @ProjectId = FLOOR(RAND() * (@MaxID - @MinID + 1)) + @MinID;
        				SET @Date = DATEADD(DAY, FLOOR(RAND() * -180), GETDATE())
        				SET @Hours = ROUND(RAND() * 7.75 + 0.25, 2)
        				-- Ensure total hours per day do not exceed 8
        				IF @TotalHours + @Hours > 8
        				BEGIN
        					SET @Hours = 8 - @TotalHours
        				END
        				INSERT INTO time_logs (users_id, projects_id, date, hours)
        				VALUES (@UserId, @ProjectId, @Date, @Hours)
        				SET @TotalHours = @TotalHours + @Hours
        				SET @EntryCount = @EntryCount - 1
        				-- Reset total hours if a new day is reached
        				IF @TotalHours >= 8
        				BEGIN
        					SET @TotalHours = 0
        				END
        			END
        			FETCH NEXT FROM UserCursor INTO @UserId
        		END
        		CLOSE UserCursor
        		DEALLOCATE UserCursor
        	END";

        //await Task.FromResult(_context.Database.ExecuteSql($"{sql}"));
        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}


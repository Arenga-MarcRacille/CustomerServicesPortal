using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Web.Migrations
{
    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- 1. Seed TicketStatus (Critical for the Enum to work) ---
            // IDs: 0=Open, 1=InProgress, 2=Resolved, 3=Closed
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM TicketStatus)
                BEGIN
                    INSERT INTO TicketStatus (StatusName) VALUES (0);
                    INSERT INTO TicketStatus (StatusName) VALUES (1);
                    INSERT INTO TicketStatus (StatusName) VALUES (2);
                    INSERT INTO TicketStatus (StatusName) VALUES (3);
                END
            ");

            // --- 2. Procedure: Create Ticket ---
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_CreateTicket
                    @ClientId INT,
                    @AssetId INT,
                    @Title NVARCHAR(200),
                    @Description NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Logic: Default status is 'Open' (StatusName=0, so ID might be 1 depending on identity)
                    -- To be safe, let's look up the ID for 'Open' (0)
                    DECLARE @DefaultStatusId INT;
                    SELECT @DefaultStatusId = StatusId FROM TicketStatus WHERE StatusName = 0;

                    DECLARE @NewTicketId INT;

                    INSERT INTO Tickets (ClientId, AssetId, TicketStatusId, Title, Description, CreatedAt)
                    VALUES (@ClientId, @AssetId, @DefaultStatusId, @Title, @Description, GETDATE());

                    SET @NewTicketId = SCOPE_IDENTITY();

                    -- Log to Timeline automatically
                    INSERT INTO TicketTimelines (TicketId, ActivityDescription, LogDate)
                    VALUES (@NewTicketId, 'Ticket Created', GETDATE());

                    SELECT @NewTicketId AS TicketId;
                END
            ");

            // --- 3. Procedure: Get Client Tickets ---
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetClientTickets
                    @ClientId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        t.TicketId,
                        t.Title,
                        t.Description,
                        t.CreatedAt,
                        s.StatusName AS StatusEnumId,
                        a.AssetName
                    FROM Tickets t
                    INNER JOIN Assets a ON t.AssetId = a.AssetId
                    INNER JOIN TicketStatus s ON t.TicketStatusId = s.StatusId
                    WHERE t.ClientId = @ClientId
                    ORDER BY t.CreatedAt DESC;
                END
            ");

            // --- 4. Procedure: Update Status ---
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdateTicketStatus
                    @TicketId INT,
                    @NewStatusId INT,
                    @UpdaterName NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE Tickets
                    SET TicketStatusId = @NewStatusId
                    WHERE TicketId = @TicketId;

                    DECLARE @LogMessage NVARCHAR(200);
                    SET @LogMessage = 'Status updated to ID ' + CAST(@NewStatusId AS NVARCHAR(10)) + ' by ' + @UpdaterName;

                    INSERT INTO TicketTimelines (TicketId, ActivityDescription, LogDate)
                    VALUES (@TicketId, @LogMessage, GETDATE());
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Always provide a rollback path!
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateTicket");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetClientTickets");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateTicketStatus");
            // We usually don't delete seeded data in Down() unless necessary, to preserve integrity.
        }
    }
}
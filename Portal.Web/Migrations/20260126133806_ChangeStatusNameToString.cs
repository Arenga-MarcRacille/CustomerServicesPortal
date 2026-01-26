using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusNameToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop the old Enum column and recreate it as String
            // (Simplest way to handle the type change cleanly for the exam)
            migrationBuilder.Sql("DELETE FROM TicketStatus"); // Clear old data first
            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "TicketStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 2. Seed the table with STRINGS now
            migrationBuilder.Sql(@"
                INSERT INTO TicketStatus (StatusName) VALUES ('Open');
                INSERT INTO TicketStatus (StatusName) VALUES ('InProgress');
                INSERT INTO TicketStatus (StatusName) VALUES ('Resolved');
                INSERT INTO TicketStatus (StatusName) VALUES ('Closed');
            ");

                    // 3. UPDATE Stored Procedure: sp_CreateTicket
                    // It needs to look for 'Open' instead of 0
                    migrationBuilder.Sql(@"
                ALTER PROCEDURE sp_CreateTicket
                    @ClientId INT,
                    @AssetId INT,
                    @Title NVARCHAR(200),
                    @Description NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- FIX: Look for string 'Open'
                    DECLARE @DefaultStatusId INT;
                    SELECT @DefaultStatusId = StatusId FROM TicketStatus WHERE StatusName = 'Open';

                    DECLARE @NewTicketId INT;

                    INSERT INTO Tickets (ClientId, AssetId, TicketStatusId, Title, Description, CreatedAt)
                    VALUES (@ClientId, @AssetId, @DefaultStatusId, @Title, @Description, GETDATE());

                    SET @NewTicketId = SCOPE_IDENTITY();

                    INSERT INTO TicketTimelines (TicketId, ActivityDescription, LogDate)
                    VALUES (@NewTicketId, 'Ticket Created', GETDATE());

                    SELECT @NewTicketId AS TicketId;
                END
            ");

                    // 4. UPDATE Procedure: sp_GetClientTickets
                    // No change needed logically, but running ALTER ensures it recompiles with the new schema
                    migrationBuilder.Sql(@"
                ALTER PROCEDURE sp_GetClientTickets
                    @ClientId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        t.TicketId,
                        t.Title,
                        t.Description,
                        t.CreatedAt,
                        s.StatusName, -- This will now return 'Open'/'Closed' directly!
                        a.AssetName
                    FROM Tickets t
                    INNER JOIN Assets a ON t.AssetId = a.AssetId
                    INNER JOIN TicketStatus s ON t.TicketStatusId = s.StatusId
                    WHERE t.ClientId = @ClientId
                    ORDER BY t.CreatedAt DESC;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusName",
                table: "TicketStatus",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowZeroCurrentPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.Sql(
                "ALTER TABLE GameSessions DROP CONSTRAINT IF EXISTS CK_GameSessions_CurrentPlayers;"
            );

            migrationBuilder.Sql(
                "ALTER TABLE GameSessions ADD CONSTRAINT CK_GameSessions_CurrentPlayers CHECK (CurrentPlayers >= 0 AND CurrentPlayers <= 100);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.Sql(
                "ALTER TABLE GameSessions DROP CONSTRAINT IF EXISTS CK_GameSessions_CurrentPlayers;"
            );

            migrationBuilder.Sql(
                "ALTER TABLE GameSessions ADD CONSTRAINT CK_GameSessions_CurrentPlayers CHECK (CurrentPlayers >= 1 AND CurrentPlayers <= 100);"
            );
        }
    }
}

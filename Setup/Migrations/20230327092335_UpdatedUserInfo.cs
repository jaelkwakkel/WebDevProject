using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "highScore",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GameFinishData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    WonGame = table.Column<bool>(type: "bit", nullable: false),
                    WinnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetupUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameFinishData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameFinishData_AspNetUsers_SetupUserId",
                        column: x => x.SetupUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameFinishData_SetupUserId",
                table: "GameFinishData",
                column: "SetupUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameFinishData");

            migrationBuilder.DropColumn(
                name: "highScore",
                table: "AspNetUsers");
        }
    }
}

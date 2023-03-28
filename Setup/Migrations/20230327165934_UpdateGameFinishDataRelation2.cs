using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameFinishDataRelation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetupUserId1",
                table: "GameFinishData",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameFinishData_SetupUserId1",
                table: "GameFinishData",
                column: "SetupUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId1",
                table: "GameFinishData",
                column: "SetupUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId1",
                table: "GameFinishData");

            migrationBuilder.DropIndex(
                name: "IX_GameFinishData_SetupUserId1",
                table: "GameFinishData");

            migrationBuilder.DropColumn(
                name: "SetupUserId1",
                table: "GameFinishData");
        }
    }
}

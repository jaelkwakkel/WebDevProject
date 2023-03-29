using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class DeleteGamesOnDeleteUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId",
                table: "GameFinishData");

            migrationBuilder.AddForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId",
                table: "GameFinishData",
                column: "SetupUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId",
                table: "GameFinishData");

            migrationBuilder.AddForeignKey(
                name: "FK_GameFinishData_AspNetUsers_SetupUserId",
                table: "GameFinishData",
                column: "SetupUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

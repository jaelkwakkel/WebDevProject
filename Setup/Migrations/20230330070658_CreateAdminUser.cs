using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class CreateAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "HighScore", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "24396ef8-4708-496b-92e3-fce56fc743c5", 0, "83a22fd1-2983-4fb9-9fdd-ccd0d1afb4fe", "gamemailservice18+test@gmail.com", true, 0, false, null, null, null, "AQAAAAEAACcQAAAAEGDQ0dsG/t4/zmOmno2KLogurm6pahfFUmY78783IlHS8z6Zh8CXveNMuwCUZhT5bw==", null, false, "a921ade0-d4db-4c17-a8be-605622cd23cd", false, "testadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "24396ef8-4708-496b-92e3-fce56fc743c5");
        }
    }
}

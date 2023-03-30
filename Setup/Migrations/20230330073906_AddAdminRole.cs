using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "24396ef8-4708-496b-92e3-fce56fc743c5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "28dd4688-4f35-4364-868d-d76c8baffa31", "03776b77-3453-45fd-87ce-a6da17ef0fc7", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "HighScore", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "264cde90-7d7c-408c-9018-229e7cb9166f", 0, "b5733dd4-a817-48fa-aa62-c5cefbd1a3bb", "gamemailservice18+test@gmail.com", true, 0, false, null, "GAMEMAILSERVICE18+TEST@GMAIL.COM", "TESTADMIN", "AQAAAAEAACcQAAAAELYZ0oMitodFE1XQnlJtqevzyzkswPqXJu2jsiWoMkmkuB8cDRi1bNf+UEQS0IjPiQ==", null, false, "c0f3f24d-014c-4923-a2ca-6d2e2ccb52f4", false, "testadmin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "28dd4688-4f35-4364-868d-d76c8baffa31", "264cde90-7d7c-408c-9018-229e7cb9166f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "28dd4688-4f35-4364-868d-d76c8baffa31", "264cde90-7d7c-408c-9018-229e7cb9166f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28dd4688-4f35-4364-868d-d76c8baffa31");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "264cde90-7d7c-408c-9018-229e7cb9166f");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "HighScore", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "24396ef8-4708-496b-92e3-fce56fc743c5", 0, "83a22fd1-2983-4fb9-9fdd-ccd0d1afb4fe", "gamemailservice18+test@gmail.com", true, 0, false, null, null, null, "AQAAAAEAACcQAAAAEGDQ0dsG/t4/zmOmno2KLogurm6pahfFUmY78783IlHS8z6Zh8CXveNMuwCUZhT5bw==", null, false, "a921ade0-d4db-4c17-a8be-605622cd23cd", false, "testadmin" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3a6a8866-d58e-4b9b-9f40-0341d78b9204", "967b0f66-d096-49c9-a244-eda25f2ea4dd", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "HighScore", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "cb776a53-9b54-4eba-bf35-6a8ace0a418e", 0, "afc67e2d-62ef-48b8-a3d1-25f69cebad85", "gamemailservice18+test@gmail.com", true, 0, false, null, "GAMEMAILSERVICE18+TEST@GMAIL.COM", "TESTADMIN", "AQAAAAEAACcQAAAAEMsGXsZqW8ntxpSC1Q05Z++GwCQivQXmzz1KO3GbBmpHfB/QFJ9s+YiDhmGaCN8K6w==", null, false, "f61c7c90-9a5b-49be-a789-5801536416a2", false, "testadmin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "3a6a8866-d58e-4b9b-9f40-0341d78b9204", "cb776a53-9b54-4eba-bf35-6a8ace0a418e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3a6a8866-d58e-4b9b-9f40-0341d78b9204", "cb776a53-9b54-4eba-bf35-6a8ace0a418e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a6a8866-d58e-4b9b-9f40-0341d78b9204");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cb776a53-9b54-4eba-bf35-6a8ace0a418e");

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
    }
}

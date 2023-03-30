using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminLogica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "c79d3d41-1379-45b1-8f77-aae3bd6042ac", "39d8cb28-9752-4001-8bff-d967472795d0", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "HighScore", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "54173ae5-e1fd-461a-960d-c9c666157f45", 0, "ce8a2326-6153-40d3-b546-36d5e7afcc2e", "gamemailservice18+test@gmail.com", true, 0, false, null, "GAMEMAILSERVICE18+TEST@GMAIL.COM", "TESTADMIN", "AQAAAAEAACcQAAAAELuiTcjFL1MtsbtLsGh0V9zSTqJ2abpMsJK+lJDOqAsX2ootXto4zwsyNilFWY1LBA==", null, false, "3ebb999b-f27a-4e77-b6c9-8f114c8aa9fd", false, "testadmin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c79d3d41-1379-45b1-8f77-aae3bd6042ac", "54173ae5-e1fd-461a-960d-c9c666157f45" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c79d3d41-1379-45b1-8f77-aae3bd6042ac", "54173ae5-e1fd-461a-960d-c9c666157f45" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c79d3d41-1379-45b1-8f77-aae3bd6042ac");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "54173ae5-e1fd-461a-960d-c9c666157f45");

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
    }
}

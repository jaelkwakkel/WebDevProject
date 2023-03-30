using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminLogicaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c79d3d41-1379-45b1-8f77-aae3bd6042ac",
                column: "ConcurrencyStamp",
                value: "81dee3a7-becd-4e26-b329-0d6fe3c8cee2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "54173ae5-e1fd-461a-960d-c9c666157f45",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "252c107c-81fa-4033-9c44-e3a0a174e6c5", "gamemailservice18+admin@gmail.com", "GAMEMAILSERVICE18+ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEN3OKe9bgd1MKxYWlVdTBoDn0SN84XXNUZDG0UUkileLfpWdwmkzzJBgu5iWJ/D5NA==", "f69f80a4-3f75-4e96-8bdd-63d2ec713c6d", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c79d3d41-1379-45b1-8f77-aae3bd6042ac",
                column: "ConcurrencyStamp",
                value: "39d8cb28-9752-4001-8bff-d967472795d0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "54173ae5-e1fd-461a-960d-c9c666157f45",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "ce8a2326-6153-40d3-b546-36d5e7afcc2e", "gamemailservice18+test@gmail.com", "GAMEMAILSERVICE18+TEST@GMAIL.COM", "TESTADMIN", "AQAAAAEAACcQAAAAELuiTcjFL1MtsbtLsGh0V9zSTqJ2abpMsJK+lJDOqAsX2ootXto4zwsyNilFWY1LBA==", "3ebb999b-f27a-4e77-b6c9-8f114c8aa9fd", "testadmin" });
        }
    }
}

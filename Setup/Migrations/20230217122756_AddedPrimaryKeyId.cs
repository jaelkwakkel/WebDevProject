using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrimaryKeyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactFormModels",
                table: "ContactFormModels");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ContactFormModels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ContactFormModels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactFormModels",
                table: "ContactFormModels",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactFormModels",
                table: "ContactFormModels");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContactFormModels");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ContactFormModels",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactFormModels",
                table: "ContactFormModels",
                column: "Email");
        }
    }
}

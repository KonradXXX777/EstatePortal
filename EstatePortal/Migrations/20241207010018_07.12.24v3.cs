using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstatePortal.Migrations
{
    /// <inheritdoc />
    public partial class _071224v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Area",
                table: "Announcements",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Announcements",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Street",
                table: "Announcements");

            migrationBuilder.AlterColumn<double>(
                name: "Area",
                table: "Announcements",
                type: "double",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");
        }
    }
}

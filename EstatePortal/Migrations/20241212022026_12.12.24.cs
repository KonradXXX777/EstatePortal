using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstatePortal.Migrations
{
    /// <inheritdoc />
    public partial class _121224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaleOrRent",
                table: "Announcements",
                newName: "SellOrRent");

            migrationBuilder.RenameColumn(
                name: "IsDisabledAccessible",
                table: "AnnouncementFeatures",
                newName: "IsAccessible");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Announcements",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Area",
                table: "Announcements",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "HasForest",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasGarden",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLake",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMountains",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPark",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSea",
                table: "AnnouncementFeatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Neighborhood",
                table: "AnnouncementFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Security",
                table: "AnnouncementFeatures",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasForest",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "HasGarden",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "HasLake",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "HasMountains",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "HasPark",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "HasSea",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "AnnouncementFeatures");

            migrationBuilder.DropColumn(
                name: "Security",
                table: "AnnouncementFeatures");

            migrationBuilder.RenameColumn(
                name: "SellOrRent",
                table: "Announcements",
                newName: "SaleOrRent");

            migrationBuilder.RenameColumn(
                name: "IsAccessible",
                table: "AnnouncementFeatures",
                newName: "IsDisabledAccessible");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Announcements",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "Area",
                table: "Announcements",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}

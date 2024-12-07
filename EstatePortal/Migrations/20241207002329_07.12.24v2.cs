using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstatePortal.Migrations
{
    /// <inheritdoc />
    public partial class _071224v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_BuyerId",
                table: "Chats",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SellerId",
                table: "Chats",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_BuyerId",
                table: "Chats",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_SellerId",
                table: "Chats",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_BuyerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_SellerId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_BuyerId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_SellerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Chats");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstatePortal.Migrations
{
    /// <inheritdoc />
    public partial class edit_chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "LastMessage",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Chats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Chats",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Chats",
                type: "int",
                nullable: true);

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
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_SellerId",
                table: "Chats",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

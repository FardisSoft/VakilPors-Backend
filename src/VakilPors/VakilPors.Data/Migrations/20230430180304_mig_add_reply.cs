using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class mig_add_reply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyId",
                table: "ChatMessage",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ReplyId",
                table: "ChatMessage",
                column: "ReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_ChatMessage_ReplyId",
                table: "ChatMessage",
                column: "ReplyId",
                principalTable: "ChatMessage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_ChatMessage_ReplyId",
                table: "ChatMessage");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ReplyId",
                table: "ChatMessage");

            migrationBuilder.DropColumn(
                name: "ReplyId",
                table: "ChatMessage");
        }
    }
}

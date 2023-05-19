using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class mig_add_visits_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyId",
                table: "ChatMessage",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Visitor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserGUID = table.Column<string>(type: "text", nullable: false),
                    IPv4 = table.Column<string>(type: "text", nullable: false),
                    visitTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ReplyId",
                table: "ChatMessage",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitor_UserGUID",
                table: "Visitor",
                column: "UserGUID",
                unique: true);

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

            migrationBuilder.DropTable(
                name: "Visitor");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ReplyId",
                table: "ChatMessage");

            migrationBuilder.DropColumn(
                name: "ReplyId",
                table: "ChatMessage");
        }
    }
}

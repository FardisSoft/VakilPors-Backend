using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class user_sub_1tM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed",
                column: "UserId",
                unique: true);
        }
    }
}

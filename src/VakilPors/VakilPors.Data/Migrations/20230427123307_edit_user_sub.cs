using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class edit_user_sub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed",
                column: "UserId");
        }
    }
}

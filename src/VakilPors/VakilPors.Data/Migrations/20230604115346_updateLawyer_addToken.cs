using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class updateLawyer_addToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tokens",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "Lawyer");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class addFieldToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentCategory",
                table: "LegalDocument",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumBudget",
                table: "LegalDocument",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumBudget",
                table: "LegalDocument",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentCategory",
                table: "LegalDocument");

            migrationBuilder.DropColumn(
                name: "MaximumBudget",
                table: "LegalDocument");

            migrationBuilder.DropColumn(
                name: "MinimumBudget",
                table: "LegalDocument");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class add_status_to_report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Report",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Report");
        }
    }
}

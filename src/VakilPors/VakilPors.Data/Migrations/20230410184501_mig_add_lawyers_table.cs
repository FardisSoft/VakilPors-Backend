using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class mig_add_lawyers_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LawyerId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lawyer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    ParvandeNo = table.Column<string>(type: "text", nullable: false),
                    IsAuthorized = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lawyer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lawyer_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LawyerId",
                table: "AspNetUsers",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lawyer_UserId",
                table: "Lawyer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Lawyer_LawyerId",
                table: "AspNetUsers",
                column: "LawyerId",
                principalTable: "Lawyer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Lawyer_LawyerId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Lawyer");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LawyerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LawyerId",
                table: "AspNetUsers");
        }
    }
}

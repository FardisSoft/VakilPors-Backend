using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class updateLawyerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallingCardImageUrl",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Grade",
                table: "Lawyer",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Lawyer",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberOf",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeAddress",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeLink",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specialties",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "CallingCardImageUrl",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "MemberOf",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "OfficeAddress",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "ResumeLink",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "Specialties",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Lawyer");
        }
    }
}

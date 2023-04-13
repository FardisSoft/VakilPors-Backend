using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAnswers",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLikes",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRates",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVerifies",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumbereOfConsulations",
                table: "Lawyer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileBackgroundPictureUrl",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "RatesList",
                table: "Lawyer",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfAnswers",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfLikes",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfRates",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfVerifies",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumbereOfConsulations",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "ProfileBackgroundPictureUrl",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "RatesList",
                table: "Lawyer");
        }
    }
}

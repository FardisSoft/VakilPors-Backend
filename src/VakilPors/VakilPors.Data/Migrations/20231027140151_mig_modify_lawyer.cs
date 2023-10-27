using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class mig_modify_lawyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "IsAuthorized",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfAnswers",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfLikes",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumberOfVerifies",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "NumbereOfConsulations",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "ParvandeNo",
                table: "Lawyer");

            migrationBuilder.DropColumn(
                name: "RatesList",
                table: "Lawyer");

            migrationBuilder.RenameColumn(
                name: "visitTime",
                table: "Visitor",
                newName: "VisitTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitTime",
                table: "Visitor",
                newName: "visitTime");

            migrationBuilder.AddColumn<byte>(
                name: "Grade",
                table: "Lawyer",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthorized",
                table: "Lawyer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
                name: "ParvandeNo",
                table: "Lawyer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "RatesList",
                table: "Lawyer",
                type: "text[]",
                nullable: true);
        }
    }
}

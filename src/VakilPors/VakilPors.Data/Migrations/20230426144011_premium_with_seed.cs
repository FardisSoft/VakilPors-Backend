using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class premium_with_seed : Migration
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

            migrationBuilder.CreateTable(
                name: "Premium",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premium", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribed",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PremiumID = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribed", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subscribed_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscribed_Premium_PremiumID",
                        column: x => x.PremiumID,
                        principalTable: "Premium",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Premium",
                columns: new[] { "Id", "ServiceType" },
                values: new object[,]
                {
                    { 1, 0 },
                    { 2, 1 },
                    { 3, 2 },
                    { 4, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_PremiumID",
                table: "Subscribed",
                column: "PremiumID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscribed");

            migrationBuilder.DropTable(
                name: "Premium");

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

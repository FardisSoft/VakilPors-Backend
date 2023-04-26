using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class Subscribed : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "SubscribedID",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubscribedID",
                table: "AspNetUsers",
                column: "SubscribedID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_PremiumID",
                table: "Subscribed",
                column: "PremiumID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribed_UserId",
                table: "Subscribed",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Subscribed_SubscribedID",
                table: "AspNetUsers",
                column: "SubscribedID",
                principalTable: "Subscribed",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Subscribed_SubscribedID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Subscribed");

            migrationBuilder.DropTable(
                name: "Premium");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SubscribedID",
                table: "AspNetUsers");

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

            migrationBuilder.DropColumn(
                name: "SubscribedID",
                table: "AspNetUsers");
        }
    }
}

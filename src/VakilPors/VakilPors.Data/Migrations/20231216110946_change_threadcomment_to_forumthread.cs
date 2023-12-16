using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VakilPors.Data.Migrations
{
    public partial class change_threadcomment_to_forumthread : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_ThreadComment_CommentId",
                table: "Report");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Report",
                newName: "ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_CommentId",
                table: "Report",
                newName: "IX_Report_ThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ForumThread_ThreadId",
                table: "Report",
                column: "ThreadId",
                principalTable: "ForumThread",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_ForumThread_ThreadId",
                table: "Report");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "Report",
                newName: "CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_ThreadId",
                table: "Report",
                newName: "IX_Report_CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ThreadComment_CommentId",
                table: "Report",
                column: "CommentId",
                principalTable: "ThreadComment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CommentBlog_BlogID",
                table: "CommentBlog",
                column: "BlogID");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentBlog_Account_AccountID",
                table: "CommentBlog",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentBlog_Blogs_BlogID",
                table: "CommentBlog",
                column: "BlogID",
                principalTable: "Blogs",
                principalColumn: "BlogID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentBlog_Account_AccountID",
                table: "CommentBlog");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentBlog_Blogs_BlogID",
                table: "CommentBlog");

            migrationBuilder.DropIndex(
                name: "IX_CommentBlog_BlogID",
                table: "CommentBlog");
        }
    }
}

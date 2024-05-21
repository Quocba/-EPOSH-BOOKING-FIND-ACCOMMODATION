using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdateBlogComment1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentBlog_FeedBack_FeedBackID",
                table: "CommentBlog");

            migrationBuilder.DropIndex(
                name: "IX_CommentBlog_FeedBackID",
                table: "CommentBlog");

            migrationBuilder.DropColumn(
                name: "FeedBackID",
                table: "CommentBlog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeedBackID",
                table: "CommentBlog",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentBlog_FeedBackID",
                table: "CommentBlog",
                column: "FeedBackID");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentBlog_FeedBack_FeedBackID",
                table: "CommentBlog",
                column: "FeedBackID",
                principalTable: "FeedBack",
                principalColumn: "FeedBackID");
        }
    }
}

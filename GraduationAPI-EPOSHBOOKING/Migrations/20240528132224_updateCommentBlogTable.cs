using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateCommentBlogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentBlog",
                table: "CommentBlog");

            migrationBuilder.AddColumn<int>(
                name: "CommentID",
                table: "CommentBlog",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentBlog",
                table: "CommentBlog",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentBlog_AccountID",
                table: "CommentBlog",
                column: "AccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentBlog",
                table: "CommentBlog");

            migrationBuilder.DropIndex(
                name: "IX_CommentBlog_AccountID",
                table: "CommentBlog");

            migrationBuilder.DropColumn(
                name: "CommentID",
                table: "CommentBlog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentBlog",
                table: "CommentBlog",
                columns: new[] { "AccountID", "BlogID" });
        }
    }
}

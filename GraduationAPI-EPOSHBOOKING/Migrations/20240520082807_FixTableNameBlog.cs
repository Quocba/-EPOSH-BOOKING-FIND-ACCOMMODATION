using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class FixTableNameBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogIamge_Blogs_BlogID",
                table: "BlogIamge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogIamge",
                table: "BlogIamge");

            migrationBuilder.RenameTable(
                name: "BlogIamge",
                newName: "BlogImage");

            migrationBuilder.RenameIndex(
                name: "IX_BlogIamge_BlogID",
                table: "BlogImage",
                newName: "IX_BlogImage_BlogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogImage",
                table: "BlogImage",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImage_Blogs_BlogID",
                table: "BlogImage",
                column: "BlogID",
                principalTable: "Blogs",
                principalColumn: "BlogID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImage_Blogs_BlogID",
                table: "BlogImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogImage",
                table: "BlogImage");

            migrationBuilder.RenameTable(
                name: "BlogImage",
                newName: "BlogIamge");

            migrationBuilder.RenameIndex(
                name: "IX_BlogImage_BlogID",
                table: "BlogIamge",
                newName: "IX_BlogIamge_BlogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogIamge",
                table: "BlogIamge",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogIamge_Blogs_BlogID",
                table: "BlogIamge",
                column: "BlogID",
                principalTable: "Blogs",
                principalColumn: "BlogID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class changeNameTableBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Account_AccountID",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "Desciption",
                table: "CommentBlog",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Blogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Account_AccountID",
                table: "Blogs",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Account_AccountID",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CommentBlog",
                newName: "Desciption");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Account_AccountID",
                table: "Blogs",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

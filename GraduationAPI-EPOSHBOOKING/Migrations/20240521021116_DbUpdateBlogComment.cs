using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdateBlogComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking");

            migrationBuilder.AddColumn<int>(
                name: "FeedBackID",
                table: "CommentBlog",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VoucherID",
                table: "Booking",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CommentBlog_FeedBackID",
                table: "CommentBlog",
                column: "FeedBackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "VoucherID");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentBlog_FeedBack_FeedBackID",
                table: "CommentBlog",
                column: "FeedBackID",
                principalTable: "FeedBack",
                principalColumn: "FeedBackID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentBlog_FeedBack_FeedBackID",
                table: "CommentBlog");

            migrationBuilder.DropIndex(
                name: "IX_CommentBlog_FeedBackID",
                table: "CommentBlog");

            migrationBuilder.DropColumn(
                name: "FeedBackID",
                table: "CommentBlog");

            migrationBuilder.AlterColumn<int>(
                name: "VoucherID",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "VoucherID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

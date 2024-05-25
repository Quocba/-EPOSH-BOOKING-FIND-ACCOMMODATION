using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Account_AccountID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Booking_BookingID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Hotel_HotelID",
                table: "FeedBack");

            migrationBuilder.AlterColumn<int>(
                name: "HotelID",
                table: "FeedBack",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BookingID",
                table: "FeedBack",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "FeedBack",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Account_AccountID",
                table: "FeedBack",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Booking_BookingID",
                table: "FeedBack",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Hotel_HotelID",
                table: "FeedBack",
                column: "HotelID",
                principalTable: "Hotel",
                principalColumn: "HotelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Account_AccountID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Booking_BookingID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Hotel_HotelID",
                table: "FeedBack");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Booking");

            migrationBuilder.AlterColumn<int>(
                name: "HotelID",
                table: "FeedBack",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BookingID",
                table: "FeedBack",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "FeedBack",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Account_AccountID",
                table: "FeedBack",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Booking_BookingID",
                table: "FeedBack",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Hotel_HotelID",
                table: "FeedBack",
                column: "HotelID",
                principalTable: "Hotel",
                principalColumn: "HotelID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class editAllowNullTableBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Account_AccountID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Room_RoomID",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "RoomID",
                table: "Booking",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Booking",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Account_AccountID",
                table: "Booking",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Room_RoomID",
                table: "Booking",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Account_AccountID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Room_RoomID",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomID",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Account_AccountID",
                table: "Booking",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Room_RoomID",
                table: "Booking",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

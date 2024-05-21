using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateIcollectionHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_HotelAddress_AddressID",
                table: "Hotel");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "Hotel",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_HotelAddress_AddressID",
                table: "Hotel",
                column: "AddressID",
                principalTable: "HotelAddress",
                principalColumn: "AddressID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_HotelAddress_AddressID",
                table: "Hotel");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "Hotel",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_HotelAddress_AddressID",
                table: "Hotel",
                column: "AddressID",
                principalTable: "HotelAddress",
                principalColumn: "AddressID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

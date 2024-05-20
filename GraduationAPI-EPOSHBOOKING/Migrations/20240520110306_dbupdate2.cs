using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelID",
                table: "HotelService",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelService_HotelID",
                table: "HotelService",
                column: "HotelID");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelService_Hotel_HotelID",
                table: "HotelService",
                column: "HotelID",
                principalTable: "Hotel",
                principalColumn: "HotelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelService_Hotel_HotelID",
                table: "HotelService");

            migrationBuilder.DropIndex(
                name: "IX_HotelService_HotelID",
                table: "HotelService");

            migrationBuilder.DropColumn(
                name: "HotelID",
                table: "HotelService");
        }
    }
}

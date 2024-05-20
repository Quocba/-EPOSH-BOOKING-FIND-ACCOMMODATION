using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelSubService_Hotel_HotelID",
                table: "HotelSubService");

            migrationBuilder.DropIndex(
                name: "IX_HotelSubService_HotelID",
                table: "HotelSubService");

            migrationBuilder.DropColumn(
                name: "HotelID",
                table: "HotelSubService");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelID",
                table: "HotelSubService",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HotelSubService_HotelID",
                table: "HotelSubService",
                column: "HotelID");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelSubService_Hotel_HotelID",
                table: "HotelSubService",
                column: "HotelID",
                principalTable: "Hotel",
                principalColumn: "HotelID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSetupDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "RoomService",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomService_RoomID",
                table: "RoomService",
                column: "RoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomService_Room_RoomID",
                table: "RoomService",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomService_Room_RoomID",
                table: "RoomService");

            migrationBuilder.DropIndex(
                name: "IX_RoomService_RoomID",
                table: "RoomService");

            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "RoomService");
        }
    }
}

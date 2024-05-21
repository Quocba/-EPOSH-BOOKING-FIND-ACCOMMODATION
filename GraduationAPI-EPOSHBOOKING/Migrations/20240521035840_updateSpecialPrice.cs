using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateSpecialPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Voucher",
                newName: "QuantityUseed");

            migrationBuilder.CreateTable(
                name: "SpecialPrice",
                columns: table => new
                {
                    SpecialPriceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialPrice", x => x.SpecialPriceID);
                    table.ForeignKey(
                        name: "FK_SpecialPrice_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialPrice_RoomID",
                table: "SpecialPrice",
                column: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialPrice");

            migrationBuilder.RenameColumn(
                name: "QuantityUseed",
                table: "Voucher",
                newName: "Quantity");
        }
    }
}

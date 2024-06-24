using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateFeedbacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "FeedBack");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FeedBack",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "FeedBack");

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "FeedBack",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

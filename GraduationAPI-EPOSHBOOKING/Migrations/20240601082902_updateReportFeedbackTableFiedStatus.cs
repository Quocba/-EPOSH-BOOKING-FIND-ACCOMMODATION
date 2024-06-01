using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateReportFeedbackTableFiedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "FeedBack",
                newName: "isDeleted");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ReportFeedBack",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ReportFeedBack");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "FeedBack",
                newName: "IsBlocked");
        }
    }
}

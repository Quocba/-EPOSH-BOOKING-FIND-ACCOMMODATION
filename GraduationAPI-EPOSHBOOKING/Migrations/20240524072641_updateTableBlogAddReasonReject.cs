using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationAPI_EPOSHBOOKING.Migrations
{
    /// <inheritdoc />
    public partial class updateTableBlogAddReasonReject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonReject",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonReject",
                table: "Blogs");
        }
    }
}

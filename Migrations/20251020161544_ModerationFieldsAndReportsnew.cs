using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App.Migrations
{
    /// <inheritdoc />
    public partial class ModerationFieldsAndReportsnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PostReports",
                newName: "PostReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostReportId",
                table: "PostReports",
                newName: "Id");
        }
    }
}

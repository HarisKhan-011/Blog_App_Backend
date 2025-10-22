using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App.Migrations
{
    /// <inheritdoc />
    public partial class Addpostpagestatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Postpages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Postpages");
        }
    }
}

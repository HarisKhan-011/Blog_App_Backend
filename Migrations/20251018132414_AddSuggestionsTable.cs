using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    SuggestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuggestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    AuthorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentSuggestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.SuggestionId);
                    table.ForeignKey(
                        name: "FK_Suggestions_Postpages_PostId",
                        column: x => x.PostId,
                        principalTable: "Postpages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suggestions_Suggestions_ParentSuggestionId",
                        column: x => x.ParentSuggestionId,
                        principalTable: "Suggestions",
                        principalColumn: "SuggestionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_ParentSuggestionId",
                table: "Suggestions",
                column: "ParentSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_PostId",
                table: "Suggestions",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suggestions");
        }
    }
}

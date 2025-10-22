using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App.Migrations
{
    /// <inheritdoc />
    public partial class ModerationFieldsAndReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "PostReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNote",
                table: "PostReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "PostReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedBy",
                table: "PostReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Postpages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Postpages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Postpages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ResolutionNote",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "ResolvedBy",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Postpages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Postpages");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Postpages");
        }
    }
}
